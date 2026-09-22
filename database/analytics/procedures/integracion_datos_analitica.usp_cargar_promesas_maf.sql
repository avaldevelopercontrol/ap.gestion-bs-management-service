USE [aval_cob];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

CREATE OR ALTER PROCEDURE integracion_datos_analitica.usp_cargar_promesas_maf
    @FechaDesde DATE,
    @FechaCorte DATE,
    @Aplicar BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @IdCliente INT = 59,
        @IdContrato INT = 246,
        @IdGrupo INT = 194,
        @ClaveCliente INT,
        @FechaHasta DATETIME2(3);

    IF @FechaDesde IS NULL
       OR @FechaCorte IS NULL
       OR @FechaDesde > @FechaCorte
    BEGIN
        THROW 52100,
              'Rango de fechas invalido para promesas MAF.',
              1;
    END;

    SET @FechaHasta =
        DATEADD(DAY, 1, CONVERT(DATETIME2(3), @FechaCorte));

    SELECT
        @ClaveCliente = clave_cliente
    FROM analitica.dim_cliente
    WHERE id_cliente_crm = @IdCliente;

    IF @ClaveCliente IS NULL
    BEGIN
        THROW 52101,
              'No existe MAF (cliente CRM 59) en analitica.dim_cliente.',
              1;
    END;


    /* =====================================================================
       1. CARTERAS / CAMPAÑAS MAF
       ===================================================================== */

    IF OBJECT_ID('tempdb..#Carteras') IS NOT NULL
        DROP TABLE #Carteras;

    SELECT
        ca.nId_Cartera,
        ca.cCar_Nombre,
        ca.cCiclo,
        ca.nAnioCar,
        ca.nCampCar,

        DATEFROMPARTS(ca.nAnioCar, ca.nCampCar, 1)
            AS fecha_inicio_campana,

        DATEADD
        (
            MONTH,
            1,
            DATEFROMPARTS(ca.nAnioCar, ca.nCampCar, 1)
        ) AS fecha_fin_exclusiva_campana,

        dc.clave_cartera,
        dca.clave_campana

    INTO #Carteras

    FROM dbo.av_Cartera ca

    INNER JOIN analitica.dim_cartera dc
        ON dc.clave_cliente = @ClaveCliente
       AND dc.id_cartera_origen = ca.nId_Cartera

    INNER JOIN analitica.dim_campana dca
        ON dca.clave_cliente = @ClaveCliente
       AND dca.anio_campana = ca.nAnioCar
       AND dca.mes_campana = ca.nCampCar

    WHERE ca.nId_Cliente = @IdCliente
      AND ca.nId_Contrato = @IdContrato
      AND ca.nId_Grupo = @IdGrupo
      AND ca.cCiclo IN
          (
              'CICLO 04',
              'CICLO 11',
              'CICLO 18',
              'CICLO 25'
          )
      AND UPPER(ISNULL(ca.cCar_Nombre,'')) NOT LIKE '%BORRADOR%'
      AND DATEFROMPARTS(ca.nAnioCar, ca.nCampCar, 1)
          BETWEEN DATEFROMPARTS(YEAR(@FechaDesde), MONTH(@FechaDesde), 1)
              AND DATEFROMPARTS(YEAR(@FechaCorte), MONTH(@FechaCorte), 1);

    IF NOT EXISTS (SELECT 1 FROM #Carteras)
    BEGIN
        THROW 52102,
              'No se encontraron carteras MAF para el rango solicitado.',
              1;
    END;


    /* =====================================================================
       2. TIPO DE CAMBIO POR CARTERA

       Prioridad:
         1) fallback explicito historico validado;
         2) mediana cDocParam76 numerico valido de documentos USD;
         3) mediana nDoc_ImpTotal / cDocParam108 en documentos PEN.
       ===================================================================== */

    IF OBJECT_ID('tempdb..#Tc76') IS NOT NULL
        DROP TABLE #Tc76;

    ;WITH Base AS
    (
        SELECT
            c.nId_Cartera,
            TRY_CONVERT
            (
                DECIMAL(19,6),
                NULLIF
                (
                    REPLACE(LTRIM(RTRIM(dp.cDocParam76)), ',', '.'),
                    ''
                )
            ) AS tc
        FROM #Carteras c
        INNER JOIN dbo.av_DocxCobrar dc
            ON dc.nId_Cliente = @IdCliente
           AND dc.nId_Cartera = c.nId_Cartera
        INNER JOIN dbo.av_DocxCobrarParam dp
            ON dp.nId_Cliente = dc.nId_Cliente
           AND dp.nId_Cartera = dc.nId_Cartera
           AND dp.nId_DocxCobrar = dc.nId_DocxCobrar
        WHERE dc.nId_Moneda = 2
          AND ISNULL(dc.nId_DocxCobrarEst,1) <> 8
          AND UPPER(LTRIM(RTRIM(ISNULL(dc.cCampo4,'')))) <> 'RETIRADO'
    ),
    Valido AS
    (
        SELECT
            nId_Cartera,
            tc
        FROM Base
        WHERE tc BETWEEN 2.5 AND 5.0
    )
    SELECT DISTINCT
        nId_Cartera,
        CONVERT
        (
            DECIMAL(19,6),
            PERCENTILE_CONT(0.5)
                WITHIN GROUP (ORDER BY tc)
                OVER (PARTITION BY nId_Cartera)
        ) AS tc_mediana_76
    INTO #Tc76
    FROM Valido;


    IF OBJECT_ID('tempdb..#TcPen') IS NOT NULL
        DROP TABLE #TcPen;

    ;WITH Base AS
    (
        SELECT
            c.nId_Cartera,
            CONVERT
            (
                DECIMAL(19,6),
                dc.nDoc_ImpTotal
                /
                NULLIF
                (
                    TRY_CONVERT
                    (
                        DECIMAL(19,6),
                        NULLIF
                        (
                            REPLACE(LTRIM(RTRIM(dp.cDocParam108)), ',', '.'),
                            ''
                        )
                    ),
                    0
                )
            ) AS tc
        FROM #Carteras c
        INNER JOIN dbo.av_DocxCobrar dc
            ON dc.nId_Cliente = @IdCliente
           AND dc.nId_Cartera = c.nId_Cartera
        INNER JOIN dbo.av_DocxCobrarParam dp
            ON dp.nId_Cliente = dc.nId_Cliente
           AND dp.nId_Cartera = dc.nId_Cartera
           AND dp.nId_DocxCobrar = dc.nId_DocxCobrar
        WHERE dc.nId_Moneda = 1
          AND ISNULL(dc.nId_DocxCobrarEst,1) <> 8
          AND UPPER(LTRIM(RTRIM(ISNULL(dc.cCampo4,'')))) <> 'RETIRADO'
          AND TRY_CONVERT
              (
                  DECIMAL(19,6),
                  NULLIF
                  (
                      REPLACE(LTRIM(RTRIM(dp.cDocParam108)), ',', '.'),
                      ''
                  )
              ) > 0
    ),
    Valido AS
    (
        SELECT
            nId_Cartera,
            tc
        FROM Base
        WHERE tc BETWEEN 2.5 AND 5.0
    )
    SELECT DISTINCT
        nId_Cartera,
        CONVERT
        (
            DECIMAL(19,6),
            PERCENTILE_CONT(0.5)
                WITHIN GROUP (ORDER BY tc)
                OVER (PARTITION BY nId_Cartera)
        ) AS tc_mediana_pen
    INTO #TcPen
    FROM Valido;


    IF OBJECT_ID('tempdb..#TipoCambioCartera') IS NOT NULL
        DROP TABLE #TipoCambioCartera;

    SELECT
        c.nId_Cartera,

        CONVERT
        (
            DECIMAL(19,6),
            COALESCE
            (
                fb.tipo_cambio,
                t76.tc_mediana_76,
                tp.tc_mediana_pen
            )
        ) AS tipo_cambio_cartera,

        CASE
            WHEN fb.tipo_cambio IS NOT NULL
                THEN 'FALLBACK_EXPLICITO'
            WHEN t76.tc_mediana_76 IS NOT NULL
                THEN 'MEDIANA_TC76'
            WHEN tp.tc_mediana_pen IS NOT NULL
                THEN 'MEDIANA_PEN'
            ELSE 'SIN_TC'
        END AS origen_tipo_cambio

    INTO #TipoCambioCartera

    FROM #Carteras c

    LEFT JOIN integracion_datos_analitica.tipo_cambio_fallback_maf fb
        ON fb.nId_Cartera = c.nId_Cartera

    LEFT JOIN #Tc76 t76
        ON t76.nId_Cartera = c.nId_Cartera

    LEFT JOIN #TcPen tp
        ON tp.nId_Cartera = c.nId_Cartera;


    /* =====================================================================
       3. PROMESAS FUENTE

       Importante:
         - Promesa semantica = nId_TipoContacto 2.
         - Solo CALL + WhatsApp para alinear hecho_promesa con
           cantidad_promesas_dia del ETL MAF.
         - Requiere que el deudor tenga al menos un documento elegible en
           la cartera al momento de la gestion.
       ===================================================================== */

    IF OBJECT_ID('tempdb..#PromesaFuente') IS NOT NULL
        DROP TABLE #PromesaFuente;

    ;WITH Fuente AS
    (
        SELECT
            c.clave_campana,
            c.clave_cartera,
            c.nId_Cartera,
            c.fecha_inicio_campana,
            c.fecha_fin_exclusiva_campana,

            CONVERT(BIGINT, o.nId_DocxCobrarOpe)
                AS id_operacion_origen,

            CONVERT(BIGINT, o.nId_PersDeudor)
                AS id_deudor_origen,

            o.nId_DocxCobrar,
            o.nId_UsuOpe,

            CONVERT
            (
                DATETIME2(3),
                COALESCE
                (
                    /*
                       Fecha canónica MAF para Analítica:
                       priorizar dDoc_FecIngresoGes, igual que el ETL diario
                       y el filtro temporal del reporte MAF vigente.
                    */
                    o.dDoc_FecIngresoGes,
                    o.dDocCobOpe_FecIni
                )
            ) AS fecha_hora_gestion,

            CONVERT(DATE, o.dFechCompromisoPago)
                AS fecha_vencimiento_promesa,

            CONVERT(DECIMAL(19,4), ISNULL(o.monto_comp,0))
                AS monto_comp,

            CONVERT(DECIMAL(19,4), ISNULL(o.monto_compDolares,0))
                AS monto_comp_dolares,

            dc_op.nId_Moneda
                AS moneda_documento,

            CONVERT(VARCHAR(100), cod.cNombre_OpeCodCliOut)
                AS estado_origen,

            TRY_CONVERT
            (
                DECIMAL(19,6),
                NULLIF
                (
                    REPLACE(LTRIM(RTRIM(dp_op.cDocParam76)), ',', '.'),
                    ''
                )
            ) AS tc_documento_raw,

            tcc.tipo_cambio_cartera,

            ROW_NUMBER() OVER
            (
                PARTITION BY o.nId_DocxCobrarOpe
                ORDER BY
                    COALESCE
                    (
                        o.dDoc_FecIngresoGes,
                        o.dDocCobOpe_FecIni
                    ) DESC,
                    o.nId_DocxCobrarOpe DESC
            ) AS rn

        FROM #Carteras c

        INNER JOIN dbo.av_DocxCobrarOpe o
            ON o.nId_Cliente = @IdCliente
           AND o.nId_Cartera = c.nId_Cartera

        INNER JOIN dbo.av_Usuario u
            ON u.nId_Usuario = o.nId_UsuOpe
           AND u.nId_PerfilGest IS NULL

        INNER JOIN dbo.av_OpeCodCliOut cod
            ON cod.nId_Cliente = o.nId_Cliente
           AND cod.nId_OpeCodCliOut = o.nId_OpeCodOut
           AND cod.nId_TipoContacto = 2

        LEFT JOIN dbo.av_DocxCobrar dc_op
            ON dc_op.nId_Cliente = o.nId_Cliente
           AND dc_op.nId_Cartera = o.nId_Cartera
           AND dc_op.nId_DocxCobrar = o.nId_DocxCobrar

        LEFT JOIN dbo.av_DocxCobrarParam dp_op
            ON dp_op.nId_Cliente = dc_op.nId_Cliente
           AND dp_op.nId_Cartera = dc_op.nId_Cartera
           AND dp_op.nId_DocxCobrar = dc_op.nId_DocxCobrar

        LEFT JOIN #TipoCambioCartera tcc
            ON tcc.nId_Cartera = c.nId_Cartera

        WHERE o.tip_gestion IN (1,4)

          AND COALESCE
              (
                  o.dDoc_FecIngresoGes,
                  o.dDocCobOpe_FecIni
              ) >= @FechaDesde

          AND COALESCE
              (
                  o.dDoc_FecIngresoGes,
                  o.dDocCobOpe_FecIni
              ) < @FechaHasta

          /*
             Contrato mensual de Analítica MAF:
             el procedimiento diario solo procesa una cartera cuando
             nAnioCar/nCampCar coinciden con el mes de @FechaCorte.
             Por tanto, una gestión de una cartera de junio registrada
             en julio NO pertenece al hecho analítico de la campaña junio.
          */
          AND COALESCE
              (
                  o.dDoc_FecIngresoGes,
                  o.dDocCobOpe_FecIni
              ) >= c.fecha_inicio_campana

          AND COALESCE
              (
                  o.dDoc_FecIngresoGes,
                  o.dDocCobOpe_FecIni
              ) < c.fecha_fin_exclusiva_campana

          AND
          (
              /*
                 Para una promesa nueva, el deudor debe pertenecer al universo
                 elegible del ETL diario MAF en la fecha de la gestion.

                 Si la promesa ya fue admitida previamente por Analitica, se
                 conserva aunque posteriormente todos sus documentos hayan
                 quedado retirados/cerrados. Esto mantiene el evento historico
                 y permite seguir reconciliando pagos y estado.
              */
              EXISTS
              (
                  SELECT 1

                  FROM dbo.av_DocxCobrar dx

                  INNER JOIN dbo.av_DocxCobrarParam dpx
                      ON dpx.nId_Cliente = dx.nId_Cliente
                     AND dpx.nId_Cartera = dx.nId_Cartera
                     AND dpx.nId_DocxCobrar = dx.nId_DocxCobrar

                  WHERE dx.nId_Cliente = @IdCliente
                    AND dx.nId_Cartera = o.nId_Cartera
                    AND dx.nId_PersDeudor = o.nId_PersDeudor

                    AND ISNULL(dx.nId_DocxCobrarEst,1) <> 8

                    AND UPPER
                        (
                            LTRIM
                            (
                                RTRIM
                                (
                                    ISNULL(dx.cCampo4,'')
                                )
                            )
                        ) <> 'RETIRADO'

                    AND dx.dDoc_FecIngreso
                        <
                        DATEADD
                        (
                            DAY,
                            1,
                            CONVERT
                            (
                                DATE,
                                COALESCE
                                (
                                    o.dDoc_FecIngresoGes,
                                    o.dDocCobOpe_FecIni
                                )
                            )
                        )
              )

              OR

              EXISTS
              (
                  SELECT 1
                  FROM analitica.hecho_promesa hp
                  WHERE hp.clave_cliente = @ClaveCliente
                    AND hp.id_operacion_origen =
                        CONVERT(BIGINT, o.nId_DocxCobrarOpe)
              )
          )
    )
    SELECT
        clave_campana,
        clave_cartera,
        nId_Cartera,
        fecha_inicio_campana,
        fecha_fin_exclusiva_campana,
        id_operacion_origen,
        id_deudor_origen,
        nId_DocxCobrar,
        nId_UsuOpe,
        fecha_hora_gestion,
        fecha_vencimiento_promesa,
        monto_comp,
        monto_comp_dolares,
        moneda_documento,
        estado_origen,

        CONVERT
        (
            DECIMAL(19,6),
            CASE
                WHEN tc_documento_raw BETWEEN 2.5 AND 5.0
                    THEN tc_documento_raw
                ELSE tipo_cambio_cartera
            END
        ) AS tipo_cambio,

        CONVERT
        (
            DECIMAL(19,4),
            CASE
                /*
                   Regla MAF:
                   - La moneda del documento es autoritativa cuando existe.
                   - Documento USD: usa monto_compDolares * TC si el campo USD
                     tiene valor.
                   - Documento PEN: usa monto_comp.
                   - Si la operación no conserva documento, solo se infiere
                     moneda cuando exactamente uno de los dos importes tiene
                     valor.
                   - Si no hay documento y ambos importes tienen valor, queda
                     NULL y el preflight detiene la carga.
                */
                WHEN moneda_documento = 2
                 AND monto_comp_dolares <> 0
                    THEN monto_comp_dolares
                         *
                         CASE
                             WHEN tc_documento_raw BETWEEN 2.5 AND 5.0
                                 THEN tc_documento_raw
                             ELSE tipo_cambio_cartera
                         END

                WHEN moneda_documento = 1
                    THEN monto_comp

                /*
                   Compatibilidad con el ETL diario original:
                   documento USD sin monto_compDolares -> monto_comp.
                */
                WHEN moneda_documento = 2
                 AND monto_comp_dolares = 0
                    THEN monto_comp

                WHEN moneda_documento IS NULL
                 AND monto_comp <> 0
                 AND monto_comp_dolares = 0
                    THEN monto_comp

                WHEN moneda_documento IS NULL
                 AND monto_comp = 0
                 AND monto_comp_dolares <> 0
                    THEN monto_comp_dolares
                         *
                         CASE
                             WHEN tc_documento_raw BETWEEN 2.5 AND 5.0
                                 THEN tc_documento_raw
                             ELSE tipo_cambio_cartera
                         END

                WHEN moneda_documento IS NULL
                 AND monto_comp = 0
                 AND monto_comp_dolares = 0
                    THEN CONVERT(DECIMAL(19,4),0)

                ELSE NULL
            END
        ) AS monto_promesa_pen

    INTO #PromesaFuente

    FROM Fuente
    WHERE rn = 1;


    /* =====================================================================
       3A. TIPO DE CAMBIO A LA FECHA DE CADA PROMESA

       La V5 calculaba un TC estático por cartera usando todo el rango.
       El ETL diario, en cambio, resuelve el TC con documentos disponibles
       hasta el cierre de CADA día.

       Aquí se replica esa temporalidad para:
         A) monto_promesa_pen (regla detalle, moneda documento autoritativa)
         B) monto_promesa_pen_legacy (réplica exacta del agregado diario
            vigente: si monto_compDolares <> 0, convertirlo a PEN).

       Esto permite validar el histórico sin copiar silenciosamente la regla
       legacy al hecho detallado.
       ===================================================================== */

    IF OBJECT_ID('tempdb..#CarteraFechaPromesa') IS NOT NULL
        DROP TABLE #CarteraFechaPromesa;

    SELECT DISTINCT
        nId_Cartera,
        CONVERT(DATE, fecha_hora_gestion) AS fecha_gestion
    INTO #CarteraFechaPromesa
    FROM #PromesaFuente;


    IF OBJECT_ID('tempdb..#DocumentoTcPromesaDia') IS NOT NULL
        DROP TABLE #DocumentoTcPromesaDia;

    SELECT
        cf.nId_Cartera,
        cf.fecha_gestion,

        dc.nId_DocxCobrar,
        dc.nId_Moneda,

        TRY_CONVERT
        (
            DECIMAL(19,6),
            NULLIF
            (
                REPLACE(LTRIM(RTRIM(dp.cDocParam76)), ',', '.'),
                ''
            )
        ) AS tc76_raw,

        CONVERT
        (
            DECIMAL(19,6),
            CASE
                WHEN dc.nId_Moneda = 2
                 AND TRY_CONVERT
                     (
                         DECIMAL(19,6),
                         NULLIF
                         (
                             REPLACE(LTRIM(RTRIM(dp.cDocParam76)), ',', '.'),
                             ''
                         )
                     ) BETWEEN 2.5 AND 5.0
                    THEN TRY_CONVERT
                         (
                             DECIMAL(19,6),
                             NULLIF
                             (
                                 REPLACE(LTRIM(RTRIM(dp.cDocParam76)), ',', '.'),
                                 ''
                             )
                         )

                WHEN dc.nId_Moneda = 2
                 AND fb.tipo_cambio BETWEEN 2.5 AND 5.0
                    THEN fb.tipo_cambio

                ELSE NULL
            END
        ) AS tc_usd_efectivo,

        CONVERT
        (
            DECIMAL(19,6),
            CASE
                WHEN dc.nId_Moneda = 1
                 AND TRY_CONVERT
                     (
                         DECIMAL(19,6),
                         NULLIF
                         (
                             REPLACE(LTRIM(RTRIM(dp.cDocParam108)), ',', '.'),
                             ''
                         )
                     ) > 0
                THEN
                    dc.nDoc_ImpTotal
                    /
                    NULLIF
                    (
                        TRY_CONVERT
                        (
                            DECIMAL(19,6),
                            NULLIF
                            (
                                REPLACE(LTRIM(RTRIM(dp.cDocParam108)), ',', '.'),
                                ''
                            )
                        ),
                        0
                    )
                ELSE NULL
            END
        ) AS tc_pen_derivado

    INTO #DocumentoTcPromesaDia

    FROM #CarteraFechaPromesa cf

    INNER JOIN dbo.av_DocxCobrar dc
        ON dc.nId_Cliente = @IdCliente
       AND dc.nId_Cartera = cf.nId_Cartera

    INNER JOIN dbo.av_DocxCobrarParam dp
        ON dp.nId_Cliente = dc.nId_Cliente
       AND dp.nId_Cartera = dc.nId_Cartera
       AND dp.nId_DocxCobrar = dc.nId_DocxCobrar

    LEFT JOIN integracion_datos_analitica.tipo_cambio_fallback_maf fb
        ON fb.nId_Cartera = dc.nId_Cartera

    WHERE ISNULL(dc.nId_DocxCobrarEst,1) <> 8
      AND UPPER(LTRIM(RTRIM(ISNULL(dc.cCampo4,'')))) <> 'RETIRADO'
      AND dc.dDoc_FecIngreso
          <
          DATEADD
          (
              DAY,
              1,
              CONVERT(DATETIME2(3), cf.fecha_gestion)
          );


    IF OBJECT_ID('tempdb..#TipoCambioCarteraDia') IS NOT NULL
        DROP TABLE #TipoCambioCarteraDia;

    ;WITH TcUsd AS
    (
        SELECT DISTINCT
            nId_Cartera,
            fecha_gestion,

            CONVERT
            (
                DECIMAL(19,6),
                PERCENTILE_CONT(0.5)
                WITHIN GROUP (ORDER BY tc_usd_efectivo)
                OVER
                (
                    PARTITION BY
                        nId_Cartera,
                        fecha_gestion
                )
            ) AS tc_usd

        FROM #DocumentoTcPromesaDia

        WHERE nId_Moneda = 2
          AND tc_usd_efectivo BETWEEN 2.5 AND 5.0
    ),
    TcPen AS
    (
        SELECT DISTINCT
            nId_Cartera,
            fecha_gestion,

            CONVERT
            (
                DECIMAL(19,6),
                PERCENTILE_CONT(0.5)
                WITHIN GROUP (ORDER BY tc_pen_derivado)
                OVER
                (
                    PARTITION BY
                        nId_Cartera,
                        fecha_gestion
                )
            ) AS tc_pen

        FROM #DocumentoTcPromesaDia

        WHERE nId_Moneda = 1
          AND tc_pen_derivado BETWEEN 2.5 AND 5.0
    )
    SELECT
        cf.nId_Cartera,
        cf.fecha_gestion,

        CONVERT
        (
            DECIMAL(19,6),
            COALESCE
            (
                u.tc_usd,

                CASE
                    WHEN fb.tipo_cambio BETWEEN 2.5 AND 5.0
                        THEN fb.tipo_cambio
                END,

                p.tc_pen
            )
        ) AS tipo_cambio_cartera_dia

    INTO #TipoCambioCarteraDia

    FROM #CarteraFechaPromesa cf

    LEFT JOIN TcUsd u
        ON u.nId_Cartera = cf.nId_Cartera
       AND u.fecha_gestion = cf.fecha_gestion

    LEFT JOIN integracion_datos_analitica.tipo_cambio_fallback_maf fb
        ON fb.nId_Cartera = cf.nId_Cartera

    LEFT JOIN TcPen p
        ON p.nId_Cartera = cf.nId_Cartera
       AND p.fecha_gestion = cf.fecha_gestion;


    ALTER TABLE #PromesaFuente
        ADD monto_promesa_pen_legacy DECIMAL(19,4) NULL;


    /*
       Recalcular el TC efectivo de la operación con la misma temporalidad
       del corte diario.

       tc_documento_dia:
         - solo existe si el documento puntual formaba parte de
           #BaseDocumento en ese día;
         - TC76 válido;
         - fallback explícito si TC76 no es válido.

       Si el documento puntual no estaba en #BaseDocumento, usa
       #TipoCambioCarteraDia, igual que el hotfix diario.
    */
    UPDATE pf
    SET
        pf.tipo_cambio =
            tc.tc_efectivo_dia,

        pf.monto_promesa_pen =
            CONVERT
            (
                DECIMAL(19,4),
                CASE
                    /* Regla detallada: moneda del documento autoritativa. */
                    WHEN pf.moneda_documento = 2
                     AND pf.monto_comp_dolares <> 0
                        THEN pf.monto_comp_dolares
                             * tc.tc_efectivo_dia

                    WHEN pf.moneda_documento = 1
                        THEN pf.monto_comp

                    WHEN pf.moneda_documento = 2
                     AND pf.monto_comp_dolares = 0
                        THEN pf.monto_comp

                    WHEN pf.moneda_documento IS NULL
                     AND pf.monto_comp <> 0
                     AND pf.monto_comp_dolares = 0
                        THEN pf.monto_comp

                    WHEN pf.moneda_documento IS NULL
                     AND pf.monto_comp = 0
                     AND pf.monto_comp_dolares <> 0
                        THEN pf.monto_comp_dolares
                             * tc.tc_efectivo_dia

                    WHEN pf.moneda_documento IS NULL
                     AND pf.monto_comp = 0
                     AND pf.monto_comp_dolares = 0
                        THEN 0

                    ELSE NULL
                END
            ),

        pf.monto_promesa_pen_legacy =
            CONVERT
            (
                DECIMAL(19,4),
                CASE
                    /*
                       Réplica EXACTA de la regla monetaria que usa hoy
                       usp_cargar_corte_diario_cartera_maf.
                    */
                    WHEN pf.monto_comp_dolares <> 0
                        THEN pf.monto_comp_dolares
                             * tc.tc_efectivo_dia
                    ELSE pf.monto_comp
                END
            )

    FROM #PromesaFuente pf

    LEFT JOIN dbo.av_DocxCobrar dc_op
        ON dc_op.nId_Cliente = @IdCliente
       AND dc_op.nId_Cartera = pf.nId_Cartera
       AND dc_op.nId_DocxCobrar = pf.nId_DocxCobrar

    LEFT JOIN dbo.av_DocxCobrarParam dp_op
        ON dp_op.nId_Cliente = dc_op.nId_Cliente
       AND dp_op.nId_Cartera = dc_op.nId_Cartera
       AND dp_op.nId_DocxCobrar = dc_op.nId_DocxCobrar

    LEFT JOIN integracion_datos_analitica.tipo_cambio_fallback_maf fb
        ON fb.nId_Cartera = pf.nId_Cartera

    LEFT JOIN #TipoCambioCarteraDia tcd
        ON tcd.nId_Cartera = pf.nId_Cartera
       AND tcd.fecha_gestion = CONVERT(DATE, pf.fecha_hora_gestion)

    OUTER APPLY
    (
        SELECT
            TRY_CONVERT
            (
                DECIMAL(19,6),
                NULLIF
                (
                    REPLACE(LTRIM(RTRIM(dp_op.cDocParam76)), ',', '.'),
                    ''
                )
            ) AS tc76
    ) rawtc

    OUTER APPLY
    (
        SELECT
            CONVERT
            (
                DECIMAL(19,6),
                CASE
                    WHEN dc_op.nId_DocxCobrar IS NOT NULL
                     AND dp_op.nId_DocxCobrar IS NOT NULL
                     AND ISNULL(dc_op.nId_DocxCobrarEst,1) <> 8
                     AND UPPER
                         (
                             LTRIM
                             (
                                 RTRIM
                                 (
                                     ISNULL(dc_op.cCampo4,'')
                                 )
                             )
                         ) <> 'RETIRADO'
                     AND dc_op.dDoc_FecIngreso
                         <
                         DATEADD
                         (
                             DAY,
                             1,
                             CONVERT
                             (
                                 DATETIME2(3),
                                 CONVERT(DATE, pf.fecha_hora_gestion)
                             )
                         )
                    THEN
                        CASE
                            WHEN rawtc.tc76 BETWEEN 2.5 AND 5.0
                                THEN rawtc.tc76

                            WHEN fb.tipo_cambio BETWEEN 2.5 AND 5.0
                                THEN fb.tipo_cambio

                            ELSE NULL
                        END

                    ELSE NULL
                END
            ) AS tc_documento_dia
    ) tdoc

    OUTER APPLY
    (
        SELECT
            CONVERT
            (
                DECIMAL(19,6),
                COALESCE
                (
                    tdoc.tc_documento_dia,
                    tcd.tipo_cambio_cartera_dia
                )
            ) AS tc_efectivo_dia
    ) tc;


    /* =====================================================================
       3B. DIAGNOSTICO DE OPERACIONES FUERA DEL MES DE CAMPANA

       Solo informativo. Estas operaciones existen en SISGES pero no forman
       parte del contrato mensual de hechos MAF porque la cartera pertenece
       a otro nAnioCar/nCampCar.
       ===================================================================== */

    SELECT
        COUNT_BIG(*) AS operaciones_promesa_fuera_mes_campana
    FROM dbo.av_DocxCobrarOpe o
    INNER JOIN #Carteras c
        ON c.nId_Cartera = o.nId_Cartera
    INNER JOIN dbo.av_Usuario u
        ON u.nId_Usuario = o.nId_UsuOpe
       AND u.nId_PerfilGest IS NULL
    INNER JOIN dbo.av_OpeCodCliOut cod
        ON cod.nId_Cliente = o.nId_Cliente
       AND cod.nId_OpeCodCliOut = o.nId_OpeCodOut
       AND cod.nId_TipoContacto = 2
    WHERE o.nId_Cliente = @IdCliente
      AND o.tip_gestion IN (1,4)
      AND COALESCE(o.dDoc_FecIngresoGes, o.dDocCobOpe_FecIni) >= @FechaDesde
      AND COALESCE(o.dDoc_FecIngresoGes, o.dDocCobOpe_FecIni) <  @FechaHasta
      AND
      (
          COALESCE(o.dDoc_FecIngresoGes, o.dDocCobOpe_FecIni)
              < c.fecha_inicio_campana
          OR
          COALESCE(o.dDoc_FecIngresoGes, o.dDocCobOpe_FecIni)
              >= c.fecha_fin_exclusiva_campana
      );


    /* =====================================================================
       4. PREFLIGHT PROMESAS
       ===================================================================== */

    IF EXISTS
    (
        SELECT 1
        FROM #PromesaFuente
        WHERE monto_comp <> 0
          AND monto_comp_dolares <> 0
          AND moneda_documento IS NULL
    )
    BEGIN
        THROW 52103,
              'Existe una promesa MAF con monto PEN y USD simultaneos sin moneda de documento para resolverla.',
              1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM #PromesaFuente
        WHERE
        (
               (moneda_documento = 2 AND monto_comp_dolares <> 0)
            OR (moneda_documento IS NULL
                AND monto_comp = 0
                AND monto_comp_dolares <> 0)
        )
          AND ISNULL(tipo_cambio,0) <= 0
    )
    BEGIN
        THROW 52104,
              'Existe una promesa MAF USD sin tipo de cambio valido.',
              1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM #PromesaFuente
        WHERE monto_promesa_pen IS NULL
    )
    BEGIN
        THROW 52105,
              'Existe una promesa MAF cuyo monto no pudo resolverse de forma deterministica.',
              1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM #PromesaFuente
        WHERE monto_promesa_pen < 0
    )
    BEGIN
        THROW 52109,
              'Existe una promesa MAF con monto negativo.',
              1;
    END;


    /* =====================================================================
       5. VENTANA DE ATRIBUCION

       Un pago pertenece a:
         [fecha promesa actual, fecha siguiente promesa)
       del mismo deudor/cartera.

       Evita utilizar el mismo pago para dos promesas sucesivas.
       ===================================================================== */

    IF OBJECT_ID('tempdb..#PromesaVentana') IS NOT NULL
        DROP TABLE #PromesaVentana;

    SELECT
        pf.*,

        sig.fecha_siguiente_promesa

    INTO #PromesaVentana

    FROM #PromesaFuente pf

    OUTER APPLY
    (
        SELECT TOP (1)
            CONVERT
            (
                DATETIME2(3),
                COALESCE
                (
                    o2.dDoc_FecIngresoGes,
                    o2.dDocCobOpe_FecIni
                )
            ) AS fecha_siguiente_promesa

        FROM dbo.av_DocxCobrarOpe o2

        INNER JOIN dbo.av_Usuario u2
            ON u2.nId_Usuario = o2.nId_UsuOpe
           AND u2.nId_PerfilGest IS NULL

        INNER JOIN dbo.av_OpeCodCliOut cod2
            ON cod2.nId_Cliente = o2.nId_Cliente
           AND cod2.nId_OpeCodCliOut = o2.nId_OpeCodOut
           AND cod2.nId_TipoContacto = 2

        WHERE o2.nId_Cliente = @IdCliente
          AND o2.nId_Cartera = pf.nId_Cartera
          AND o2.nId_PersDeudor = pf.id_deudor_origen
          AND o2.tip_gestion IN (1,4)

          AND
          (
              COALESCE
              (
                  o2.dDoc_FecIngresoGes,
                  o2.dDocCobOpe_FecIni
              ) > pf.fecha_hora_gestion

              OR
              (
                  COALESCE
                  (
                      o2.dDoc_FecIngresoGes,
                      o2.dDocCobOpe_FecIni
                  ) = pf.fecha_hora_gestion
                  AND o2.nId_DocxCobrarOpe > pf.id_operacion_origen
              )
          )

        ORDER BY
            COALESCE
            (
                o2.dDoc_FecIngresoGes,
                o2.dDocCobOpe_FecIni
            ),
            o2.nId_DocxCobrarOpe

    ) sig;


    /* =====================================================================
       6. PAGOS NORMALIZADOS A PEN
       ===================================================================== */

    IF OBJECT_ID('tempdb..#PagoFuente') IS NOT NULL
        DROP TABLE #PagoFuente;

    SELECT
        CONVERT(BIGINT, p.nId_DocxPago)
            AS id_pago_origen,

        p.nId_Cartera,

        CONVERT(BIGINT, p.nId_PersDeudor)
            AS id_deudor_origen,

        p.nId_DocxCobrar,
        p.nId_MonPago,

        CONVERT(DATETIME2(3), p.dDoc_FecPago)
            AS fecha_pago,

        CONVERT
        (
            DECIMAL(19,6),
            CASE
                WHEN tc_doc.tc BETWEEN 2.5 AND 5.0
                    THEN tc_doc.tc
                ELSE tcc.tipo_cambio_cartera
            END
        ) AS tipo_cambio,

        CONVERT
        (
            DECIMAL(19,4),
            CASE
                WHEN p.nId_MonPago = 1
                    THEN ISNULL(p.nDoc_ImpPago,0)

                WHEN p.nId_MonPago = 2
                    THEN ISNULL(p.nDoc_ImpPago,0)
                         *
                         CASE
                             WHEN tc_doc.tc BETWEEN 2.5 AND 5.0
                                 THEN tc_doc.tc
                             ELSE tcc.tipo_cambio_cartera
                         END

                ELSE 0
            END
        ) AS monto_pago_pen

    INTO #PagoFuente

    FROM dbo.av_DocxPago p

    INNER JOIN #Carteras c
        ON c.nId_Cartera = p.nId_Cartera

    INNER JOIN dbo.av_DocxCobrar dc
        ON dc.nId_Cliente = p.nId_Cliente
       AND dc.nId_Cartera = p.nId_Cartera
       AND dc.nId_DocxCobrar = p.nId_DocxCobrar
       AND dc.nId_PersDeudor = p.nId_PersDeudor

    LEFT JOIN dbo.av_DocxCobrarParam dp
        ON dp.nId_Cliente = dc.nId_Cliente
       AND dp.nId_Cartera = dc.nId_Cartera
       AND dp.nId_DocxCobrar = dc.nId_DocxCobrar

    OUTER APPLY
    (
        SELECT
            TRY_CONVERT
            (
                DECIMAL(19,6),
                NULLIF
                (
                    REPLACE(LTRIM(RTRIM(dp.cDocParam76)), ',', '.'),
                    ''
                )
            ) AS tc
    ) tc_doc

    LEFT JOIN #TipoCambioCartera tcc
        ON tcc.nId_Cartera = p.nId_Cartera

    WHERE p.nId_Cliente = @IdCliente
      AND p.dDoc_FecPago < @FechaHasta
      AND ISNULL(dc.nId_DocxCobrarEst,1) <> 8
      AND UPPER(LTRIM(RTRIM(ISNULL(dc.cCampo4,'')))) <> 'RETIRADO'
      AND dc.dDoc_FecIngreso < @FechaHasta;


    IF EXISTS
    (
        SELECT 1
        FROM #PagoFuente
        WHERE nId_MonPago NOT IN (1,2)
    )
    BEGIN
        THROW 52106,
              'MAF contiene pagos en una moneda no soportada.',
              1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM #PagoFuente
        WHERE nId_MonPago = 2
          AND ISNULL(tipo_cambio,0) <= 0
    )
    BEGIN
        THROW 52107,
              'Existe un pago MAF USD sin tipo de cambio valido.',
              1;
    END;


    /* =====================================================================
       7. PAGO ATRIBUIDO POR PROMESA
       ===================================================================== */

    IF OBJECT_ID('tempdb..#PromesaPago') IS NOT NULL
        DROP TABLE #PromesaPago;

    SELECT
        pv.id_operacion_origen,

        CONVERT
        (
            DECIMAL(19,4),
            ISNULL(SUM(pg.monto_pago_pen),0)
        ) AS monto_pagado_bruto,

        CONVERT
        (
            DECIMAL(19,4),
            ISNULL
            (
                SUM
                (
                    CASE
                        WHEN pv.fecha_vencimiento_promesa IS NOT NULL
                         AND CONVERT(DATE, pg.fecha_pago)
                             <= pv.fecha_vencimiento_promesa
                            THEN pg.monto_pago_pen
                        ELSE 0
                    END
                ),
                0
            )
        ) AS monto_pagado_hasta_vencimiento,

        MAX(pg.fecha_pago)
            AS fecha_ultimo_pago

    INTO #PromesaPago

    FROM #PromesaVentana pv

    LEFT JOIN #PagoFuente pg
        ON pg.nId_Cartera = pv.nId_Cartera
       AND pg.id_deudor_origen = pv.id_deudor_origen

       /*
          Pago en el mismo dia de la promesa se considera atribuible.
          av_DocxPago se usa funcionalmente como fecha de pago, no como
          secuencia temporal de alta.
       */
       AND CONVERT(DATE, pg.fecha_pago)
           >= CONVERT(DATE, pv.fecha_hora_gestion)

       AND pg.fecha_pago < @FechaHasta

       /*
          Si existe otra promesa posterior, sus pagos ya no se atribuyen
          a la promesa anterior. Al trabajar por DATE, una nueva promesa
          en el mismo dia toma los pagos de ese dia.
       */
       AND
       (
           pv.fecha_siguiente_promesa IS NULL
           OR CONVERT(DATE, pg.fecha_pago)
              < CONVERT(DATE, pv.fecha_siguiente_promesa)
       )

    GROUP BY
        pv.id_operacion_origen;


    /* =====================================================================
       8. STAGE FINAL
       ===================================================================== */

    IF OBJECT_ID('tempdb..#PromesaStage') IS NOT NULL
        DROP TABLE #PromesaStage;

    SELECT
        pv.clave_campana,
        pv.clave_cartera,

        CAST(NULL AS BIGINT)
            AS clave_asesor,

        pv.id_operacion_origen,
        pv.id_deudor_origen,
        pv.fecha_hora_gestion,
        pv.fecha_vencimiento_promesa,

        CONVERT
        (
            DECIMAL(19,4),
            ISNULL(pv.monto_promesa_pen,0)
        ) AS monto_promesa,

        /*
           Se almacena monto cubierto, no sobrepago.
           El bruto permanece disponible durante el stage para clasificar.
        */
        CONVERT
        (
            DECIMAL(19,4),
            CASE
                WHEN ISNULL(pp.monto_pagado_bruto,0)
                     > ISNULL(pv.monto_promesa_pen,0)
                 AND ISNULL(pv.monto_promesa_pen,0) > 0
                    THEN pv.monto_promesa_pen
                ELSE ISNULL(pp.monto_pagado_bruto,0)
            END
        ) AS monto_pagado,

        pp.fecha_ultimo_pago,

        pv.estado_origen,

        CASE
            /*
               Una promesa valida requiere:
                 - tipificacion de Promesa Pago (ya garantizada por la fuente),
                 - vencimiento,
                 - monto positivo.
            */
            WHEN pv.fecha_vencimiento_promesa IS NULL
              OR ISNULL(pv.monto_promesa_pen,0) <= 0
              OR pv.fecha_vencimiento_promesa
                 < CONVERT(DATE, pv.fecha_hora_gestion)
                THEN 'UNKNOWN'

            /*
               Cubierta completamente antes o hasta el vencimiento.
            */
            WHEN ISNULL(pp.monto_pagado_hasta_vencimiento,0)
                 >= pv.monto_promesa_pen
                THEN 'FULFILLED'

            /*
               Se completo, pero solo considerando pagos posteriores
               al vencimiento y antes de la siguiente promesa.
            */
            WHEN ISNULL(pp.monto_pagado_bruto,0)
                 >= pv.monto_promesa_pen
                THEN 'FULFILLED_OUT_OF_RANGE'

            /*
               Aun no vence al corte.
               Puede tener abonos, pero sigue siendo ACTIVE mientras
               no este totalmente cubierta.
            */
            WHEN @FechaCorte < pv.fecha_vencimiento_promesa
                THEN 'ACTIVE'

            /*
               Vence exactamente en la fecha de corte.
            */
            WHEN @FechaCorte = pv.fecha_vencimiento_promesa
             AND ISNULL(pp.monto_pagado_bruto,0) <= 0
                THEN 'DUE_TODAY'

            WHEN @FechaCorte = pv.fecha_vencimiento_promesa
             AND ISNULL(pp.monto_pagado_bruto,0) > 0
                THEN 'PARTIAL'

            /*
               Ya vencio y solo tiene pago parcial.
            */
            WHEN @FechaCorte > pv.fecha_vencimiento_promesa
             AND ISNULL(pp.monto_pagado_bruto,0) > 0
                THEN 'PARTIAL'

            /*
               Ya vencio y no registra pago atribuible.
            */
            WHEN @FechaCorte > pv.fecha_vencimiento_promesa
                THEN 'BROKEN'

            ELSE 'UNKNOWN'
        END AS codigo_estado,

        CONVERT
        (
            BIT,
            CASE
                WHEN pv.fecha_vencimiento_promesa IS NOT NULL
                 AND ISNULL(pv.monto_promesa_pen,0) > 0
                 AND pv.fecha_vencimiento_promesa
                     >= CONVERT(DATE, pv.fecha_hora_gestion)
                    THEN 1
                ELSE 0
            END
        ) AS es_promesa_valida,

        CONVERT
        (
            DATETIME2(3),
            CASE
                WHEN pp.fecha_ultimo_pago IS NOT NULL
                 AND pp.fecha_ultimo_pago > pv.fecha_hora_gestion
                    THEN pp.fecha_ultimo_pago
                ELSE pv.fecha_hora_gestion
            END
        ) AS fecha_actualizacion_origen,

        pp.monto_pagado_bruto,
        pp.monto_pagado_hasta_vencimiento,
        pv.nId_Cartera,
        pv.tipo_cambio,
        pv.fecha_siguiente_promesa

    INTO #PromesaStage

    FROM #PromesaVentana pv

    LEFT JOIN #PromesaPago pp
        ON pp.id_operacion_origen = pv.id_operacion_origen;


    /* =====================================================================
       9. CONTROLES DE STAGE
       ===================================================================== */

    IF EXISTS
    (
        SELECT 1
        FROM #PromesaStage
        GROUP BY id_operacion_origen
        HAVING COUNT(*) > 1
    )
    BEGIN
        THROW 52108,
              'El stage MAF genero operaciones de promesa duplicadas.',
              1;
    END;


    /* =====================================================================
       10. PREVIEW / PREFLIGHT
       ===================================================================== */

    SELECT
        @FechaDesde AS fecha_desde,
        @FechaCorte AS fecha_corte,

        COUNT_BIG(*) AS operaciones_promesa_fuente,

        SUM
        (
            CASE
                WHEN es_promesa_valida = 1
                    THEN 1
                ELSE 0
            END
        ) AS promesas_validas,

        SUM
        (
            CASE
                WHEN es_promesa_valida = 0
                    THEN 1
                ELSE 0
            END
        ) AS promesas_invalidas,

        COUNT
        (
            DISTINCT clave_campana
        ) AS campanas,

        COUNT
        (
            DISTINCT clave_cartera
        ) AS carteras,

        COUNT
        (
            DISTINCT id_deudor_origen
        ) AS deudores,

        CONVERT
        (
            DECIMAL(19,4),
            SUM
            (
                CASE
                    WHEN es_promesa_valida = 1
                        THEN monto_promesa
                    ELSE 0
                END
            )
        ) AS monto_prometido_pen,

        CONVERT
        (
            DECIMAL(19,4),
            SUM
            (
                CASE
                    WHEN es_promesa_valida = 1
                        THEN monto_pagado
                    ELSE 0
                END
            )
        ) AS monto_pagado_atribuido_pen

    FROM #PromesaStage;


    SELECT
        codigo_estado,
        COUNT_BIG(*) AS cantidad,
        COUNT(DISTINCT id_deudor_origen) AS deudores,

        CONVERT
        (
            DECIMAL(19,4),
            SUM(monto_promesa)
        ) AS monto_promesa,

        CONVERT
        (
            DECIMAL(19,4),
            SUM(monto_pagado)
        ) AS monto_pagado

    FROM #PromesaStage
    WHERE es_promesa_valida = 1

    GROUP BY codigo_estado
    ORDER BY codigo_estado;


    SELECT
        COUNT(*) AS operaciones_duplicadas_stage
    FROM
    (
        SELECT id_operacion_origen
        FROM #PromesaStage
        GROUP BY id_operacion_origen
        HAVING COUNT(*) > 1
    ) d;


    SELECT
        COUNT(*) AS promesas_usd_sin_tc
    FROM #PromesaFuente
    WHERE
    (
           (moneda_documento = 2 AND monto_comp_dolares <> 0)
        OR (moneda_documento IS NULL
            AND monto_comp = 0
            AND monto_comp_dolares <> 0)
    )
      AND ISNULL(tipo_cambio,0) <= 0;


    /*
       Tener ambos campos poblados NO es error si conocemos la moneda
       del documento. Esta salida sirve para auditar la casuistica.
    */
    SELECT
        ISNULL(CONVERT(VARCHAR(20), moneda_documento), 'SIN_DOCUMENTO')
            AS moneda_documento,
        COUNT(*) AS promesas_monto_dual,

        CONVERT
        (
            DECIMAL(19,4),
            SUM(monto_comp)
        ) AS suma_monto_comp,

        CONVERT
        (
            DECIMAL(19,4),
            SUM(monto_comp_dolares)
        ) AS suma_monto_comp_dolares,

        CONVERT
        (
            DECIMAL(19,4),
            SUM(monto_promesa_pen)
        ) AS suma_monto_normalizado_pen

    FROM #PromesaFuente

    WHERE monto_comp <> 0
      AND monto_comp_dolares <> 0

    GROUP BY moneda_documento
    ORDER BY moneda_documento;


    SELECT
        COUNT(*) AS promesas_monto_dual_sin_moneda
    FROM #PromesaFuente
    WHERE monto_comp <> 0
      AND monto_comp_dolares <> 0
      AND moneda_documento IS NULL;


    SELECT
        COUNT(*) AS promesas_fecha_invalida
    FROM #PromesaStage
    WHERE fecha_vencimiento_promesa IS NULL
       OR fecha_vencimiento_promesa
          < CONVERT(DATE, fecha_hora_gestion);


    /* =====================================================================
       DIAGNOSTICO MONETARIO FINAL

       Comparar la regla detallada (moneda_documento autoritativa) contra
       la regla legacy actualmente usada por hecho_cartera_diario.

       Esto permite identificar exactamente qué configuraciones históricas
       generan las diferencias antes de modificar ningún dato.
       ===================================================================== */

    IF OBJECT_ID('tempdb..#DiferenciaReglaMonetaria') IS NOT NULL
        DROP TABLE #DiferenciaReglaMonetaria;

    SELECT
        pf.clave_campana,
        pf.clave_cartera,
        pf.nId_Cartera,
        pf.id_operacion_origen,
        pf.id_deudor_origen,
        pf.nId_DocxCobrar,

        CONVERT(DATE, pf.fecha_hora_gestion)
            AS fecha_gestion,

        pf.moneda_documento,
        pf.monto_comp,
        pf.monto_comp_dolares,
        pf.tipo_cambio,

        pf.monto_promesa_pen
            AS monto_detalle,

        pf.monto_promesa_pen_legacy
            AS monto_legacy,

        CONVERT
        (
            DECIMAL(19,4),
            ISNULL(pf.monto_promesa_pen,0)
            -
            ISNULL(pf.monto_promesa_pen_legacy,0)
        ) AS diferencia,

        CASE
            WHEN pf.moneda_documento = 1
             AND pf.monto_comp <> 0
             AND pf.monto_comp_dolares = 0
                THEN 'PEN_NORMAL'

            WHEN pf.moneda_documento = 2
             AND pf.monto_comp = 0
             AND pf.monto_comp_dolares <> 0
                THEN 'USD_NORMAL'

            WHEN pf.moneda_documento = 1
             AND pf.monto_comp <> 0
             AND pf.monto_comp_dolares <> 0
                THEN 'PEN_DUAL'

            WHEN pf.moneda_documento = 2
             AND pf.monto_comp <> 0
             AND pf.monto_comp_dolares <> 0
                THEN 'USD_DUAL'

            WHEN pf.moneda_documento = 1
             AND pf.monto_comp = 0
             AND pf.monto_comp_dolares <> 0
                THEN 'PEN_SOLO_CAMPO_USD'

            WHEN pf.moneda_documento = 2
             AND pf.monto_comp <> 0
             AND pf.monto_comp_dolares = 0
                THEN 'USD_SOLO_CAMPO_PEN'

            WHEN pf.moneda_documento = 1
             AND pf.monto_comp = 0
             AND pf.monto_comp_dolares = 0
                THEN 'PEN_SIN_MONTO'

            WHEN pf.moneda_documento = 2
             AND pf.monto_comp = 0
             AND pf.monto_comp_dolares = 0
                THEN 'USD_SIN_MONTO'

            WHEN pf.moneda_documento IS NULL
             AND pf.monto_comp <> 0
             AND pf.monto_comp_dolares = 0
                THEN 'SIN_DOC_SOLO_PEN'

            WHEN pf.moneda_documento IS NULL
             AND pf.monto_comp = 0
             AND pf.monto_comp_dolares <> 0
                THEN 'SIN_DOC_SOLO_USD'

            WHEN pf.moneda_documento IS NULL
             AND pf.monto_comp <> 0
             AND pf.monto_comp_dolares <> 0
                THEN 'SIN_DOC_DUAL'

            ELSE 'OTRO'
        END AS patron_monetario

    INTO #DiferenciaReglaMonetaria

    FROM #PromesaFuente pf

    WHERE ABS
          (
              ISNULL(pf.monto_promesa_pen,0)
              -
              ISNULL(pf.monto_promesa_pen_legacy,0)
          ) > 0.01;


    SELECT
        COUNT(*) AS operaciones_con_diferencia_regla,

        COUNT(DISTINCT id_deudor_origen)
            AS deudores_con_diferencia_regla,

        COUNT(DISTINCT clave_cartera)
            AS carteras_con_diferencia_regla,

        COUNT(DISTINCT fecha_gestion)
            AS fechas_con_diferencia_regla,

        CONVERT
        (
            DECIMAL(19,4),
            ISNULL(SUM(ABS(diferencia)),0)
        ) AS diferencia_absoluta_regla

    FROM #DiferenciaReglaMonetaria;


    SELECT
        patron_monetario,
        moneda_documento,

        COUNT(*) AS operaciones,

        CONVERT
        (
            DECIMAL(19,4),
            SUM(monto_comp)
        ) AS suma_monto_comp,

        CONVERT
        (
            DECIMAL(19,4),
            SUM(monto_comp_dolares)
        ) AS suma_monto_comp_dolares,

        CONVERT
        (
            DECIMAL(19,4),
            SUM(monto_detalle)
        ) AS suma_monto_detalle,

        CONVERT
        (
            DECIMAL(19,4),
            SUM(monto_legacy)
        ) AS suma_monto_legacy,

        CONVERT
        (
            DECIMAL(19,4),
            SUM(diferencia)
        ) AS diferencia_neta,

        CONVERT
        (
            DECIMAL(19,4),
            SUM(ABS(diferencia))
        ) AS diferencia_absoluta

    FROM #DiferenciaReglaMonetaria

    GROUP BY
        patron_monetario,
        moneda_documento

    ORDER BY
        diferencia_absoluta DESC,
        patron_monetario;


    SELECT TOP (100)
        fecha_gestion,
        clave_campana,
        clave_cartera,
        nId_Cartera,
        id_operacion_origen,
        id_deudor_origen,
        nId_DocxCobrar,
        moneda_documento,
        patron_monetario,
        monto_comp,
        monto_comp_dolares,
        tipo_cambio,
        monto_detalle,
        monto_legacy,
        diferencia

    FROM #DiferenciaReglaMonetaria

    ORDER BY
        ABS(diferencia) DESC,
        fecha_gestion,
        id_operacion_origen;


    /*
       Comparacion contra cantidad_promesas_dia ya cargada.
       Se usa fecha_hora_gestion, ahora alineada con dDoc_FecIngresoGes.
    */
    IF OBJECT_ID('tempdb..#ComparacionDia') IS NOT NULL
        DROP TABLE #ComparacionDia;

    ;WITH StageDia AS
    (
        SELECT
            CONVERT(DATE, fecha_hora_gestion) AS fecha,
            clave_cartera,
            COUNT_BIG(*) AS cantidad_stage
        FROM #PromesaStage
        GROUP BY
            CONVERT(DATE, fecha_hora_gestion),
            clave_cartera
    ),
    HechoDia AS
    (
        SELECT
            df.fecha_calendario AS fecha,
            h.clave_cartera,
            CONVERT(BIGINT, h.cantidad_promesas_dia)
                AS cantidad_hecho
        FROM analitica.hecho_cartera_diario h
        INNER JOIN analitica.dim_fecha df
            ON df.clave_fecha = h.clave_fecha
        WHERE h.clave_cliente = @ClaveCliente
          AND df.fecha_calendario >= @FechaDesde
          AND df.fecha_calendario <= @FechaCorte
    )
    SELECT
        COALESCE(s.fecha, h.fecha) AS fecha,
        COALESCE(s.clave_cartera, h.clave_cartera) AS clave_cartera,
        ISNULL(s.cantidad_stage,0) AS cantidad_stage,
        ISNULL(h.cantidad_hecho,0) AS cantidad_hecho
    INTO #ComparacionDia
    FROM StageDia s
    FULL OUTER JOIN HechoDia h
        ON h.fecha = s.fecha
       AND h.clave_cartera = s.clave_cartera
    WHERE ISNULL(s.cantidad_stage,0)
          <> ISNULL(h.cantidad_hecho,0);


    SELECT
        COUNT(*) AS filas_diferencia_dia,
        ISNULL(SUM(cantidad_stage),0) AS promesas_stage_en_diferencias,
        ISNULL(SUM(cantidad_hecho),0) AS promesas_hecho_en_diferencias,
        ISNULL(SUM(ABS(cantidad_stage - cantidad_hecho)),0)
            AS diferencia_absoluta_promesas
    FROM #ComparacionDia;


    SELECT TOP (100)
        fecha,
        clave_cartera,
        cantidad_stage,
        cantidad_hecho,
        cantidad_stage - cantidad_hecho AS diferencia
    FROM #ComparacionDia
    ORDER BY
        ABS(cantidad_stage - cantidad_hecho) DESC,
        fecha,
        clave_cartera;


    /*
       VALIDACION DE REPLICA LEGACY

       Si esto devuelve 0 diferencias, queda demostrado que:
         - la temporalidad del TC quedó replicada;
         - las diferencias restantes entre monto_promesa detallado y
           hecho_cartera_diario provienen de la REGLA monetaria, no del TC.
    */
    IF OBJECT_ID('tempdb..#ComparacionLegacyDia') IS NOT NULL
        DROP TABLE #ComparacionLegacyDia;

    ;WITH LegacyDia AS
    (
        SELECT
            CONVERT(DATE, fecha_hora_gestion) AS fecha,
            clave_cartera,
            COUNT_BIG(*) AS cantidad_stage,
            CONVERT
            (
                DECIMAL(19,4),
                SUM(ISNULL(monto_promesa_pen_legacy,0))
            ) AS monto_legacy
        FROM #PromesaFuente
        GROUP BY
            CONVERT(DATE, fecha_hora_gestion),
            clave_cartera
    ),
    HechoDia AS
    (
        SELECT
            df.fecha_calendario AS fecha,
            h.clave_cartera,
            CONVERT(BIGINT, h.cantidad_promesas_dia)
                AS cantidad_hecho,
            CONVERT
            (
                DECIMAL(19,4),
                ISNULL(h.monto_promesas_dia,0)
            ) AS monto_hecho
        FROM analitica.hecho_cartera_diario h
        INNER JOIN analitica.dim_fecha df
            ON df.clave_fecha = h.clave_fecha
        WHERE h.clave_cliente = @ClaveCliente
          AND df.fecha_calendario >= @FechaDesde
          AND df.fecha_calendario <= @FechaCorte
    )
    SELECT
        COALESCE(l.fecha,h.fecha) AS fecha,
        COALESCE(l.clave_cartera,h.clave_cartera) AS clave_cartera,
        ISNULL(l.cantidad_stage,0) AS cantidad_stage,
        ISNULL(h.cantidad_hecho,0) AS cantidad_hecho,
        ISNULL(l.monto_legacy,0) AS monto_legacy,
        ISNULL(h.monto_hecho,0) AS monto_hecho,
        CONVERT
        (
            DECIMAL(19,4),
            ISNULL(l.monto_legacy,0) - ISNULL(h.monto_hecho,0)
        ) AS diferencia_monto
    INTO #ComparacionLegacyDia
    FROM LegacyDia l
    FULL OUTER JOIN HechoDia h
        ON h.fecha = l.fecha
       AND h.clave_cartera = l.clave_cartera
    WHERE ISNULL(l.cantidad_stage,0) <> ISNULL(h.cantidad_hecho,0)
       OR ABS(ISNULL(l.monto_legacy,0) - ISNULL(h.monto_hecho,0)) > 0.01;


    SELECT
        COUNT(*) AS dias_cartera_diferencia_legacy,
        SUM
        (
            CASE
                WHEN cantidad_stage <> cantidad_hecho
                    THEN 1
                ELSE 0
            END
        ) AS dias_cartera_diferencia_cantidad_legacy,
        CONVERT
        (
            DECIMAL(19,4),
            ISNULL(SUM(ABS(diferencia_monto)),0)
        ) AS diferencia_absoluta_monto_legacy
    FROM #ComparacionLegacyDia;


    SELECT TOP (100)
        fecha,
        clave_cartera,
        cantidad_stage,
        cantidad_hecho,
        monto_legacy,
        monto_hecho,
        diferencia_monto
    FROM #ComparacionLegacyDia
    ORDER BY
        ABS(diferencia_monto) DESC,
        fecha,
        clave_cartera;


    /*
       Comparacion monetaria diaria.
       Se separa de la comparación de cantidades para identificar exactamente
       en qué fechas el detalle y el agregado usan una normalización monetaria
       distinta.
    */
    IF OBJECT_ID('tempdb..#ComparacionMontoDia') IS NOT NULL
        DROP TABLE #ComparacionMontoDia;

    ;WITH StageMontoDia AS
    (
        SELECT
            CONVERT(DATE, fecha_hora_gestion) AS fecha,
            clave_cartera,
            COUNT_BIG(*) AS cantidad_stage,
            CONVERT
            (
                DECIMAL(19,4),
                SUM(ISNULL(monto_promesa,0))
            ) AS monto_stage
        FROM #PromesaStage
        GROUP BY
            CONVERT(DATE, fecha_hora_gestion),
            clave_cartera
    ),
    HechoMontoDia AS
    (
        SELECT
            df.fecha_calendario AS fecha,
            h.clave_cartera,
            CONVERT(BIGINT, h.cantidad_promesas_dia)
                AS cantidad_hecho,
            CONVERT
            (
                DECIMAL(19,4),
                ISNULL(h.monto_promesas_dia,0)
            ) AS monto_hecho
        FROM analitica.hecho_cartera_diario h
        INNER JOIN analitica.dim_fecha df
            ON df.clave_fecha = h.clave_fecha
        WHERE h.clave_cliente = @ClaveCliente
          AND df.fecha_calendario >= @FechaDesde
          AND df.fecha_calendario <= @FechaCorte
    )
    SELECT
        COALESCE(s.fecha, h.fecha) AS fecha,
        COALESCE(s.clave_cartera, h.clave_cartera) AS clave_cartera,
        ISNULL(s.cantidad_stage,0) AS cantidad_stage,
        ISNULL(h.cantidad_hecho,0) AS cantidad_hecho,
        ISNULL(s.monto_stage,0) AS monto_stage,
        ISNULL(h.monto_hecho,0) AS monto_hecho,
        CONVERT
        (
            DECIMAL(19,4),
            ISNULL(s.monto_stage,0) - ISNULL(h.monto_hecho,0)
        ) AS diferencia_monto
    INTO #ComparacionMontoDia
    FROM StageMontoDia s
    FULL OUTER JOIN HechoMontoDia h
        ON h.fecha = s.fecha
       AND h.clave_cartera = s.clave_cartera
    WHERE ISNULL(s.cantidad_stage,0) <> ISNULL(h.cantidad_hecho,0)
       OR ABS(ISNULL(s.monto_stage,0) - ISNULL(h.monto_hecho,0)) > 0.01;


    SELECT
        COUNT(*) AS dias_cartera_con_diferencia_monto,
        SUM
        (
            CASE
                WHEN cantidad_stage <> cantidad_hecho
                    THEN 1
                ELSE 0
            END
        ) AS dias_cartera_con_diferencia_cantidad,
        CONVERT
        (
            DECIMAL(19,4),
            ISNULL(SUM(ABS(diferencia_monto)),0)
        ) AS diferencia_absoluta_monto_dia
    FROM #ComparacionMontoDia;


    SELECT TOP (100)
        fecha,
        clave_cartera,
        cantidad_stage,
        cantidad_hecho,
        monto_stage,
        monto_hecho,
        diferencia_monto
    FROM #ComparacionMontoDia
    ORDER BY
        CASE
            WHEN cantidad_stage <> cantidad_hecho THEN 0
            ELSE 1
        END,
        ABS(diferencia_monto) DESC,
        fecha,
        clave_cartera;


    /*
       Comparacion agregada por cartera/campana contra el historico diario.
       Sirve especialmente para validar monto_promesas_dia.
    */
    IF OBJECT_ID('tempdb..#ComparacionCartera') IS NOT NULL
        DROP TABLE #ComparacionCartera;

    ;WITH StageCartera AS
    (
        SELECT
            clave_campana,
            clave_cartera,
            COUNT_BIG(*) AS cantidad_stage,
            CONVERT(DECIMAL(19,4), SUM(ISNULL(monto_promesa,0)))
                AS monto_stage
        FROM #PromesaStage
        GROUP BY
            clave_campana,
            clave_cartera
    ),
    HechoCartera AS
    (
        SELECT
            h.clave_campana,
            h.clave_cartera,
            SUM(CONVERT(BIGINT, h.cantidad_promesas_dia))
                AS cantidad_hecho,
            CONVERT
            (
                DECIMAL(19,4),
                SUM(ISNULL(h.monto_promesas_dia,0))
            ) AS monto_hecho
        FROM analitica.hecho_cartera_diario h
        INNER JOIN analitica.dim_fecha df
            ON df.clave_fecha = h.clave_fecha
        WHERE h.clave_cliente = @ClaveCliente
          AND df.fecha_calendario >= @FechaDesde
          AND df.fecha_calendario <= @FechaCorte
        GROUP BY
            h.clave_campana,
            h.clave_cartera
    )
    SELECT
        COALESCE(s.clave_campana, h.clave_campana)
            AS clave_campana,
        COALESCE(s.clave_cartera, h.clave_cartera)
            AS clave_cartera,
        ISNULL(s.cantidad_stage,0)
            AS cantidad_stage,
        ISNULL(h.cantidad_hecho,0)
            AS cantidad_hecho,
        ISNULL(s.monto_stage,0)
            AS monto_stage,
        ISNULL(h.monto_hecho,0)
            AS monto_hecho,
        CONVERT
        (
            DECIMAL(19,4),
            ISNULL(s.monto_stage,0) - ISNULL(h.monto_hecho,0)
        ) AS diferencia_monto
    INTO #ComparacionCartera
    FROM StageCartera s
    FULL OUTER JOIN HechoCartera h
        ON h.clave_campana = s.clave_campana
       AND h.clave_cartera = s.clave_cartera
    WHERE ISNULL(s.cantidad_stage,0)
          <> ISNULL(h.cantidad_hecho,0)
       OR ABS(ISNULL(s.monto_stage,0) - ISNULL(h.monto_hecho,0)) > 0.01;


    SELECT
        COUNT(*) AS carteras_con_diferencia,
        SUM
        (
            CASE
                WHEN cantidad_stage <> cantidad_hecho
                    THEN 1
                ELSE 0
            END
        ) AS carteras_con_diferencia_cantidad,
        SUM
        (
            CASE
                WHEN ABS(diferencia_monto) > 0.01
                    THEN 1
                ELSE 0
            END
        ) AS carteras_con_diferencia_monto,
        CONVERT
        (
            DECIMAL(19,4),
            ISNULL(SUM(ABS(diferencia_monto)),0)
        ) AS diferencia_absoluta_monto
    FROM #ComparacionCartera;


    SELECT TOP (100)
        clave_campana,
        clave_cartera,
        cantidad_stage,
        cantidad_hecho,
        monto_stage,
        monto_hecho,
        diferencia_monto
    FROM #ComparacionCartera
    ORDER BY
        CASE
            WHEN cantidad_stage <> cantidad_hecho THEN 0
            ELSE 1
        END,
        ABS(diferencia_monto) DESC,
        clave_campana,
        clave_cartera;


    /* =====================================================================
       11. APLICACION
       ===================================================================== */

    IF @Aplicar = 0
    BEGIN
        RETURN;
    END;


    BEGIN TRY
        BEGIN TRANSACTION;


        /* -------------------------------------------------------------
           UPDATE por grano natural.
           No se sobreescribe clave_asesor si en el futuro otra carga ya
           lo completo y este stage aun no tiene dimension de asesores MAF.
           ------------------------------------------------------------- */
        UPDATE p
        SET
            p.clave_campana =
                s.clave_campana,

            p.clave_cartera =
                s.clave_cartera,

            p.clave_asesor =
                COALESCE(s.clave_asesor, p.clave_asesor),

            p.id_deudor_origen =
                s.id_deudor_origen,

            p.fecha_hora_gestion =
                s.fecha_hora_gestion,

            p.fecha_vencimiento_promesa =
                s.fecha_vencimiento_promesa,

            p.monto_promesa =
                s.monto_promesa,

            p.monto_pagado =
                s.monto_pagado,

            p.fecha_ultimo_pago =
                s.fecha_ultimo_pago,

            p.estado_origen =
                s.estado_origen,

            p.codigo_estado =
                s.codigo_estado,

            p.es_promesa_valida =
                s.es_promesa_valida,

            p.fecha_actualizacion_origen =
                s.fecha_actualizacion_origen,

            p.fecha_carga =
                SYSUTCDATETIME()

        FROM analitica.hecho_promesa p

        INNER JOIN #PromesaStage s
            ON s.id_operacion_origen = p.id_operacion_origen

        WHERE p.clave_cliente = @ClaveCliente;


        /* -------------------------------------------------------------
           INSERT.
           clave_hecho_promesa es IDENTITY; nunca MAX+1.
           ------------------------------------------------------------- */
        INSERT INTO analitica.hecho_promesa
        (
            clave_cliente,
            clave_campana,
            clave_cartera,
            clave_asesor,

            id_operacion_origen,
            id_deudor_origen,

            fecha_hora_gestion,
            fecha_vencimiento_promesa,

            monto_promesa,
            monto_pagado,
            fecha_ultimo_pago,

            estado_origen,
            codigo_estado,
            es_promesa_valida,

            fecha_actualizacion_origen
        )
        SELECT
            @ClaveCliente,
            s.clave_campana,
            s.clave_cartera,
            s.clave_asesor,

            s.id_operacion_origen,
            s.id_deudor_origen,

            s.fecha_hora_gestion,
            s.fecha_vencimiento_promesa,

            s.monto_promesa,
            s.monto_pagado,
            s.fecha_ultimo_pago,

            s.estado_origen,
            s.codigo_estado,
            s.es_promesa_valida,

            s.fecha_actualizacion_origen

        FROM #PromesaStage s

        WHERE NOT EXISTS
        (
            SELECT 1
            FROM analitica.hecho_promesa p
            WHERE p.clave_cliente = @ClaveCliente
              AND p.id_operacion_origen = s.id_operacion_origen
        );


        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH;


    /* =====================================================================
       12. RESUMEN POST-APLICACION
       ===================================================================== */

    SELECT
        COUNT_BIG(*) AS filas_hecho_promesa_maf,

        SUM
        (
            CASE
                WHEN es_promesa_valida = 1
                    THEN 1
                ELSE 0
            END
        ) AS promesas_validas,

        COUNT
        (
            DISTINCT CASE
                WHEN es_promesa_valida = 1
                    THEN id_deudor_origen
            END
        ) AS deudores_con_promesa_valida,

        MIN(fecha_hora_gestion) AS primera_gestion,
        MAX(fecha_hora_gestion) AS ultima_gestion,

        MAX(fecha_carga) AS ultima_carga_utc

    FROM analitica.hecho_promesa

    WHERE clave_cliente = @ClaveCliente
      AND fecha_hora_gestion >= @FechaDesde
      AND fecha_hora_gestion < @FechaHasta;


    SELECT
        codigo_estado,
        COUNT_BIG(*) AS cantidad,

        CONVERT
        (
            DECIMAL(19,4),
            SUM(ISNULL(monto_promesa,0))
        ) AS monto_promesa,

        CONVERT
        (
            DECIMAL(19,4),
            SUM(ISNULL(monto_pagado,0))
        ) AS monto_pagado

    FROM analitica.hecho_promesa

    WHERE clave_cliente = @ClaveCliente
      AND es_promesa_valida = 1
      AND fecha_hora_gestion >= @FechaDesde
      AND fecha_hora_gestion < @FechaHasta

    GROUP BY codigo_estado

    ORDER BY codigo_estado;

END;
GO
