/*
===============================================================================
CLARO PORTFOLIO DIMENSION SYNC - DEFINICION CANONICA VERSIONADA
Procedimiento: integracion_datos_analitica.usp_sincronizar_carteras_claro

Objetivo:
- sincronizar directamente desde dbo.av_Cartera las carteras CLARO del mes;
- no depender de PBI_CLARO_CORP_* para resolver analitica.dim_cartera;
- clasificar GOBIERNO vs ADMINISTRATIVO con la misma regla validada del origen;
- actualizar e insertar de forma idempotente.

Scope validado CLARO:
- cliente CRM: 95
- contrato: 182
- grupo: 156
===============================================================================
*/

USE [aval_cob];
GO

CREATE OR ALTER PROCEDURE
    integracion_datos_analitica.usp_sincronizar_carteras_claro
(
    @FechaCorte DATE = NULL,
    @MostrarResultado BIT = 0
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @IdCliente INT = 95,
        @IdContrato INT = 182,
        @IdGrupo INT = 156,
        @ClaveCliente INT,
        @AbrioTransaccion BIT = CASE WHEN @@TRANCOUNT = 0 THEN 1 ELSE 0 END,
        @Actualizadas INT = 0,
        @Insertadas INT = 0;

    IF @FechaCorte IS NULL
        SET @FechaCorte = CONVERT(DATE, GETDATE());

    SELECT @ClaveCliente = clave_cliente
    FROM analitica.dim_cliente
    WHERE id_cliente_crm = @IdCliente
      AND es_activo = 1;

    IF @ClaveCliente IS NULL
        THROW 53401,
              'CLARO no existe o no esta activo en analitica.dim_cliente.',
              1;

    DROP TABLE IF EXISTS #CarterasClaro;

    SELECT DISTINCT
        ca.nId_Cartera AS id_cartera_origen,
        CONVERT(VARCHAR(100), ca.nId_Cartera)
            COLLATE Modern_Spanish_CI_AS AS codigo_cartera,
        CONVERT(VARCHAR(200), LTRIM(RTRIM(ca.cCar_Nombre)))
            COLLATE Modern_Spanish_CI_AS AS nombre_cartera,
        CONVERT(
            VARCHAR(150),
            CASE
                WHEN LOWER(LTRIM(RTRIM(ISNULL(ca.cCar_Nombre, '')))) LIKE '%gob%'
                    THEN 'CLARO GOBIERNO'
                ELSE 'CLARO ADMINISTRATIVO'
            END
        ) COLLATE Modern_Spanish_CI_AS AS unidad_negocio_origen
    INTO #CarterasClaro
    FROM dbo.av_Cartera AS ca
    WHERE ca.nId_Cliente = @IdCliente
      AND ca.nId_Contrato = @IdContrato
      AND ca.nId_Grupo = @IdGrupo
      AND ca.nAnioCar = YEAR(@FechaCorte)
      AND ca.nCampCar = MONTH(@FechaCorte)
      AND NULLIF(LTRIM(RTRIM(ca.cCar_Nombre)), '') IS NOT NULL;

    IF NOT EXISTS (SELECT 1 FROM #CarterasClaro)
        THROW 53402,
              'No existen carteras CLARO para el mes solicitado.',
              1;

    BEGIN TRY
        IF @AbrioTransaccion = 1
            BEGIN TRANSACTION;

        UPDATE tgt
        SET
            tgt.codigo_cartera = src.codigo_cartera,
            tgt.nombre_cartera = src.nombre_cartera,
            tgt.unidad_negocio_origen = src.unidad_negocio_origen,
            tgt.es_activo = 1,
            tgt.fecha_actualizacion = SYSUTCDATETIME()
        FROM analitica.dim_cartera AS tgt
        INNER JOIN #CarterasClaro AS src
            ON src.id_cartera_origen = tgt.id_cartera_origen
        WHERE tgt.clave_cliente = @ClaveCliente
          AND
          (
                 ISNULL(tgt.codigo_cartera, '') <> ISNULL(src.codigo_cartera, '')
              OR ISNULL(tgt.nombre_cartera, '') <> ISNULL(src.nombre_cartera, '')
              OR ISNULL(tgt.unidad_negocio_origen, '') <> ISNULL(src.unidad_negocio_origen, '')
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
            src.unidad_negocio_origen
        FROM #CarterasClaro AS src
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM analitica.dim_cartera AS tgt WITH (UPDLOCK, HOLDLOCK)
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
                SUM(CASE WHEN unidad_negocio_origen = 'CLARO ADMINISTRATIVO' THEN 1 ELSE 0 END)
                    AS carteras_administrativo,
                SUM(CASE WHEN unidad_negocio_origen = 'CLARO GOBIERNO' THEN 1 ELSE 0 END)
                    AS carteras_gobierno,
                @Actualizadas AS carteras_actualizadas,
                @Insertadas AS carteras_insertadas
            FROM #CarterasClaro;
        END;
    END TRY
    BEGIN CATCH
        IF @AbrioTransaccion = 1 AND XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO
