/*
===============================================================================
MAF PROMISE DAILY SYNC - DEFINICION CANONICA VERSIONADA
Procedimiento: integracion_datos_analitica.usp_sincronizar_promesas_cartera_diario_maf

Fuente autoritativa de cantidad_promesas_dia y monto_promesas_dia:
analitica.hecho_promesa.
===============================================================================
*/

USE [aval_cob];
GO

CREATE OR ALTER PROCEDURE
    integracion_datos_analitica.usp_sincronizar_promesas_cartera_diario_maf
(
    @FechaDesde DATE,
    @FechaHasta DATE,
    @MostrarResultado BIT = 0
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @IdCliente INT = 59,
        @ClaveCliente INT,
        @FechaHastaExclusiva DATETIME2(3),
        @AhoraUtc DATETIME2(3) = SYSUTCDATETIME(),
        @FilasActualizadas INT = 0;

    IF @FechaDesde IS NULL
       OR @FechaHasta IS NULL
       OR @FechaDesde > @FechaHasta
    BEGIN
        THROW 52820,
              'Rango de fechas invalido para sincronizar promesas MAF.',
              1;
    END;

    SET @FechaHastaExclusiva =
        DATEADD(DAY, 1, CONVERT(DATETIME2(3), @FechaHasta));

    SELECT
        @ClaveCliente = clave_cliente
    FROM analitica.dim_cliente
    WHERE id_cliente_crm = @IdCliente
      AND es_activo = 1;

    IF @ClaveCliente IS NULL
    BEGIN
        THROW 52821,
              'MAF no existe o no esta activo en analitica.dim_cliente.',
              1;
    END;

    IF OBJECT_ID(N'analitica.hecho_promesa', N'U') IS NULL
        THROW 52822, 'No existe analitica.hecho_promesa.', 1;

    IF OBJECT_ID(N'analitica.hecho_cartera_diario', N'U') IS NULL
        THROW 52823, 'No existe analitica.hecho_cartera_diario.', 1;

    IF OBJECT_ID('tempdb..#PromesasDia') IS NOT NULL
        DROP TABLE #PromesasDia;

    /*
       IMPORTANTE:
       Se cuentan TODAS las filas de hecho_promesa del rango, no solamente
       es_promesa_valida = 1.

       cantidad_promesas_dia del loader MAF representa eventos de promesa
       (tipo contacto 2, CALL/WhatsApp). La validez de vencimiento es otra
       dimension del detalle y no cambia la existencia del evento.
    */
    SELECT
        CONVERT(DATE, p.fecha_hora_gestion) AS fecha,
        p.clave_campana,
        p.clave_cartera,
        COUNT_BIG(*) AS cantidad_promesas,
        CONVERT
        (
            DECIMAL(19,4),
            SUM(ISNULL(p.monto_promesa, 0))
        ) AS monto_promesas
    INTO #PromesasDia
    FROM analitica.hecho_promesa AS p
    WHERE p.clave_cliente = @ClaveCliente
      AND p.fecha_hora_gestion >= @FechaDesde
      AND p.fecha_hora_gestion < @FechaHastaExclusiva
    GROUP BY
        CONVERT(DATE, p.fecha_hora_gestion),
        p.clave_campana,
        p.clave_cartera;

    CREATE UNIQUE CLUSTERED INDEX
        IX_PromesasDia
    ON #PromesasDia
    (
        fecha,
        clave_campana,
        clave_cartera
    );

    /* Todas las fechas del detalle deben existir en dim_fecha. */
    IF EXISTS
    (
        SELECT 1
        FROM #PromesasDia AS p
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM analitica.dim_fecha AS df
            WHERE CONVERT(DATE, df.fecha_calendario) = p.fecha
        )
    )
    BEGIN
        THROW 52824,
              'Hay fechas de hecho_promesa que no existen en analitica.dim_fecha.',
              1;
    END;

    /*
       No inventar una fila completa de hecho_cartera_diario solamente a partir
       de una promesa. Si falta el snapshot cartera/dia, debe reconstruirse con
       el loader diario y luego sincronizar las columnas de promesas.
    */
    IF EXISTS
    (
        SELECT 1
        FROM #PromesasDia AS p
        INNER JOIN analitica.dim_fecha AS df
            ON CONVERT(DATE, df.fecha_calendario) = p.fecha
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM analitica.hecho_cartera_diario AS h
            WHERE h.clave_fecha = df.clave_fecha
              AND h.clave_cliente = @ClaveCliente
              AND h.clave_campana = p.clave_campana
              AND h.clave_cartera = p.clave_cartera
        )
    )
    BEGIN
        THROW 52825,
              'Existen promesas MAF cuyo snapshot cartera/dia no existe. Reconstruya primero el corte diario faltante.',
              1;
    END;

    /*
       Para cada snapshot MAF existente en el rango:
         - si hay detalle de promesa, copiar cantidad y monto;
         - si no existe detalle, dejar ambos valores en cero.

       hecho_promesa queda como fuente autoritativa para estas dos metricas.
    */
    UPDATE h
    SET
        h.cantidad_promesas_dia =
            ISNULL(p.cantidad_promesas, 0),

        h.monto_promesas_dia =
            CONVERT
            (
                DECIMAL(19,4),
                ISNULL(p.monto_promesas, 0)
            ),

        h.fecha_carga = @AhoraUtc
    FROM analitica.hecho_cartera_diario AS h
    INNER JOIN analitica.dim_fecha AS df
        ON df.clave_fecha = h.clave_fecha
    LEFT JOIN #PromesasDia AS p
        ON p.fecha = CONVERT(DATE, df.fecha_calendario)
       AND p.clave_campana = h.clave_campana
       AND p.clave_cartera = h.clave_cartera
    WHERE h.clave_cliente = @ClaveCliente
      AND df.fecha_calendario >= @FechaDesde
      AND df.fecha_calendario < @FechaHastaExclusiva
      AND
      (
           ISNULL(CONVERT(BIGINT, h.cantidad_promesas_dia), 0)
                <> ISNULL(p.cantidad_promesas, 0)

        OR ABS
           (
               CONVERT
               (
                   DECIMAL(19,4),
                   ISNULL(h.monto_promesas_dia, 0)
               )
               -
               CONVERT
               (
                   DECIMAL(19,4),
                   ISNULL(p.monto_promesas, 0)
               )
           ) > 0.01
      );

    SET @FilasActualizadas = @@ROWCOUNT;

    IF @MostrarResultado = 1
    BEGIN
        ;WITH HechoDia AS
        (
            SELECT
                CONVERT(DATE, df.fecha_calendario) AS fecha,
                h.clave_campana,
                h.clave_cartera,
                CONVERT(BIGINT, h.cantidad_promesas_dia)
                    AS cantidad_hecho,
                CONVERT
                (
                    DECIMAL(19,4),
                    ISNULL(h.monto_promesas_dia, 0)
                ) AS monto_hecho
            FROM analitica.hecho_cartera_diario AS h
            INNER JOIN analitica.dim_fecha AS df
                ON df.clave_fecha = h.clave_fecha
            WHERE h.clave_cliente = @ClaveCliente
              AND df.fecha_calendario >= @FechaDesde
              AND df.fecha_calendario < @FechaHastaExclusiva
        ),
        Diferencias AS
        (
            SELECT
                COALESCE(p.fecha, h.fecha) AS fecha,
                COALESCE(p.clave_campana, h.clave_campana)
                    AS clave_campana,
                COALESCE(p.clave_cartera, h.clave_cartera)
                    AS clave_cartera,
                ISNULL(p.cantidad_promesas, 0)
                    AS cantidad_detalle,
                ISNULL(h.cantidad_hecho, 0)
                    AS cantidad_hecho,
                CONVERT
                (
                    DECIMAL(19,4),
                    ISNULL(p.monto_promesas, 0)
                ) AS monto_detalle,
                CONVERT
                (
                    DECIMAL(19,4),
                    ISNULL(h.monto_hecho, 0)
                ) AS monto_hecho
            FROM #PromesasDia AS p
            FULL OUTER JOIN HechoDia AS h
                ON h.fecha = p.fecha
               AND h.clave_campana = p.clave_campana
               AND h.clave_cartera = p.clave_cartera
            WHERE
                ISNULL(p.cantidad_promesas, 0)
                    <> ISNULL(h.cantidad_hecho, 0)
                OR ABS
                   (
                       ISNULL(p.monto_promesas, 0)
                       -
                       ISNULL(h.monto_hecho, 0)
                   ) > 0.01
        )
        SELECT
            @FechaDesde AS fecha_desde,
            @FechaHasta AS fecha_hasta,
            @FilasActualizadas AS filas_actualizadas,
            COUNT(*) AS diferencias_restantes,
            ISNULL
            (
                SUM
                (
                    ABS(cantidad_detalle - cantidad_hecho)
                ),
                0
            ) AS diferencia_absoluta_cantidad,
            CONVERT
            (
                DECIMAL(19,4),
                ISNULL
                (
                    SUM
                    (
                        ABS(monto_detalle - monto_hecho)
                    ),
                    0
                )
            ) AS diferencia_absoluta_monto
        FROM Diferencias;
    END;
END;
GO
