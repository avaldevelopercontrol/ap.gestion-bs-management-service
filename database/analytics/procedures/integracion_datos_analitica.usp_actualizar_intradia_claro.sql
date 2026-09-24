/*
===============================================================================
CLARO INTRADAY DIRECT - DEFINICION CANONICA VERSIONADA
Procedimiento: integracion_datos_analitica.usp_actualizar_intradia_claro

Objetivo:
- detectar cambios directamente en aval_cob, sin usar pbi_ciclo_ejecucion;
- refrescar las operaciones CLARO de ADMINISTRATIVO y GOBIERNO;
- conservar CLARO_INTRADAY_UPSTREAM como watermark de compatibilidad con API;
- usar una fecha logica PDP separada del instante intradia;
- no depender de MAX(nId_DocxCobrarOpe) como unica marca de cambio.

Nota:
El rendimiento de asesor/supervisor conserva su pipeline versionado separado.
Este procedimiento refresca el dominio operativo usado por resumen/contactos/
promesas/pagos y no cambia contratos de API.
===============================================================================
*/

USE [aval_cob];
GO

CREATE OR ALTER PROCEDURE
    integracion_datos_analitica.usp_actualizar_intradia_claro
(
    @id_cliente_crm INT = 95,
    @forzar BIT = 0,
    @mostrar_resultado BIT = 0
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @Ahora DATETIME2(3) = SYSDATETIME(),
        @AhoraUtc DATETIME2(3) = SYSUTCDATETIME(),
        @FechaCorte DATE = CONVERT(DATE, SYSDATETIME()),
        @FechaHasta DATETIME2(3),
        @FechaEstadoPdp DATE,
        @Anio SMALLINT,
        @Mes TINYINT,
        @ClaveCliente INT,
        @LockResult INT,
        @LockResource NVARCHAR(255) = CONCAT(N'analytics_claro_intraday_direct_', @id_cliente_crm),
        @CantidadCarteras INT = 0,
        @CantidadOperaciones BIGINT = 0,
        @CantidadPagos BIGINT = 0,
        @CantidadDocumentos BIGINT = 0,
        @IdMaxOperacion BIGINT = NULL,
        @IdMaxPago BIGINT = NULL,
        @FechaMaxOperacion DATETIME2(3) = NULL,
        @ChecksumOperaciones INT = 0,
        @ChecksumPagos INT = 0,
        @ChecksumDocumentos INT = 0,
        @Fingerprint BIGINT = NULL,
        @FingerprintAnterior BIGINT = NULL,
        @FechaOrigen DATETIME2(3) = NULL,
        @TieneAdministrativo BIT = 0,
        @TieneGobierno BIT = 0;

    IF @id_cliente_crm <> 95
        THROW 53500, 'Este procedimiento directo aplica solo a CLARO (95).', 1;

    SET @Anio = YEAR(@FechaCorte);
    SET @Mes = MONTH(@FechaCorte);
    SET @FechaHasta = DATEADD(DAY, 1, CONVERT(DATETIME2(3), @FechaCorte));

    SELECT @ClaveCliente = clave_cliente
    FROM analitica.dim_cliente
    WHERE id_cliente_crm = @id_cliente_crm
      AND es_activo = 1;

    IF @ClaveCliente IS NULL
        THROW 53501, 'CLARO no existe en analitica.dim_cliente.', 1;

    EXEC @LockResult = sys.sp_getapplock
        @Resource = @LockResource,
        @LockMode = 'Exclusive',
        @LockOwner = 'Session',
        @LockTimeout = 0;

    IF @LockResult < 0
    BEGIN
        IF @mostrar_resultado = 1
            SELECT 'OMITIDO_LOCK' AS resultado, @FechaCorte AS fecha_corte;
        RETURN 0;
    END;

    BEGIN TRY
        /* Asegura nuevas carteras sin pasar por PBI. */
        EXEC integracion_datos_analitica.usp_sincronizar_carteras_claro
            @FechaCorte = @FechaCorte,
            @MostrarResultado = 0;

        DROP TABLE IF EXISTS #CarterasClaroIntradia;

        SELECT
            ca.nId_Cartera,
            d.unidad_negocio_origen
        INTO #CarterasClaroIntradia
        FROM dbo.av_Cartera AS ca
        INNER JOIN analitica.dim_cartera AS d
            ON d.clave_cliente = @ClaveCliente
           AND d.id_cartera_origen = ca.nId_Cartera
           AND d.es_activo = 1
        WHERE ca.nId_Cliente = @id_cliente_crm
          AND ca.nId_Contrato = 182
          AND ca.nId_Grupo = 156
          AND ca.nAnioCar = @Anio
          AND ca.nCampCar = @Mes
          AND d.unidad_negocio_origen IN ('CLARO ADMINISTRATIVO', 'CLARO GOBIERNO');

        CREATE UNIQUE CLUSTERED INDEX CX_CarterasClaroIntradia
            ON #CarterasClaroIntradia(nId_Cartera);

        SELECT
            @CantidadCarteras = COUNT(*),
            @TieneAdministrativo = CONVERT(BIT, MAX(CASE WHEN unidad_negocio_origen = 'CLARO ADMINISTRATIVO' THEN 1 ELSE 0 END)),
            @TieneGobierno = CONVERT(BIT, MAX(CASE WHEN unidad_negocio_origen = 'CLARO GOBIERNO' THEN 1 ELSE 0 END))
        FROM #CarterasClaroIntradia;

        IF @CantidadCarteras = 0
            THROW 53502, 'No existen carteras CLARO directas para el mes vigente.', 1;

        /*
           Fingerprint de operaciones.
           Incluye campos que cambian contacto, promesa, deduplicacion y fecha.
        */
        SELECT
            @CantidadOperaciones = COUNT_BIG(*),
            @IdMaxOperacion = MAX(CONVERT(BIGINT, o.nId_DocxCobrarOpe)),
            @FechaMaxOperacion = MAX(CONVERT(DATETIME2(3), COALESCE(o.dDoc_FecIngresoGes, o.dDocCobOpe_FecIni))),
            @ChecksumOperaciones = ISNULL(CHECKSUM_AGG(BINARY_CHECKSUM(
                o.nId_DocxCobrarOpe,
                o.nId_DocxCobrar,
                o.nId_PersDeudor,
                o.nId_Cartera,
                o.nId_UsuOpe,
                o.tip_gestion,
                o.dDocCobOpe_FecIni,
                o.dDocCobOpe_FecFin,
                o.dDoc_FecIngresoGes,
                o.monto_comp,
                o.monto_compDolares,
                o.dFechCompromisoPago,
                o.nId_GestionDisp,
                o.nId_OpeCodOut,
                o.nId_OpeCodOutNp2,
                o.nTelef_Nro,
                o.cDocOpeCobOut_Descr
            )), 0)
        FROM dbo.av_DocxCobrarOpe AS o
        INNER JOIN #CarterasClaroIntradia AS c
            ON c.nId_Cartera = o.nId_Cartera
        WHERE o.nId_Cliente = @id_cliente_crm
          AND o.dDocCobOpe_FecIni < @FechaHasta;

        SELECT
            @CantidadPagos = COUNT_BIG(*),
            @IdMaxPago = MAX(CONVERT(BIGINT, p.nId_DocxPago)),
            @ChecksumPagos = ISNULL(CHECKSUM_AGG(BINARY_CHECKSUM(
                p.nId_DocxPago,
                p.nId_DocxCobrar,
                p.nId_PersDeudor,
                p.nId_Cartera,
                p.dDoc_FecPago,
                p.nDoc_ImpPago,
                p.nDoc_ImpParam01
            )), 0)
        FROM dbo.av_DocxPago AS p
        INNER JOIN #CarterasClaroIntradia AS c
            ON c.nId_Cartera = p.nId_Cartera
        WHERE p.nId_Cliente = @id_cliente_crm
          AND p.dDoc_FecPago < @FechaHasta;

        SELECT
            @CantidadDocumentos = COUNT_BIG(*),
            @ChecksumDocumentos = ISNULL(CHECKSUM_AGG(BINARY_CHECKSUM(
                dc.nId_DocxCobrar,
                dc.nId_PersDeudor,
                dc.nId_Cartera,
                dc.nDoc_ImpTotal,
                dc.nDoc_ImpSaldo,
                dp.cDocParam33
            )), 0)
        FROM dbo.av_DocxCobrar AS dc
        INNER JOIN #CarterasClaroIntradia AS c
            ON c.nId_Cartera = dc.nId_Cartera
        INNER JOIN
        (
            SELECT
                p.nId_Cartera,
                p.nId_DocxCobrar,
                MAX(CONVERT(VARCHAR(500), p.cDocParam33)) AS cDocParam33
            FROM dbo.av_DocxCobrarParam AS p
            INNER JOIN #CarterasClaroIntradia AS c2
                ON c2.nId_Cartera = p.nId_Cartera
            WHERE p.nId_Cliente = @id_cliente_crm
            GROUP BY p.nId_Cartera, p.nId_DocxCobrar
        ) AS dp
            ON dp.nId_Cartera = dc.nId_Cartera
           AND dp.nId_DocxCobrar = dc.nId_DocxCobrar
        WHERE dc.nId_Cliente = @id_cliente_crm;

        SET @Fingerprint = CONVERT(
            BIGINT,
            SUBSTRING(
                HASHBYTES(
                    'SHA2_256',
                    CONCAT(
                        @Anio, '|', @Mes, '|',
                        @CantidadCarteras, '|',
                        @CantidadOperaciones, '|', ISNULL(@IdMaxOperacion, 0), '|', ISNULL(@ChecksumOperaciones, 0), '|',
                        @CantidadPagos, '|', ISNULL(@IdMaxPago, 0), '|', ISNULL(@ChecksumPagos, 0), '|',
                        @CantidadDocumentos, '|', ISNULL(@ChecksumDocumentos, 0)
                    )
                ),
                1,
                8
            )
        );

        SELECT @FingerprintAnterior = id_ultimo_origen
        FROM integracion_datos_analitica.control_carga
        WHERE codigo_origen = 'CLARO_INTRADAY_UPSTREAM';

        IF @forzar = 0
           AND @FingerprintAnterior IS NOT NULL
           AND @FingerprintAnterior = @Fingerprint
        BEGIN
            EXEC sys.sp_releaseapplock
                @Resource = @LockResource,
                @LockOwner = 'Session';

            IF @mostrar_resultado = 1
                SELECT
                    'SIN_CAMBIOS' AS resultado,
                    @FechaCorte AS fecha_corte,
                    @Fingerprint AS fingerprint;

            RETURN 0;
        END;

        /*
           Fecha logica PDP = ultimo D-1 diario materializado para CLARO.
           Si aun no existe, usa D-1; el primer dia del mes no retrocede fuera
           del periodo vigente.
        */
        SELECT @FechaEstadoPdp = MIN(CONVERT(DATE, fecha_hora_ultimo_origen))
        FROM integracion_datos_analitica.control_carga
        WHERE codigo_origen IN ('CLARO_DAILY_ADMINISTRATIVO', 'CLARO_DAILY_GOBIERNO')
          AND fecha_hora_ultimo_origen IS NOT NULL;

        IF @FechaEstadoPdp IS NULL
            SET @FechaEstadoPdp = DATEADD(DAY, -1, @FechaCorte);

        IF @FechaEstadoPdp < DATEFROMPARTS(@Anio, @Mes, 1)
            SET @FechaEstadoPdp = DATEFROMPARTS(@Anio, @Mes, 1);

        IF @FechaEstadoPdp > @FechaCorte
            SET @FechaEstadoPdp = @FechaCorte;

        BEGIN TRANSACTION;

        IF @TieneAdministrativo = 1
        BEGIN
            EXEC integracion_datos_analitica.usp_cargar_operaciones_actuales_claro
                @id_cliente_crm = @id_cliente_crm,
                @fecha_corte = @Ahora,
                @anio_campana = @Anio,
                @mes_campana = @Mes,
                @codigo_unidad_negocio = 'ADMINISTRATIVO',
                @fecha_estado_pdp = @FechaEstadoPdp;
        END;

        IF @TieneGobierno = 1
        BEGIN
            EXEC integracion_datos_analitica.usp_cargar_operaciones_actuales_claro
                @id_cliente_crm = @id_cliente_crm,
                @fecha_corte = @Ahora,
                @anio_campana = @Anio,
                @mes_campana = @Mes,
                @codigo_unidad_negocio = 'GOBIERNO',
                @fecha_estado_pdp = @FechaEstadoPdp;
        END;

        SET @FechaOrigen = COALESCE(@FechaMaxOperacion, @Ahora);

        UPDATE integracion_datos_analitica.control_carga
        SET
            fecha_ultimo_exito = @AhoraUtc,
            fecha_hora_ultimo_origen = @FechaOrigen,
            id_ultimo_origen = @Fingerprint,
            dias_solapamiento = 1,
            fecha_actualizacion = @AhoraUtc
        WHERE codigo_origen = 'CLARO_INTRADAY_UPSTREAM';

        IF @@ROWCOUNT = 0
        BEGIN
            INSERT INTO integracion_datos_analitica.control_carga
            (
                codigo_origen,
                fecha_ultimo_exito,
                fecha_hora_ultimo_origen,
                id_ultimo_origen,
                dias_solapamiento
            )
            VALUES
            (
                'CLARO_INTRADAY_UPSTREAM',
                @AhoraUtc,
                @FechaOrigen,
                @Fingerprint,
                1
            );
        END;

        COMMIT TRANSACTION;

        EXEC sys.sp_releaseapplock
            @Resource = @LockResource,
            @LockOwner = 'Session';

        IF @mostrar_resultado = 1
        BEGIN
            SELECT
                'PROCESADO' AS resultado,
                @FechaCorte AS fecha_corte,
                @FechaEstadoPdp AS fecha_estado_pdp,
                @CantidadCarteras AS carteras,
                @CantidadOperaciones AS operaciones_origen,
                @CantidadPagos AS pagos_origen,
                @CantidadDocumentos AS documentos_origen,
                @Fingerprint AS fingerprint,
                @FechaOrigen AS fecha_hora_ultimo_origen;
        END;

        RETURN 1;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;

        EXEC sys.sp_releaseapplock
            @Resource = @LockResource,
            @LockOwner = 'Session';

        THROW;
    END CATCH;
END;
GO
