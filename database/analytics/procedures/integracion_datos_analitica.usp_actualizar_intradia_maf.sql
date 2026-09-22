/*
===============================================================================
MAF INTRADAY CHANGE DETECTION - DEFINICION CANONICA VERSIONADA
Procedimiento: integracion_datos_analitica.usp_actualizar_intradia_maf

El SQL Agent ejecuta este procedimiento cada 15 minutos. El procedimiento
calcula un fingerprint del origen y solo llama al ETL diario cuando existe un
cambio, cambia el dia o se solicita @Forzar = 1.
===============================================================================
*/

USE [aval_cob];
GO

CREATE OR ALTER PROCEDURE
    integracion_datos_analitica.usp_actualizar_intradia_maf
(
    @Forzar BIT = 0,
    @MostrarResultado BIT = 0
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @IdCliente INT = 59,
        @IdContrato INT = 246,
        @IdGrupo INT = 194,

        @AhoraUtc DATETIME2(3) = SYSUTCDATETIME(),
        @AhoraPeru DATETIME2(3) =
            DATEADD(HOUR, -5, SYSUTCDATETIME()),
        @FechaCorte DATE,
        @FechaHasta DATETIME2(3),

        @Anio SMALLINT,
        @Mes TINYINT,

        @LockResult INT,
        @LockResource NVARCHAR(255) =
            N'aval_cob:analitica:MAF_INTRADIA',

        @EstadoExiste BIT = 0,
        @FingerprintAnterior VARBINARY(32),
        @FechaCorteAnterior DATE,

        @CantidadCarteras INT = 0,

        @CantidadDocumentos BIGINT = 0,
        @CantidadDeudores BIGINT = 0,
        @SaldoDocumentos DECIMAL(38,4) = 0,
        @ChecksumDocumentos INT = 0,

        @CantidadGestiones BIGINT = 0,
        @IdMaxGestion BIGINT = NULL,
        @FechaMaxGestion DATETIME2(3) = NULL,
        @ChecksumGestiones INT = 0,

        @CantidadPagos BIGINT = 0,
        @IdMaxPago BIGINT = NULL,
        @FechaMaxPago DATETIME2(3) = NULL,
        @MontoPagos DECIMAL(38,4) = 0,
        @ChecksumPagos INT = 0,

        @FingerprintActual VARBINARY(32),
        @DebeProcesar BIT = 0,

        @InicioProcesoUtc DATETIME2(3),
        @FinProcesoUtc DATETIME2(3),
        @DuracionMs BIGINT;

    SET @FechaCorte = CONVERT(DATE, @AhoraPeru);
    SET @FechaHasta =
        DATEADD(DAY, 1, CONVERT(DATETIME2(3), @FechaCorte));

    SET @Anio = YEAR(@FechaCorte);
    SET @Mes = MONTH(@FechaCorte);


    /* ------------------------------------------------------------------------
       Dependencias
       ------------------------------------------------------------------------ */

    IF OBJECT_ID(
           N'integracion_datos_analitica.usp_ejecutar_diario_maf',
           N'P'
       ) IS NULL
        THROW 53010, 'Falta usp_ejecutar_diario_maf.', 1;

    IF OBJECT_ID(
           N'integracion_datos_analitica.estado_intradia_maf',
           N'U'
       ) IS NULL
        THROW 53011, 'Falta estado_intradia_maf.', 1;


    /* ------------------------------------------------------------------------
       Una sola ejecucion MAF a la vez.
       ------------------------------------------------------------------------ */

    EXEC @LockResult = sys.sp_getapplock
        @Resource = @LockResource,
        @LockMode = 'Exclusive',
        @LockOwner = 'Session',
        @LockTimeout = 0;

    IF @LockResult < 0
    BEGIN
        IF @MostrarResultado = 1
        BEGIN
            SELECT
                'OMITIDO_LOCK' AS resultado,
                @FechaCorte AS fecha_corte,
                @AhoraUtc AS comprobado_utc;
        END;

        RETURN 0;
    END;


    BEGIN TRY

        /* --------------------------------------------------------------------
           Carteras MAF del mes vigente.
           Mismo contrato funcional de los loaders MAF.
           -------------------------------------------------------------------- */

        IF OBJECT_ID('tempdb..#CarterasMafIntradia') IS NOT NULL
            DROP TABLE #CarterasMafIntradia;

        SELECT
            ca.nId_Cartera
        INTO #CarterasMafIntradia
        FROM dbo.av_Cartera AS ca
        WHERE ca.nId_Cliente = @IdCliente
          AND ca.nId_Contrato = @IdContrato
          AND ca.nId_Grupo = @IdGrupo
          AND ca.nAnioCar = @Anio
          AND ca.nCampCar = @Mes
          AND ca.cCiclo IN
              (
                  'CICLO 04',
                  'CICLO 11',
                  'CICLO 18',
                  'CICLO 25'
              )
          AND UPPER(ISNULL(ca.cCar_Nombre, ''))
              NOT LIKE '%BORRADOR%';

        CREATE UNIQUE CLUSTERED INDEX
            IX_CarterasMafIntradia
        ON #CarterasMafIntradia(nId_Cartera);

        SELECT
            @CantidadCarteras = COUNT(*)
        FROM #CarterasMafIntradia;

        IF @CantidadCarteras = 0
            THROW 53012,
                  'No existen carteras MAF productivas para el mes vigente.',
                  1;


        /* --------------------------------------------------------------------
           Fingerprint DOCUMENTOS.
           Incluye cantidad, deudores, saldo y checksum de campos que afectan
           el snapshot MAF. No se usa como metrica financiera; solo para cambio.
           -------------------------------------------------------------------- */

        SELECT
            @CantidadDocumentos = COUNT_BIG(*),

            @CantidadDeudores =
                CONVERT(BIGINT, COUNT(DISTINCT dc.nId_PersDeudor)),

            @SaldoDocumentos =
                CONVERT
                (
                    DECIMAL(38,4),
                    ISNULL
                    (
                        SUM
                        (
                            CONVERT
                            (
                                DECIMAL(38,4),
                                ISNULL(dc.nDoc_ImpSaldo, 0)
                            )
                        ),
                        0
                    )
                ),

            @ChecksumDocumentos =
                ISNULL
                (
                    CHECKSUM_AGG
                    (
                        BINARY_CHECKSUM
                        (
                            dc.nId_DocxCobrar,
                            dc.nId_PersDeudor,
                            dc.nId_DocxCobrarEst,
                            dc.cCampo4,
                            dc.nId_Moneda,
                            dc.nDoc_ImpTotal,
                            dc.nDoc_ImpSaldo,
                            dc.dDoc_FecIngreso
                        )
                    ),
                    0
                )

        FROM dbo.av_DocxCobrar AS dc

        INNER JOIN #CarterasMafIntradia AS c
            ON c.nId_Cartera = dc.nId_Cartera

        WHERE dc.nId_Cliente = @IdCliente
          AND ISNULL(dc.nId_DocxCobrarEst, 1) <> 8
          AND UPPER(LTRIM(RTRIM(ISNULL(dc.cCampo4, ''))))
              <> 'RETIRADO'
          AND dc.dDoc_FecIngreso < @FechaHasta;


        /* --------------------------------------------------------------------
           Fingerprint GESTIONES.
           Se observa todo lo relevante de las carteras vigentes hasta hoy,
           no solo el dia, porque una correccion historica puede cambiar el
           snapshot y/o una promesa anterior.
           -------------------------------------------------------------------- */

        SELECT
            @CantidadGestiones = COUNT_BIG(*),
            @IdMaxGestion =
                MAX(CONVERT(BIGINT, o.nId_DocxCobrarOpe)),
            @FechaMaxGestion =
                MAX
                (
                    CONVERT
                    (
                        DATETIME2(3),
                        COALESCE
                        (
                            o.dDoc_FecIngresoGes,
                            o.dDocCobOpe_FecIni
                        )
                    )
                ),
            @ChecksumGestiones =
                ISNULL
                (
                    CHECKSUM_AGG
                    (
                        BINARY_CHECKSUM
                        (
                            o.nId_DocxCobrarOpe,
                            o.nId_DocxCobrar,
                            o.nId_PersDeudor,
                            o.nId_UsuOpe,
                            o.nId_OpeCodOut,
                            o.tip_gestion,
                            o.dDoc_FecIngresoGes,
                            o.dDocCobOpe_FecIni,
                            o.dFechCompromisoPago,
                            o.monto_comp,
                            o.monto_compDolares
                        )
                    ),
                    0
                )
        FROM dbo.av_DocxCobrarOpe AS o
        INNER JOIN #CarterasMafIntradia AS c
            ON c.nId_Cartera = o.nId_Cartera
        WHERE o.nId_Cliente = @IdCliente
          AND o.tip_gestion IN (1, 2, 4)
          AND o.nId_UsuOpe IS NOT NULL
          AND COALESCE
              (
                  o.dDoc_FecIngresoGes,
                  o.dDocCobOpe_FecIni
              ) < @FechaHasta;


        /* --------------------------------------------------------------------
           Fingerprint PAGOS.
           Los pagos posteriores pueden modificar cumplimiento de promesas
           previas; por eso forman parte del cambio de origen.
           -------------------------------------------------------------------- */

        SELECT
            @CantidadPagos = COUNT_BIG(*),
            @IdMaxPago =
                MAX(CONVERT(BIGINT, p.nId_DocxPago)),
            @FechaMaxPago =
                MAX(CONVERT(DATETIME2(3), p.dDoc_FecPago)),
            @MontoPagos =
                CONVERT
                (
                    DECIMAL(38,4),
                    ISNULL
                    (
                        SUM
                        (
                            CONVERT
                            (
                                DECIMAL(38,4),
                                ISNULL(p.nDoc_ImpPago, 0)
                            )
                        ),
                        0
                    )
                ),
            @ChecksumPagos =
                ISNULL
                (
                    CHECKSUM_AGG
                    (
                        BINARY_CHECKSUM
                        (
                            p.nId_DocxPago,
                            p.nId_DocxCobrar,
                            p.nId_PersDeudor,
                            p.nId_MonPago,
                            p.nDoc_ImpPago,
                            p.dDoc_FecPago,
                            p.nId_Usuario,
                            p.nId_UsuarioCob
                        )
                    ),
                    0
                )
        FROM dbo.av_DocxPago AS p
        INNER JOIN #CarterasMafIntradia AS c
            ON c.nId_Cartera = p.nId_Cartera
        WHERE p.nId_Cliente = @IdCliente
          AND p.dDoc_FecPago < @FechaHasta;


        /* --------------------------------------------------------------------
           Fingerprint compacto SHA-256.
           Los campos detallados tambien se persisten para diagnostico.
           -------------------------------------------------------------------- */

        SET @FingerprintActual =
            HASHBYTES
            (
                'SHA2_256',
                CONCAT
                (
                    'C=', @CantidadCarteras,
                    '|D=', @CantidadDocumentos,
                    '|DEU=', @CantidadDeudores,
                    '|SAL=', CONVERT(VARCHAR(80), @SaldoDocumentos),
                    '|CD=', @ChecksumDocumentos,

                    '|G=', @CantidadGestiones,
                    '|GID=', ISNULL(CONVERT(VARCHAR(30), @IdMaxGestion), ''),
                    '|GF=', ISNULL(CONVERT(VARCHAR(33), @FechaMaxGestion, 126), ''),
                    '|CG=', @ChecksumGestiones,

                    '|P=', @CantidadPagos,
                    '|PID=', ISNULL(CONVERT(VARCHAR(30), @IdMaxPago), ''),
                    '|PF=', ISNULL(CONVERT(VARCHAR(33), @FechaMaxPago, 126), ''),
                    '|MP=', CONVERT(VARCHAR(80), @MontoPagos),
                    '|CP=', @ChecksumPagos
                )
            );


        /* --------------------------------------------------------------------
           Estado anterior.
           -------------------------------------------------------------------- */

        SELECT
            @EstadoExiste = 1,
            @FingerprintAnterior = e.fingerprint_origen,
            @FechaCorteAnterior = e.fecha_corte
        FROM integracion_datos_analitica.estado_intradia_maf AS e
        WHERE e.id_cliente = @IdCliente
          AND e.anio_campana = @Anio
          AND e.mes_campana = @Mes;


        SET @DebeProcesar =
            CONVERT
            (
                BIT,
                CASE
                    WHEN @Forzar = 1 THEN 1
                    WHEN @EstadoExiste = 0 THEN 1
                    WHEN @FechaCorteAnterior IS NULL THEN 1
                    WHEN @FechaCorteAnterior <> @FechaCorte THEN 1
                    WHEN @FingerprintAnterior IS NULL THEN 1
                    WHEN @FingerprintAnterior <> @FingerprintActual THEN 1
                    ELSE 0
                END
            );


        /* --------------------------------------------------------------------
           NO-OP como CLARO: poll sin nueva informacion.
           -------------------------------------------------------------------- */

        IF @DebeProcesar = 0
        BEGIN
            UPDATE integracion_datos_analitica.estado_intradia_maf
            SET
                fecha_ultima_comprobacion_utc = SYSUTCDATETIME(),
                ultimo_resultado = 'SIN_CAMBIOS',
                ultimo_error = NULL
            WHERE id_cliente = @IdCliente
              AND anio_campana = @Anio
              AND mes_campana = @Mes;

            EXEC sys.sp_releaseapplock
                @Resource = @LockResource,
                @LockOwner = 'Session';

            IF @MostrarResultado = 1
            BEGIN
                SELECT
                    'SIN_CAMBIOS' AS resultado,
                    @FechaCorte AS fecha_corte,
                    @FingerprintActual AS fingerprint_origen,
                    @CantidadCarteras AS carteras,
                    @CantidadDocumentos AS documentos,
                    @CantidadGestiones AS gestiones,
                    @CantidadPagos AS pagos,
                    SYSUTCDATETIME() AS comprobado_utc;
            END;

            RETURN 0;
        END;


        /* --------------------------------------------------------------------
           Cambio detectado: reconciliacion MTD existente.
           Se conserva el loader probado; solo cambia CUANDO se ejecuta.
           -------------------------------------------------------------------- */

        SET @InicioProcesoUtc = SYSUTCDATETIME();

        EXEC integracion_datos_analitica.usp_ejecutar_diario_maf
            @FechaCorte = @FechaCorte,
            @MostrarResultado = 0;

        SET @FinProcesoUtc = SYSUTCDATETIME();

        SET @DuracionMs =
            DATEDIFF_BIG
            (
                MILLISECOND,
                @InicioProcesoUtc,
                @FinProcesoUtc
            );


        /* --------------------------------------------------------------------
           Persistir el fingerprint observado ANTES del ETL.
           Si el origen cambia mientras se procesa, el siguiente poll detecta
           una diferencia y vuelve a reconciliar. No se pierde el cambio.
           -------------------------------------------------------------------- */

        UPDATE integracion_datos_analitica.estado_intradia_maf
        SET
            fecha_corte = @FechaCorte,
            fingerprint_origen = @FingerprintActual,

            cantidad_carteras = @CantidadCarteras,

            cantidad_documentos = @CantidadDocumentos,
            cantidad_deudores = @CantidadDeudores,
            saldo_documentos = @SaldoDocumentos,
            checksum_documentos = @ChecksumDocumentos,

            cantidad_gestiones = @CantidadGestiones,
            id_max_gestion = @IdMaxGestion,
            fecha_max_gestion = @FechaMaxGestion,
            checksum_gestiones = @ChecksumGestiones,

            cantidad_pagos = @CantidadPagos,
            id_max_pago = @IdMaxPago,
            fecha_max_pago = @FechaMaxPago,
            monto_pagos = @MontoPagos,
            checksum_pagos = @ChecksumPagos,

            fecha_ultima_comprobacion_utc = @FinProcesoUtc,
            fecha_ultimo_proceso_utc = @FinProcesoUtc,
            duracion_ultimo_proceso_ms = @DuracionMs,
            ultimo_resultado = 'PROCESADO',
            ultimo_error = NULL
        WHERE id_cliente = @IdCliente
          AND anio_campana = @Anio
          AND mes_campana = @Mes;

        IF @@ROWCOUNT = 0
        BEGIN
            INSERT INTO integracion_datos_analitica.estado_intradia_maf
            (
                id_cliente,
                anio_campana,
                mes_campana,
                fecha_corte,
                fingerprint_origen,

                cantidad_carteras,

                cantidad_documentos,
                cantidad_deudores,
                saldo_documentos,
                checksum_documentos,

                cantidad_gestiones,
                id_max_gestion,
                fecha_max_gestion,
                checksum_gestiones,

                cantidad_pagos,
                id_max_pago,
                fecha_max_pago,
                monto_pagos,
                checksum_pagos,

                fecha_ultima_comprobacion_utc,
                fecha_ultimo_proceso_utc,
                duracion_ultimo_proceso_ms,

                ultimo_resultado,
                ultimo_error
            )
            VALUES
            (
                @IdCliente,
                @Anio,
                @Mes,
                @FechaCorte,
                @FingerprintActual,

                @CantidadCarteras,

                @CantidadDocumentos,
                @CantidadDeudores,
                @SaldoDocumentos,
                @ChecksumDocumentos,

                @CantidadGestiones,
                @IdMaxGestion,
                @FechaMaxGestion,
                @ChecksumGestiones,

                @CantidadPagos,
                @IdMaxPago,
                @FechaMaxPago,
                @MontoPagos,
                @ChecksumPagos,

                @FinProcesoUtc,
                @FinProcesoUtc,
                @DuracionMs,

                'PROCESADO',
                NULL
            );
        END;


        EXEC sys.sp_releaseapplock
            @Resource = @LockResource,
            @LockOwner = 'Session';


        IF @MostrarResultado = 1
        BEGIN
            SELECT
                'PROCESADO' AS resultado,
                @FechaCorte AS fecha_corte,
                @Forzar AS forzado,
                @DuracionMs AS duracion_ms,
                @FingerprintActual AS fingerprint_origen,

                @CantidadCarteras AS carteras,

                @CantidadDocumentos AS documentos,
                @CantidadDeudores AS deudores,
                @SaldoDocumentos AS saldo_documentos,

                @CantidadGestiones AS gestiones,
                @IdMaxGestion AS id_max_gestion,
                @FechaMaxGestion AS fecha_max_gestion,

                @CantidadPagos AS pagos,
                @IdMaxPago AS id_max_pago,
                @FechaMaxPago AS fecha_max_pago,
                @MontoPagos AS monto_pagos,

                @FinProcesoUtc AS procesado_utc;
        END;

        RETURN 1;
    END TRY
    BEGIN CATCH
        DECLARE
            @ErrorMessage NVARCHAR(2000) = ERROR_MESSAGE(),
            @ErrorUtc DATETIME2(3) = SYSUTCDATETIME();

        /* No persistir fingerprint nuevo cuando el ETL falla. */
        IF EXISTS
        (
            SELECT 1
            FROM integracion_datos_analitica.estado_intradia_maf
            WHERE id_cliente = @IdCliente
              AND anio_campana = @Anio
              AND mes_campana = @Mes
        )
        BEGIN
            UPDATE integracion_datos_analitica.estado_intradia_maf
            SET
                fecha_ultima_comprobacion_utc = @ErrorUtc,
                ultimo_resultado = 'ERROR',
                ultimo_error = LEFT(@ErrorMessage, 2000)
            WHERE id_cliente = @IdCliente
              AND anio_campana = @Anio
              AND mes_campana = @Mes;
        END;

        EXEC sys.sp_releaseapplock
            @Resource = @LockResource,
            @LockOwner = 'Session';

        THROW;
    END CATCH;
END;
GO
