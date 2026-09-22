/*
===============================================================================
MAF PORTFOLIO DIMENSION SYNC - DEFINICION CANONICA VERSIONADA
Procedimiento: integracion_datos_analitica.usp_sincronizar_carteras_maf

Objetivo:
- sincronizar las carteras productivas MAF del mes con analitica.dim_cartera;
- actualizar metadatos de carteras ya conocidas;
- insertar nuevas carteras de forma idempotente;
- evitar que el ETL diario falle cuando aparece un nuevo tramo/cartera fisica.

Scope MAF:
- cliente CRM: 59
- contrato: 246
- grupo: 194
- unidad de negocio: MAF PREVCOBRA
===============================================================================
*/

USE [aval_cob];
GO

CREATE OR ALTER PROCEDURE
    integracion_datos_analitica.usp_sincronizar_carteras_maf
(
    @FechaCorte DATE = NULL,
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
        @UnidadNegocio VARCHAR(150) = 'MAF PREVCOBRA',
        @ClaveCliente INT,
        @AbrioTransaccion BIT =
            CASE WHEN @@TRANCOUNT = 0 THEN 1 ELSE 0 END,
        @Actualizadas INT = 0,
        @Insertadas INT = 0;

    IF @FechaCorte IS NULL
        SET @FechaCorte = CONVERT(DATE, GETDATE());

    SELECT
        @ClaveCliente = clave_cliente
    FROM analitica.dim_cliente
    WHERE id_cliente_crm = @IdCliente
      AND es_activo = 1;

    IF @ClaveCliente IS NULL
        THROW 52801,
              'MAF no existe o no esta activo en analitica.dim_cliente.',
              1;

    IF OBJECT_ID('tempdb..#CarterasMaf') IS NOT NULL
        DROP TABLE #CarterasMaf;

    SELECT DISTINCT
        ca.nId_Cartera AS id_cartera_origen,
        CONVERT(
            VARCHAR(100),
            ca.nId_Cartera
        ) COLLATE Modern_Spanish_CI_AS AS codigo_cartera,
        CONVERT(
            VARCHAR(200),
            LTRIM(RTRIM(ca.cCar_Nombre))
        ) COLLATE Modern_Spanish_CI_AS AS nombre_cartera
    INTO #CarterasMaf
    FROM dbo.av_Cartera AS ca
    WHERE ca.nId_Cliente = @IdCliente
      AND ca.nId_Contrato = @IdContrato
      AND ca.nId_Grupo = @IdGrupo
      AND ca.nAnioCar = YEAR(@FechaCorte)
      AND ca.nCampCar = MONTH(@FechaCorte)
      AND ca.cCiclo IN
      (
          'CICLO 04',
          'CICLO 11',
          'CICLO 18',
          'CICLO 25'
      )
      AND UPPER(ISNULL(ca.cCar_Nombre, ''))
            NOT LIKE '%BORRADOR%';

    IF NOT EXISTS
    (
        SELECT 1
        FROM #CarterasMaf
    )
        THROW 52802,
              'No existen carteras productivas MAF para el mes solicitado.',
              1;

    BEGIN TRY
        IF @AbrioTransaccion = 1
            BEGIN TRANSACTION;

        UPDATE tgt
        SET
            tgt.codigo_cartera = src.codigo_cartera,
            tgt.nombre_cartera = src.nombre_cartera,
            tgt.unidad_negocio_origen = @UnidadNegocio,
            tgt.es_activo = 1,
            tgt.fecha_actualizacion = SYSUTCDATETIME()
        FROM analitica.dim_cartera AS tgt
        INNER JOIN #CarterasMaf AS src
            ON src.id_cartera_origen = tgt.id_cartera_origen
        WHERE tgt.clave_cliente = @ClaveCliente
          AND
          (
                 ISNULL(tgt.codigo_cartera, '')
                    <> ISNULL(src.codigo_cartera, '')
              OR ISNULL(tgt.nombre_cartera, '')
                    <> ISNULL(src.nombre_cartera, '')
              OR ISNULL(tgt.unidad_negocio_origen, '')
                    <> @UnidadNegocio
              OR tgt.es_activo <> 1
          );

        SET @Actualizadas = @@ROWCOUNT;

        INSERT INTO analitica.dim_cartera
        (
            clave_cliente,
            id_cartera_origen,
            codigo_cartera,
            nombre_cartera,
            unidad_negocio_origen
        )
        SELECT
            @ClaveCliente,
            src.id_cartera_origen,
            src.codigo_cartera,
            src.nombre_cartera,
            @UnidadNegocio
        FROM #CarterasMaf AS src
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM analitica.dim_cartera AS tgt
                 WITH (UPDLOCK, HOLDLOCK)
            WHERE tgt.clave_cliente = @ClaveCliente
              AND tgt.id_cartera_origen = src.id_cartera_origen
        );

        SET @Insertadas = @@ROWCOUNT;

        IF @AbrioTransaccion = 1
            COMMIT TRANSACTION;

        IF @MostrarResultado = 1
        BEGIN
            SELECT
                @FechaCorte AS fecha_corte,
                COUNT(*) AS carteras_fuente,
                @Actualizadas AS carteras_actualizadas,
                @Insertadas AS carteras_insertadas
            FROM #CarterasMaf;

            SELECT
                d.clave_cartera,
                d.id_cartera_origen,
                d.codigo_cartera,
                d.nombre_cartera,
                d.unidad_negocio_origen,
                d.es_activo
            FROM analitica.dim_cartera AS d
            INNER JOIN #CarterasMaf AS src
                ON src.id_cartera_origen = d.id_cartera_origen
            WHERE d.clave_cliente = @ClaveCliente
            ORDER BY d.id_cartera_origen;
        END;
    END TRY
    BEGIN CATCH
        IF @AbrioTransaccion = 1
           AND XACT_STATE() <> 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO
