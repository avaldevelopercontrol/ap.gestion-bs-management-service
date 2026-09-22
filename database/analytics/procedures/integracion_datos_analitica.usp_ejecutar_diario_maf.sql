/*
===============================================================================
MAF DAILY ORCHESTRATOR - DEFINICION CANONICA VERSIONADA
Procedimiento: integracion_datos_analitica.usp_ejecutar_diario_maf

Orden de carga:
1. sincronizar dimensiones de cartera MAF;
2. cargar corte diario de cartera;
3. cargar corte de campaña;
4. cargar promesas;
5. sincronizar promesas detalle -> cartera/dia;
6. cargar producción de asesores/contactos/pagos.

La sincronizacion de carteras corre dentro de la misma transaccion del ETL para
que una nueva cartera no deje hechos parcialmente cargados.
===============================================================================
*/

USE [aval_cob];
GO

CREATE OR ALTER PROCEDURE
    integracion_datos_analitica.usp_ejecutar_diario_maf
(
    @FechaCorte DATE = NULL,
    @MostrarResultado BIT = 0
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @FechaDesdePromesas DATE;
    DECLARE @Intento INT = 1;
    DECLARE @MaxIntentos INT = 3;

    IF @FechaCorte IS NULL
        SET @FechaCorte = CONVERT(DATE, GETDATE());

    IF @FechaCorte > CONVERT(DATE, GETDATE())
        THROW 52300,
              'La fecha de corte MAF no puede estar en el futuro.',
              1;

    SET @FechaDesdePromesas =
        DATEFROMPARTS(YEAR(@FechaCorte), MONTH(@FechaCorte), 1);

    EXEC integracion_datos_analitica.usp_asegurar_rango_fechas
        @FechaCorte,
        @FechaCorte;

    WHILE @Intento <= @MaxIntentos
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            EXEC integracion_datos_analitica.usp_sincronizar_carteras_maf
                @FechaCorte = @FechaCorte,
                @MostrarResultado = 0;

            EXEC integracion_datos_analitica.usp_cargar_corte_diario_cartera_maf
                @FechaCorte = @FechaCorte,
                @Aplicar = 1,
                @ActualizarWatermark = 1,
                @MostrarResultado = @MostrarResultado;

            EXEC integracion_datos_analitica.usp_cargar_corte_campana_maf
                @FechaDesde = @FechaCorte,
                @FechaHasta = @FechaCorte,
                @Aplicar = 1,
                @MostrarResultado = 0;

            EXEC integracion_datos_analitica.usp_cargar_promesas_maf
                @FechaDesde = @FechaDesdePromesas,
                @FechaCorte = @FechaCorte,
                @Aplicar = 1;

            EXEC integracion_datos_analitica.usp_sincronizar_promesas_cartera_diario_maf
                @FechaDesde = @FechaDesdePromesas,
                @FechaHasta = @FechaCorte,
                @MostrarResultado = 0;

            EXEC integracion_datos_analitica.usp_cargar_asesor_diario_maf
                @FechaCorte = @FechaCorte,
                @Aplicar = 1,
                @MostrarResultado = 0;

            COMMIT TRANSACTION;
            BREAK;
        END TRY
        BEGIN CATCH
            DECLARE @NumeroError INT = ERROR_NUMBER();

            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;

            IF @NumeroError = 1205
               AND @Intento < @MaxIntentos
            BEGIN
                SET @Intento += 1;
                WAITFOR DELAY '00:00:02';
                CONTINUE;
            END;

            THROW;
        END CATCH;
    END;

    IF @MostrarResultado = 1
    BEGIN
        DECLARE @ClaveCliente INT;

        SELECT @ClaveCliente = clave_cliente
        FROM analitica.dim_cliente
        WHERE id_cliente_crm = 59;

        SELECT
            @FechaCorte AS fecha_corte,
            COUNT(DISTINCT a.clave_asesor) AS asesores_con_produccion,
            COUNT(DISTINCT s.clave_supervisor) AS supervisores_periodo
        FROM analitica.hecho_asesor_diario AS h
        INNER JOIN analitica.dim_asesor AS a
            ON a.clave_asesor = h.clave_asesor
        LEFT JOIN analitica.v_asignacion_diaria_supervisor_asesor AS s
            ON s.clave_asesor = h.clave_asesor
           AND s.clave_fecha = h.clave_fecha
        WHERE h.clave_cliente = @ClaveCliente
          AND h.clave_campana =
              (
                  SELECT clave_campana
                  FROM analitica.dim_campana
                  WHERE clave_cliente = @ClaveCliente
                    AND anio_campana = YEAR(@FechaCorte)
                    AND mes_campana = MONTH(@FechaCorte)
              );
    END;
END;
GO
