/*
===============================================================================
DEFINICION CANONICA VERSIONADA
Procedimiento: integracion_datos_analitica.usp_cargar_operaciones_actuales_claro

Origen operativo CLARO directo desde aval_cob. Ya no depende de
aval_reporteria.dbo.vw_bi_gerencia_gestiones_pagos ni de
rpt_gestiones_pagos_final para contactos, pagos, promesas y flujo diario.

La semantica fue validada OLD vs DIRECT el 24/09/2026:
- montos de promesa y pago: paridad 100%;
- promesa valida: paridad 100%;
- Pago Sin Promesa: paridad 100%;
- estado PDP: paridad 100% usando la misma fecha logica;
- late arrivals presentes en aval_cob se conservan en lugar de copiar el
  defecto del watermark global de reportería.
===============================================================================
*/
USE [aval_cob];
GO

CREATE OR ALTER PROCEDURE [integracion_datos_analitica].[usp_cargar_operaciones_actuales_claro]      @id_cliente_crm       INT,      @fecha_corte            DATETIME2(3) = NULL,      @anio_campana       SMALLINT = NULL,      @mes_campana      TINYINT = NULL,      @nombre_cliente_origen  VARCHAR(150) = 'CLARO CORPORATIVO',      @codigo_unidad_negocio VARCHAR(20) = 'ADMINISTRATIVO',      @historico_omitir_carteras_sin_mapeo BIT = 0,      @fecha_estado_pdp DATE = NULL  AS  BEGIN      SET NOCOUNT ON;      SET XACT_ABORT ON;          /* ============================================================         0. PARÁMETROS         ============================================================ */        SET @fecha_corte =          ISNULL(@fecha_corte, SYSDATETIME());        SET @anio_campana =          ISNULL(@anio_campana, YEAR(@fecha_corte));        SET @mes_campana =          ISNULL(@mes_campana, MONTH(@fecha_corte));          IF @id_cliente_crm IS NULL         OR @id_cliente_crm <= 0          THROW 51300,              '@id_cliente_crm es obligatorio.',              1;          IF @mes_campana NOT BETWEEN 1 AND 12          THROW 51301,              '@mes_campana debe estar entre 1 y 12.',              1;          SET @codigo_unidad_negocio =              UPPER(LTRIM(RTRIM(ISNULL(@codigo_unidad_negocio, ''))));          IF @codigo_unidad_negocio NOT IN ('ADMINISTRATIVO', 'GOBIERNO')          THROW 51320,              '@codigo_unidad_negocio debe ser ADMINISTRATIVO o GOBIERNO.',              1;          SET @historico_omitir_carteras_sin_mapeo = ISNULL(@historico_omitir_carteras_sin_mapeo, 0);          DECLARE @business_unit_name VARCHAR(150) =              CASE @codigo_unidad_negocio                  WHEN 'ADMINISTRATIVO' THEN 'CLARO ADMINISTRATIVO'                  WHEN 'GOBIERNO' THEN 'CLARO GOBIERNO'              END;          DECLARE @resolved_source_client_name VARCHAR(150) =              CASE @codigo_unidad_negocio                  WHEN 'ADMINISTRATIVO' THEN 'CLARO CORPORATIVO'                  WHEN 'GOBIERNO' THEN 'CLARO GOBIERNO'              END;          DECLARE @watermark_source_code VARCHAR(100) =              CASE @codigo_unidad_negocio                  WHEN 'ADMINISTRATIVO' THEN 'GESTION_COB2_LIVE'                  WHEN 'GOBIERNO' THEN 'CLARO_GOB_LIVE'              END;          /* @nombre_cliente_origen se conserva en la firma por compatibilidad              con callers existentes, pero la business unit es la fuente canonica. */          SET @nombre_cliente_origen = @resolved_source_client_name;          IF NULLIF(LTRIM(RTRIM(@nombre_cliente_origen)), '') IS NULL          THROW 51302,              '@nombre_cliente_origen es obligatorio.',              1;          /* ============================================================         1. CAMPAÑA         ============================================================ */        DECLARE @client_key INT;      DECLARE @campaign_key INT;          DECLARE @codigo_campana VARCHAR(20) =          CONCAT(              @anio_campana,              '-',              RIGHT(CONCAT('0', @mes_campana), 2)          );          DECLARE @source_campaign_code VARCHAR(15) =          CONCAT(              'C-',              RIGHT(CONCAT('0', @mes_campana), 2)          );          DECLARE @campaign_start DATE =          DATEFROMPARTS(              @anio_campana,              @mes_campana,              1          );          DECLARE @campaign_end DATE =          EOMONTH(@campaign_start);          DECLARE @as_of_date DATE =          CAST(@fecha_corte AS DATE);          SET @fecha_estado_pdp = ISNULL(@fecha_estado_pdp, @as_of_date);          IF @fecha_estado_pdp > @as_of_date              THROW 51325, '@fecha_estado_pdp no puede ser posterior a @fecha_corte.', 1;          IF @historico_omitir_carteras_sin_mapeo = 1 AND @codigo_unidad_negocio <> 'GOBIERNO'          THROW 51321,              '@historico_omitir_carteras_sin_mapeo solo está permitido para GOBIERNO.',              1;          IF @historico_omitir_carteras_sin_mapeo = 1 AND @campaign_end >= CONVERT(DATE, SYSDATETIME())          THROW 51322,              '@historico_omitir_carteras_sin_mapeo solo está permitido para campañas cerradas.',              1;          IF @historico_omitir_carteras_sin_mapeo = 1 AND @as_of_date <> @campaign_end          THROW 51323,              '@historico_omitir_carteras_sin_mapeo requiere @fecha_corte en el cierre de campaña.',              1;          DECLARE @end_exclusive DATETIME2(3) =          DATEADD(              DAY,              1,              CONVERT(DATETIME2(3), @as_of_date)          );          SELECT          @client_key = clave_cliente      FROM analitica.dim_cliente      WHERE id_cliente_crm = @id_cliente_crm        AND es_activo = 1;          IF @client_key IS NULL          THROW 51303,              'No existe el cliente activo en analitica.dim_cliente.',              1;          SELECT          @campaign_key = clave_campana      FROM analitica.dim_campana      WHERE clave_cliente = @client_key        AND codigo_campana = @codigo_campana;          IF @campaign_key IS NULL          THROW 51304,              'No existe la campaña en analitica.dim_campana.',              1;          EXEC integracion_datos_analitica.usp_asegurar_rango_fechas          @fecha_desde = @campaign_start,          @fecha_hasta = @campaign_end;
      /* ============================================================
         2-6. ORIGEN DIRECTO CLARO (aval_cob)

         Reemplaza GESTION-COB2 / rpt_gestiones_pagos_final.

         Reglas validadas contra reportería el 24/09/2026:
         - documentos: cDocParam33 -> ImpTotal -> ImpSaldo;
         - pagos: nDoc_ImpParam01;
         - monto promesa: soles + dolares * 3.72;
         - techo de promesa: asignado * 1.05;
         - filtros de usuarios técnicos y deduplicación OPERADOR;
         - adjudicación de pagos y estados PDP;
         - Pago Sin Promesa como fila sintética.

         @fecha_corte controla qué datos entran al corte.
         @fecha_estado_pdp controla la fecha lógica de Vigente/Vence Hoy/Caído.
         ============================================================ */

      DECLARE @tipo_cambio_monto_promesa DECIMAL(18,6) = 3.720000;
      DECLARE @factor_techo_promesa DECIMAL(18,6) = 1.050000;

      IF OBJECT_ID('tempdb..#ClaroCarteras') IS NOT NULL DROP TABLE #ClaroCarteras;

      SELECT
          p.clave_cartera,
          ca.nId_Cartera,
          LTRIM(RTRIM(ca.cCar_Nombre)) COLLATE DATABASE_DEFAULT AS cCar_Nombre,
          ca.nAnioCar,
          ca.nCampCar,
          ca.dFecFinProceso,
          CASE @codigo_unidad_negocio
              WHEN 'GOBIERNO' THEN 'CLARO GOBIERNO'
              ELSE 'CLARO CORPORATIVO'
          END AS cCli_Nombre
      INTO #ClaroCarteras
      FROM dbo.av_Cartera AS ca
      INNER JOIN analitica.dim_cartera AS p
          ON p.clave_cliente = @client_key
         AND p.id_cartera_origen = ca.nId_Cartera
         AND p.unidad_negocio_origen = @business_unit_name
      WHERE ca.nId_Cliente = @id_cliente_crm
        AND ca.nAnioCar = @anio_campana
        AND ca.nCampCar = @mes_campana;

      CREATE UNIQUE CLUSTERED INDEX CX_ClaroCarteras
          ON #ClaroCarteras(nId_Cartera);

      IF NOT EXISTS (SELECT 1 FROM #ClaroCarteras)
          THROW 51308, 'No existen carteras directas CLARO para el scope solicitado.', 1;

      /* ------------------------- DOCUMENTOS / TECHO ------------------------- */
      IF OBJECT_ID('tempdb..#ClaroDocParam') IS NOT NULL DROP TABLE #ClaroDocParam;

      SELECT
          dp.nId_Cartera,
          dp.nId_Cliente,
          dp.nId_DocxCobrar,
          MAX(CONVERT(VARCHAR(500), dp.cDocParam33)) AS cDocParam33
      INTO #ClaroDocParam
      FROM dbo.av_DocxCobrarParam AS dp
      INNER JOIN #ClaroCarteras AS c
          ON c.nId_Cartera = dp.nId_Cartera
      WHERE dp.nId_Cliente = @id_cliente_crm
      GROUP BY dp.nId_Cartera, dp.nId_Cliente, dp.nId_DocxCobrar;

      CREATE UNIQUE CLUSTERED INDEX CX_ClaroDocParam
          ON #ClaroDocParam(nId_Cartera, nId_DocxCobrar);

      IF OBJECT_ID('tempdb..#ClaroDocumentos') IS NOT NULL DROP TABLE #ClaroDocumentos;

      SELECT
          dc.nId_Cartera,
          dc.nId_DocxCobrar,
          dc.nId_PersDeudor,
          CONVERT(
              DECIMAL(38,2),
              COALESCE(
                  NULLIF(
                      TRY_CONVERT(
                          DECIMAL(38,2),
                          NULLIF(LTRIM(RTRIM(dp.cDocParam33)), '')
                      ),
                      0
                  ),
                  NULLIF(CONVERT(DECIMAL(38,2), dc.nDoc_ImpTotal), 0),
                  NULLIF(CONVERT(DECIMAL(38,2), dc.nDoc_ImpSaldo), 0),
                  0
              )
          ) AS importe_asignado
      INTO #ClaroDocumentos
      FROM dbo.av_DocxCobrar AS dc
      INNER JOIN #ClaroCarteras AS c
          ON c.nId_Cartera = dc.nId_Cartera
      INNER JOIN #ClaroDocParam AS dp
          ON dp.nId_Cartera = dc.nId_Cartera
         AND dp.nId_DocxCobrar = dc.nId_DocxCobrar
         AND dp.nId_Cliente = dc.nId_Cliente
      WHERE dc.nId_Cliente = @id_cliente_crm;

      CREATE UNIQUE CLUSTERED INDEX CX_ClaroDocumentos
          ON #ClaroDocumentos(nId_Cartera, nId_DocxCobrar);

      IF OBJECT_ID('tempdb..#ClaroTechoDeudor') IS NOT NULL DROP TABLE #ClaroTechoDeudor;

      SELECT
          d.nId_Cartera,
          d.nId_PersDeudor,
          CONVERT(DECIMAL(38,2), SUM(d.importe_asignado)) AS techo
      INTO #ClaroTechoDeudor
      FROM #ClaroDocumentos AS d
      WHERE d.nId_PersDeudor IS NOT NULL
      GROUP BY d.nId_Cartera, d.nId_PersDeudor;

      CREATE UNIQUE CLUSTERED INDEX CX_ClaroTechoDeudor
          ON #ClaroTechoDeudor(nId_Cartera, nId_PersDeudor);

      /* ------------------------- GESTIONES DIRECTAS ------------------------- */
      IF OBJECT_ID('tempdb..#ClaroGestionesBase') IS NOT NULL DROP TABLE #ClaroGestionesBase;

      SELECT
          c.clave_cartera,
          o.nId_Cartera,
          o.nId_DocxCobrarOpe,
          o.nId_DocxCobrar,
          o.nId_PersDeudor,
          COALESCE(u.nId_Usuario, o.nId_UsuOpe, 0) AS nId_Usuario,
          o.nId_UsuOpe,
          o.tip_gestion,
          CONVERT(DATETIME2(3), o.dDocCobOpe_FecIni) AS dDocCobOpe_FecIni,
          CONVERT(DATETIME2(3), o.dDocCobOpe_FecFin) AS dDocCobOpe_FecFin,
          CONVERT(DATETIME2(3), o.dDoc_FecIngresoGes) AS dDoc_FecIngresoGes,
          CONVERT(DATE, o.dFechCompromisoPago) AS dFechCompromisoPago,
          CONVERT(
              DECIMAL(38,4),
              ISNULL(o.monto_comp, 0)
              + (ISNULL(o.monto_compDolares, 0) * @tipo_cambio_monto_promesa)
          ) AS montoPromesa,
          CONVERT(VARCHAR(50), o.nTelef_Nro) AS nTelef_Nro,
          o.nId_GestionDisp,
          o.nId_OpeCodOut,
          o.nId_OpeCodOutNp2,
          CONVERT(VARCHAR(2000), o.cDocOpeCobOut_Descr) AS cDocOpeCobOut_Descr,
          cod.cNombre_OpeCodCliOut,
          cod.nId_TipoContacto,
          UPPER(LTRIM(RTRIM(ISNULL(tc.indicador_equiv, '')))) AS indicador_equiv,
          UPPER(LTRIM(RTRIM(CONCAT(ISNULL(u.cUsr_ApePat, ''), ' ', ISNULL(u.cUsr_Nombres, ''))))) AS nombre_asesor,
          pf.per_Nombre AS cNombre_Cargo,
          u.nId_PerfilGest,
          c.cCar_Nombre,
          c.cCli_Nombre,
          c.nAnioCar AS anio,
          c.nCampCar,
          CONVERT(BIT, CASE WHEN ISNULL(cod.nId_TipoContacto, 1) = 2 THEN 1 ELSE 0 END) AS marcaPromesa,
          CONVERT(BIT, CASE WHEN ISNULL(o.tip_gestion, 1) = 1 AND u.nId_PerfilGest IS NULL THEN 1 ELSE 0 END) AS marcaCall,
          CONVERT(BIT, CASE WHEN ISNULL(cod.nId_TipoContacto, 1) <= 3 AND u.nId_PerfilGest IS NULL THEN 1 ELSE 0 END) AS marcaCD
      INTO #ClaroGestionesBase
      FROM dbo.av_DocxCobrarOpe AS o
      INNER JOIN #ClaroCarteras AS c
          ON c.nId_Cartera = o.nId_Cartera
      LEFT JOIN dbo.av_Usuario AS u
          ON u.nId_Usuario = o.nId_UsuOpe
      LEFT JOIN dbo.av_Perfil AS pf
          ON pf.nid_perfil = u.nid_perfil
      LEFT JOIN dbo.av_OpeCodCliOut AS cod
          ON cod.nId_Cliente = o.nId_Cliente
         AND cod.nId_OpeCodCliOut = o.nId_OpeCodOut
      LEFT JOIN dbo.av_TipoContacto AS tc
          ON tc.nId_TipoContacto = cod.nId_TipoContacto
      WHERE o.nId_Cliente = @id_cliente_crm;

      CREATE UNIQUE CLUSTERED INDEX CX_ClaroGestionesBase
          ON #ClaroGestionesBase(nId_Cartera, nId_DocxCobrarOpe);

      IF OBJECT_ID('tempdb..#ClaroGestionesElegibles') IS NOT NULL DROP TABLE #ClaroGestionesElegibles;

      SELECT
          g.*,
          HASHBYTES(
              'SHA2_256',
              CONCAT(
                  CONVERT(VARCHAR(20), g.nId_PersDeudor), '|',
                  CONVERT(VARCHAR(20), g.nId_Cartera), '|',
                  CONVERT(VARCHAR(20), g.nId_Usuario), '|',
                  CONVERT(VARCHAR(20), ISNULL(g.nId_UsuOpe, -1)), '|',
                  CONVERT(VARCHAR(20), ISNULL(g.tip_gestion, -1)), '|',
                  CONVERT(VARCHAR(30), g.dDocCobOpe_FecIni, 121), '|',
                  CONVERT(VARCHAR(30), g.dFechCompromisoPago, 121), '|',
                  CONVERT(VARCHAR(50), g.montoPromesa), '|',
                  ISNULL(g.nTelef_Nro, ''), '|',
                  CONVERT(VARCHAR(20), ISNULL(g.nId_GestionDisp, -1)), '|',
                  CONVERT(VARCHAR(20), ISNULL(g.nId_OpeCodOut, -1)), '|',
                  CONVERT(VARCHAR(20), ISNULL(g.nId_OpeCodOutNp2, -1)), '|',
                  ISNULL(g.cNombre_OpeCodCliOut, ''), '|',
                  ISNULL(g.cDocOpeCobOut_Descr, ''), '|',
                  ISNULL(g.indicador_equiv, ''), '|',
                  CONVERT(VARCHAR(5), g.marcaPromesa), '|',
                  CONVERT(VARCHAR(5), g.marcaCall), '|',
                  CONVERT(VARCHAR(5), g.marcaCD), '|OPERADOR'
              )
          ) AS dup_hash
      INTO #ClaroGestionesElegibles
      FROM #ClaroGestionesBase AS g
      WHERE g.nId_PersDeudor IS NOT NULL
        AND g.dDocCobOpe_FecIni IS NOT NULL
        AND ISNULL(g.nId_GestionDisp, -1) <> 4
        AND g.nId_Usuario NOT IN (0, 10214, 10646, 10700, 12454, 12455, 14550, 14771)
        AND ISNULL(g.nId_UsuOpe, 0) NOT IN (0, 10214, 10646, 10700, 12454, 12455, 14550, 14771);

      IF OBJECT_ID('tempdb..#ClaroGestionesDedup') IS NOT NULL DROP TABLE #ClaroGestionesDedup;

      ;WITH r AS
      (
          SELECT
              g.*,
              ROW_NUMBER() OVER
              (
                  PARTITION BY g.nId_Cartera, g.cCar_Nombre, g.nId_PersDeudor, g.dup_hash
                  ORDER BY g.dDoc_FecIngresoGes DESC,
                           g.dDocCobOpe_FecFin DESC,
                           g.nId_DocxCobrarOpe DESC
              ) AS rn
          FROM #ClaroGestionesElegibles AS g
      )
      SELECT *
      INTO #ClaroGestionesDedup
      FROM r
      WHERE rn = 1;

      CREATE UNIQUE CLUSTERED INDEX CX_ClaroGestionesDedup
          ON #ClaroGestionesDedup(nId_Cartera, nId_DocxCobrarOpe);

      IF OBJECT_ID('tempdb..#ClaroGestiones') IS NOT NULL DROP TABLE #ClaroGestiones;

      SELECT
          g.*,
          CONVERT(BIT, CASE
              WHEN g.indicador_equiv = 'CD'
               AND g.montoPromesa > 0
               AND g.dFechCompromisoPago IS NOT NULL
                  THEN 1 ELSE 0 END) AS es_promesa_datos,
          CONVERT(BIT, CASE
              WHEN g.indicador_equiv = 'CD'
               AND g.montoPromesa > 0
               AND g.dFechCompromisoPago IS NOT NULL
               AND CONVERT(DECIMAL(38,2), g.montoPromesa) <= t.techo * @factor_techo_promesa
                  THEN 1 ELSE 0 END) AS es_promesa
      INTO #ClaroGestiones
      FROM #ClaroGestionesDedup AS g
      LEFT JOIN #ClaroTechoDeudor AS t
          ON t.nId_Cartera = g.nId_Cartera
         AND t.nId_PersDeudor = g.nId_PersDeudor;

      CREATE UNIQUE CLUSTERED INDEX CX_ClaroGestiones
          ON #ClaroGestiones(nId_Cartera, nId_DocxCobrarOpe);
      CREATE INDEX IX_ClaroGestiones_Deudor
          ON #ClaroGestiones(nId_Cartera, nId_PersDeudor, dDocCobOpe_FecIni, dFechCompromisoPago)
          INCLUDE (es_promesa, nId_Usuario, montoPromesa);

      /* ------------------------- PAGOS DIRECTOS ------------------------- */
      IF OBJECT_ID('tempdb..#ClaroPagos') IS NOT NULL DROP TABLE #ClaroPagos;

      SELECT
          p.nId_DocxPago,
          p.nId_Cartera,
          p.nId_DocxCobrar,
          p.nId_PersDeudor,
          p.nId_Cliente,
          CONVERT(DATETIME2(3), p.dDoc_FecPago) AS dDoc_FecPago,
          CONVERT(DECIMAL(38,4), p.nDoc_ImpParam01) AS nDoc_ImpPago,
          c.cCar_Nombre,
          c.cCli_Nombre,
          c.nAnioCar AS anio,
          c.nCampCar
      INTO #ClaroPagos
      FROM dbo.av_DocxPago AS p
      INNER JOIN #ClaroCarteras AS c
          ON c.nId_Cartera = p.nId_Cartera
      WHERE p.nId_Cliente = @id_cliente_crm
        AND p.nDoc_ImpPago IS NOT NULL;

      CREATE UNIQUE CLUSTERED INDEX CX_ClaroPagos
          ON #ClaroPagos(nId_Cartera, nId_DocxPago);

      IF OBJECT_ID('tempdb..#ClaroPagoBase') IS NOT NULL DROP TABLE #ClaroPagoBase;

      SELECT
          MIN(p.nId_DocxPago) AS id_unico,
          p.nId_PersDeudor,
          p.nId_Cliente,
          p.nId_Cartera,
          p.dDoc_FecPago,
          SUM(p.nDoc_ImpPago) AS nDoc_ImpPago,
          p.cCar_Nombre,
          MAX(p.cCli_Nombre) AS cCli_Nombre,
          MAX(p.anio) AS anio,
          MAX(p.nCampCar) AS nCampCar
      INTO #ClaroPagoBase
      FROM #ClaroPagos AS p
      WHERE p.nId_PersDeudor IS NOT NULL
        AND p.dDoc_FecPago IS NOT NULL
      GROUP BY p.nId_PersDeudor, p.nId_Cliente, p.nId_Cartera, p.dDoc_FecPago, p.cCar_Nombre;

      CREATE UNIQUE CLUSTERED INDEX CX_ClaroPagoBase ON #ClaroPagoBase(id_unico);

      /* ------------------------- ADJUDICACIÓN PAGO/PROMESA ------------------------- */
      IF OBJECT_ID('tempdb..#ClaroPagosCandidatos') IS NOT NULL DROP TABLE #ClaroPagosCandidatos;

      SELECT
          p.id_unico,
          p.nId_PersDeudor,
          p.nId_Cliente,
          p.nId_Cartera,
          p.dDoc_FecPago,
          p.nDoc_ImpPago,
          p.cCar_Nombre,
          p.cCli_Nombre,
          p.anio,
          p.nCampCar,
          g.nId_DocxCobrarOpe,
          g.nId_Usuario,
          g.dDocCobOpe_FecIni AS fecha_gestion,
          g.dDocCobOpe_FecFin AS fecha_fin_gestion,
          g.dDoc_FecIngresoGes AS fecha_ingreso_gestion,
          g.dFechCompromisoPago AS fecha_promesa,
          g.montoPromesa AS monto_promesa,
          CASE WHEN CONVERT(DATE, p.dDoc_FecPago) <= g.dFechCompromisoPago THEN 'cumplido' ELSE 'fuera_rango' END AS estado_candidato_pago,
          CASE WHEN CONVERT(DATE, p.dDoc_FecPago) <= g.dFechCompromisoPago THEN 1 ELSE 2 END AS estado_rank
      INTO #ClaroPagosCandidatos
      FROM #ClaroPagoBase AS p
      INNER JOIN #ClaroGestiones AS g
          ON g.nId_Cartera = p.nId_Cartera
         AND g.nId_PersDeudor = p.nId_PersDeudor
         AND g.es_promesa = 1
         AND g.dFechCompromisoPago >= CONVERT(DATE, g.dDocCobOpe_FecIni)
         AND CONVERT(DATE, p.dDoc_FecPago) >= CONVERT(DATE, g.dDocCobOpe_FecIni);

      IF OBJECT_ID('tempdb..#ClaroPagosAsignados') IS NOT NULL DROP TABLE #ClaroPagosAsignados;

      ;WITH Asesores AS
      (
          SELECT id_unico, nId_PersDeudor, nId_Cartera, dDoc_FecPago, estado_candidato_pago,
                 COUNT(DISTINCT nId_Usuario) AS cantidad_asesores
          FROM #ClaroPagosCandidatos
          GROUP BY id_unico, nId_PersDeudor, nId_Cartera, dDoc_FecPago, estado_candidato_pago
      ),
      Rankeados AS
      (
          SELECT
              p.*,
              a.cantidad_asesores,
              ROW_NUMBER() OVER
              (
                  PARTITION BY p.id_unico, p.nId_PersDeudor, p.nId_Cartera, p.dDoc_FecPago
                  ORDER BY
                      p.estado_rank,
                      CASE WHEN a.cantidad_asesores = 1 THEN p.fecha_promesa END DESC,
                      CASE WHEN a.cantidad_asesores = 1 THEN p.fecha_gestion END DESC,
                      CASE WHEN a.cantidad_asesores = 1 THEN p.nId_DocxCobrarOpe END DESC,
                      CASE WHEN a.cantidad_asesores > 1 THEN p.fecha_promesa END ASC,
                      CASE WHEN a.cantidad_asesores > 1 THEN p.fecha_gestion END ASC,
                      CASE WHEN a.cantidad_asesores > 1 THEN p.nId_DocxCobrarOpe END ASC
              ) AS rn
          FROM #ClaroPagosCandidatos AS p
          INNER JOIN Asesores AS a
              ON a.id_unico = p.id_unico
             AND a.nId_PersDeudor = p.nId_PersDeudor
             AND a.nId_Cartera = p.nId_Cartera
             AND a.dDoc_FecPago = p.dDoc_FecPago
             AND a.estado_candidato_pago = p.estado_candidato_pago
      )
      SELECT *
      INTO #ClaroPagosAsignados
      FROM Rankeados
      WHERE rn = 1;

      CREATE INDEX IX_ClaroPagosAsignados_Pago ON #ClaroPagosAsignados(id_unico);
      CREATE INDEX IX_ClaroPagosAsignados_Promesa ON #ClaroPagosAsignados(nId_DocxCobrarOpe);

      IF OBJECT_ID('tempdb..#ClaroPromesasNoAdjudicadas') IS NOT NULL DROP TABLE #ClaroPromesasNoAdjudicadas;

      SELECT p.nId_DocxCobrarOpe, COUNT(*) AS cantidad_competencias
      INTO #ClaroPromesasNoAdjudicadas
      FROM #ClaroPagosCandidatos AS p
      LEFT JOIN #ClaroPagosAsignados AS a
          ON a.id_unico = p.id_unico
         AND a.nId_DocxCobrarOpe = p.nId_DocxCobrarOpe
      WHERE a.id_unico IS NULL
      GROUP BY p.nId_DocxCobrarOpe;

      CREATE UNIQUE CLUSTERED INDEX CX_ClaroPromesasNoAdjudicadas
          ON #ClaroPromesasNoAdjudicadas(nId_DocxCobrarOpe);

      IF OBJECT_ID('tempdb..#ClaroPagosPorPromesa') IS NOT NULL DROP TABLE #ClaroPagosPorPromesa;

      SELECT
          nId_DocxCobrarOpe,
          COUNT(*) AS cantidad_pagos,
          SUM(nDoc_ImpPago) AS total_pagado,
          MIN(dDoc_FecPago) AS primera_fecha_pago,
          MAX(dDoc_FecPago) AS ultima_fecha_pago,
          CASE
              WHEN MIN(CASE WHEN estado_candidato_pago = 'cumplido' THEN 1 ELSE 2 END) = 1
               AND SUM(ISNULL(nDoc_ImpPago, 0)) >= ISNULL(MAX(monto_promesa), 0)
                  THEN '3. Cumplio'
              WHEN MIN(CASE WHEN estado_candidato_pago = 'cumplido' THEN 1 ELSE 2 END) = 1
               AND SUM(ISNULL(nDoc_ImpPago, 0)) < ISNULL(MAX(monto_promesa), 0)
                  THEN '4. Cumplio parcial'
              ELSE '5. Cumplio Fuera Rango'
          END AS estado_pdp
      INTO #ClaroPagosPorPromesa
      FROM #ClaroPagosAsignados
      GROUP BY nId_DocxCobrarOpe;

      CREATE UNIQUE CLUSTERED INDEX CX_ClaroPagosPorPromesa
          ON #ClaroPagosPorPromesa(nId_DocxCobrarOpe);

      /* ------------------------- CONTRATO #Live ------------------------- */
      IF OBJECT_ID('tempdb..#Live') IS NOT NULL DROP TABLE #Live;

      CREATE TABLE #Live
      (
          clave_cartera INT NOT NULL,
          id_cartera_origen INT NOT NULL,
          nombre_cartera VARCHAR(150) NULL,
          ultima_fecha_pago DATETIME2(3) NULL,
          ultima_fecha_registro DATETIME2(3) NULL,
          cCli_Nombre VARCHAR(150) NULL,
          cCar_Nombre VARCHAR(150) NULL,
          nCampCar INT NULL,
          nombre_asesor VARCHAR(150) NULL,
          anio INT NULL,
          estado_pdp VARCHAR(100) NULL,
          id_deudor_origen BIGINT NULL,
          contact_code VARCHAR(100) NULL,
          monto_promesa DECIMAL(19,4) NOT NULL,
          monto_pagado DECIMAL(19,4) NOT NULL,
          source_valid_promise BIT NOT NULL,
          is_payment_only_row BIT NOT NULL,
          cNombre_Cargo VARCHAR(150) NULL,
          fecha_vencimiento_promesa DATE NULL,
          fecha_hora_gestion DATETIME2(3) NULL,
          management_date DATE NULL,
          clave_fecha INT NULL,
          id_operacion_origen BIGINT NULL,
          detalle_gestion VARCHAR(500) NULL
      );

      DECLARE @direct_cartera_id INT,
              @direct_clave_cartera INT,
              @direct_fecha_max_pago DATE,
              @direct_fecha_fin_proceso DATE;

      DECLARE cur_claro_direct CURSOR LOCAL FAST_FORWARD FOR
      SELECT nId_Cartera, clave_cartera
      FROM #ClaroCarteras
      ORDER BY nId_Cartera;

      OPEN cur_claro_direct;
      FETCH NEXT FROM cur_claro_direct INTO @direct_cartera_id, @direct_clave_cartera;

      WHILE @@FETCH_STATUS = 0
      BEGIN
          SELECT @direct_fecha_max_pago = MAX(CONVERT(DATE, p.dDoc_FecPago))
          FROM #ClaroPagos AS p
          WHERE p.nId_Cartera = @direct_cartera_id;

          SELECT @direct_fecha_fin_proceso = MAX(CONVERT(DATE, c.dFecFinProceso))
          FROM #ClaroCarteras AS c
          WHERE c.nId_Cartera = @direct_cartera_id;

          INSERT INTO #Live
          (
              clave_cartera, id_cartera_origen, nombre_cartera,
              ultima_fecha_pago, ultima_fecha_registro,
              cCli_Nombre, cCar_Nombre, nCampCar, nombre_asesor, anio, estado_pdp,
              id_deudor_origen, contact_code, monto_promesa, monto_pagado,
              source_valid_promise, is_payment_only_row, cNombre_Cargo,
              fecha_vencimiento_promesa, fecha_hora_gestion, management_date,
              clave_fecha, id_operacion_origen, detalle_gestion
          )
          SELECT
              g.clave_cartera,
              g.nId_Cartera,
              g.cCar_Nombre,
              pp.ultima_fecha_pago,
              g.dDoc_FecIngresoGes,
              g.cCli_Nombre,
              g.cCar_Nombre,
              g.nCampCar,
              g.nombre_asesor,
              g.anio,
              CASE
                  WHEN pp.nId_DocxCobrarOpe IS NOT NULL THEN pp.estado_pdp
                  WHEN pna.nId_DocxCobrarOpe IS NOT NULL THEN '8. Caido'
                  WHEN g.es_promesa = 1 AND g.dFechCompromisoPago > @fecha_estado_pdp THEN '1. Vigente'
                  WHEN g.es_promesa = 1 AND g.dFechCompromisoPago = @fecha_estado_pdp THEN '2. Vence Hoy'
                  WHEN g.es_promesa = 1
                   AND g.dFechCompromisoPago < @fecha_estado_pdp
                   AND (
                       (@direct_fecha_fin_proceso IS NOT NULL AND @fecha_estado_pdp > @direct_fecha_fin_proceso)
                       OR (@direct_fecha_max_pago IS NOT NULL AND @direct_fecha_max_pago > g.dFechCompromisoPago)
                   ) THEN '8. Caido'
                  WHEN g.es_promesa = 1 AND g.dFechCompromisoPago < @fecha_estado_pdp THEN '7. Por Confirmar'
                  ELSE '9. No PdP y No Pagos'
              END,
              CONVERT(BIGINT, g.nId_PersDeudor),
              g.indicador_equiv,
              CONVERT(DECIMAL(19,4), ISNULL(g.montoPromesa, 0)),
              CONVERT(DECIMAL(19,4), ISNULL(pp.total_pagado, 0)),
              g.es_promesa,
              CONVERT(BIT, 0),
              g.cNombre_Cargo,
              g.dFechCompromisoPago,
              g.dDocCobOpe_FecIni,
              CONVERT(DATE, g.dDocCobOpe_FecIni),
              CONVERT(INT, CONVERT(CHAR(8), CONVERT(DATE, g.dDocCobOpe_FecIni), 112)),
              CONVERT(BIGINT, g.nId_DocxCobrarOpe),
              g.cNombre_OpeCodCliOut
          FROM #ClaroGestiones AS g
          LEFT JOIN #ClaroPagosPorPromesa AS pp
              ON pp.nId_DocxCobrarOpe = g.nId_DocxCobrarOpe
          LEFT JOIN #ClaroPromesasNoAdjudicadas AS pna
              ON pna.nId_DocxCobrarOpe = g.nId_DocxCobrarOpe
          WHERE g.nId_Cartera = @direct_cartera_id
            AND g.dDocCobOpe_FecIni >= @campaign_start
            AND g.dDocCobOpe_FecIni < @end_exclusive
            AND NOT
            (
                g.es_promesa_datos = 0
                AND pp.nId_DocxCobrarOpe IS NULL
                AND (ISNULL(g.montoPromesa, 0) > 0 OR g.dFechCompromisoPago IS NOT NULL)
            );

          INSERT INTO #Live
          (
              clave_cartera, id_cartera_origen, nombre_cartera,
              ultima_fecha_pago, ultima_fecha_registro,
              cCli_Nombre, cCar_Nombre, nCampCar, nombre_asesor, anio, estado_pdp,
              id_deudor_origen, contact_code, monto_promesa, monto_pagado,
              source_valid_promise, is_payment_only_row, cNombre_Cargo,
              fecha_vencimiento_promesa, fecha_hora_gestion, management_date,
              clave_fecha, id_operacion_origen, detalle_gestion
          )
          SELECT
              c.clave_cartera,
              p.nId_Cartera,
              p.cCar_Nombre,
              p.dDoc_FecPago,
              p.dDoc_FecPago,
              p.cCli_Nombre,
              p.cCar_Nombre,
              p.nCampCar,
              'BOLSA',
              p.anio,
              '6. Pago Sin Promesa',
              CONVERT(BIGINT, p.nId_PersDeudor),
              'NC',
              CONVERT(DECIMAL(19,4), 0),
              CONVERT(DECIMAL(19,4), ISNULL(p.nDoc_ImpPago, 0)),
              CONVERT(BIT, 0),
              CONVERT(BIT, 1),
              'BOLSA',
              NULL,
              p.dDoc_FecPago,
              CONVERT(DATE, p.dDoc_FecPago),
              CONVERT(INT, CONVERT(CHAR(8), CONVERT(DATE, p.dDoc_FecPago), 112)),
              NULL,
              NULL
          FROM #ClaroPagoBase AS p
          INNER JOIN #ClaroCarteras AS c
              ON c.nId_Cartera = p.nId_Cartera
          LEFT JOIN #ClaroPagosAsignados AS a
              ON a.id_unico = p.id_unico
          WHERE p.nId_Cartera = @direct_cartera_id
            AND a.id_unico IS NULL
            AND p.dDoc_FecPago >= @campaign_start
            AND p.dDoc_FecPago < @end_exclusive;

          FETCH NEXT FROM cur_claro_direct INTO @direct_cartera_id, @direct_clave_cartera;
      END;

      CLOSE cur_claro_direct;
      DEALLOCATE cur_claro_direct;

      CREATE INDEX IX_Live_DatePortfolio ON #Live(clave_fecha, clave_cartera);
      CREATE INDEX IX_Live_Debtor ON #Live(clave_cartera, id_deudor_origen, management_date);
      CREATE INDEX IX_Live_Operation ON #Live(id_operacion_origen);

      DECLARE @source_rows BIGINT;
      DECLARE @source_as_of_at DATETIME2(3);
      DECLARE @last_source_id BIGINT;

      SELECT
          @source_rows = COUNT_BIG(*),
          @source_as_of_at = MAX(ultima_fecha_registro),
          @last_source_id = MAX(id_operacion_origen)
      FROM #Live;

      IF ISNULL(@source_rows, 0) = 0
          THROW 51305, 'El origen directo CLARO no devolvió filas para el scope solicitado.', 1;

/* ============================================================         7. GUARDAS DE IDENTIFICADORES         ============================================================ */        DECLARE @promise_rows_without_id BIGINT;      DECLARE @ignored_non_promise_without_id BIGINT;          /*         Dos estados NO pertenecen al dominio de promesas:           - "9. No PdP y No Pagos"         - "6. Pago Sin Promesa"           Pago Sin Promesa:         - aporta a pago/recaudo;         - aporta a pagador;         - NO aporta a gestión;         - NO aporta a contacto;         - NO aporta a promise_fact.      */        SELECT          @ignored_non_promise_without_id =              COUNT_BIG(*)        FROM #Live        WHERE id_operacion_origen IS NULL          AND source_valid_promise = 0          AND fecha_vencimiento_promesa IS NULL          AND        (            UPPER(ISNULL(estado_pdp, ''))                LIKE '%NO PDP%'              OR is_payment_only_row = 1        );          SELECT          @promise_rows_without_id =              COUNT_BIG(*)        FROM #Live        WHERE id_operacion_origen IS NULL          AND UPPER(ISNULL(estado_pdp, ''))                NOT LIKE '%NO PDP%'          AND is_payment_only_row = 0          AND        (            source_valid_promise = 1            OR fecha_vencimiento_promesa IS NOT NULL            OR monto_promesa > 0            OR NULLIF(                  LTRIM(RTRIM(estado_pdp)),                  ''               ) IS NOT NULL        );          IF @promise_rows_without_id > 0          THROW 51306,              'Existen filas PDP/promesa reales sin nId_DocxCobrarOpe; revisar la fuente antes de cargar.',              1;          DECLARE @payer_rows_without_debtor_id BIGINT;          SELECT          @payer_rows_without_debtor_id =              COUNT_BIG(*)        FROM #Live        WHERE monto_pagado > 0        AND id_deudor_origen IS NULL;          IF @payer_rows_without_debtor_id > 0          THROW 51307,              'Existen filas con pago sin nId_PersDeudor; no se puede materializar el pagador Portfolio de forma estable.',              1;          /* ============================================================         8. CONTACTO A GRAIN DEUDOR / DÍA         ============================================================ */        IF OBJECT_ID('tempdb..#ContactDaily') IS NOT NULL          DROP TABLE #ContactDaily;          SELECT          l.clave_fecha,          l.clave_cartera,          l.id_deudor_origen,            CONVERT(              BIT,              MAX(                  CASE                      WHEN l.contact_code = 'CD'                          THEN 1                      ELSE 0                  END              )          ) AS indicador_contacto_directo,            CONVERT(              BIT,              MAX(                  CASE                      WHEN l.contact_code = 'CI'                          THEN 1                      ELSE 0                  END              )          ) AS indicador_contacto_indirecto,            CONVERT(              BIT,              MAX(                  CASE                      WHEN l.contact_code = 'NC'                          THEN 1                      ELSE 0                  END              )          ) AS indicador_sin_contacto        INTO #ContactDaily        FROM #Live AS l        WHERE l.is_payment_only_row = 0        GROUP BY          l.clave_fecha,          l.clave_cartera,          l.id_deudor_origen;          /* ============================================================         9. PAGADOR PORTFOLIO A GRAIN DEUDOR / DÍA           Incluye Pago Sin Promesa.         ============================================================ */        IF OBJECT_ID('tempdb..#DebtorPayerDaily') IS NOT NULL          DROP TABLE #DebtorPayerDaily;          SELECT          l.clave_fecha,          l.clave_cartera,          l.id_deudor_origen        INTO #DebtorPayerDaily        FROM #Live AS l        WHERE l.monto_pagado > 0        GROUP BY          l.clave_fecha,          l.clave_cartera,          l.id_deudor_origen;          /* ============================================================         10. PRIMERA GESTIÓN / PRIMER CD         ============================================================ */        IF OBJECT_ID('tempdb..#FirstManaged') IS NOT NULL          DROP TABLE #FirstManaged;          SELECT          clave_cartera,          id_deudor_origen,            MIN(management_date)              AS first_management_date        INTO #FirstManaged        FROM #Live        WHERE is_payment_only_row = 0        GROUP BY          clave_cartera,          id_deudor_origen;          IF OBJECT_ID('tempdb..#FirstDirect') IS NOT NULL          DROP TABLE #FirstDirect;          SELECT          clave_cartera,          id_deudor_origen,            MIN(management_date)              AS first_direct_date        INTO #FirstDirect        FROM #Live        WHERE is_payment_only_row = 0        AND contact_code = 'CD'        GROUP BY          clave_cartera,          id_deudor_origen;          /* ============================================================         11. FLOW DIARIO POR CARTERA         ============================================================ */        IF OBJECT_ID('tempdb..#DailyFlow') IS NOT NULL          DROP TABLE #DailyFlow;          ;WITH Base AS      (          SELECT              l.clave_fecha,              l.management_date,              l.clave_cartera,                SUM(                  CASE                      WHEN l.is_payment_only_row = 0                          THEN 1                      ELSE 0                  END              ) AS eventos_gestion_dia,                SUM(                  CASE                      WHEN l.source_valid_promise = 1                       AND l.monto_promesa > 0                       AND UPPER(ISNULL(l.estado_pdp, ''))                              NOT LIKE '%NO PDP%'                          THEN 1                        ELSE 0                  END              ) AS cantidad_promesas_dia,                SUM(                  CASE                      WHEN l.source_valid_promise = 1                       AND l.monto_promesa > 0                       AND UPPER(ISNULL(l.estado_pdp, ''))                              NOT LIKE '%NO PDP%'                          THEN l.monto_promesa                        ELSE CONVERT(DECIMAL(19,4), 0)                  END              ) AS monto_promesas_dia,                SUM(l.monto_pagado)                  AS monto_recuperado_dia            FROM #Live AS l            GROUP BY              l.clave_fecha,              l.management_date,              l.clave_cartera      ),        Payers AS      (          SELECT              clave_fecha,              clave_cartera,                COUNT_BIG(*)                  AS cantidad_pagadores_dia            FROM #DebtorPayerDaily            GROUP BY              clave_fecha,              clave_cartera      ),        NewManaged AS      (          SELECT              CONVERT(                  INT,                  CONVERT(                      CHAR(8),                      first_management_date,                      112                  )              ) AS clave_fecha,                clave_cartera,                COUNT_BIG(*)                  AS nuevos_clientes_gestionados_dia            FROM #FirstManaged            GROUP BY              first_management_date,              clave_cartera      ),        NewDirect AS      (          SELECT              CONVERT(                  INT,                  CONVERT(                      CHAR(8),                      first_direct_date,                      112                  )              ) AS clave_fecha,                clave_cartera,                COUNT_BIG(*)                  AS nuevos_contactos_directos_dia            FROM #FirstDirect            GROUP BY              first_direct_date,              clave_cartera      )        SELECT          b.clave_fecha,          b.clave_cartera,            CONVERT(              INT,              b.eventos_gestion_dia          ) AS eventos_gestion_dia,            CONVERT(              INT,              ISNULL(                  nm.nuevos_clientes_gestionados_dia,                  0              )          ) AS nuevos_clientes_gestionados_dia,            CONVERT(              INT,              ISNULL(                  nd.nuevos_contactos_directos_dia,                  0              )          ) AS nuevos_contactos_directos_dia,            CONVERT(              INT,              b.cantidad_promesas_dia          ) AS cantidad_promesas_dia,            CONVERT(              DECIMAL(19,4),              b.monto_promesas_dia          ) AS monto_promesas_dia,            CONVERT(              INT,              ISNULL(                  py.cantidad_pagadores_dia,                  0              )          ) AS cantidad_pagadores_dia,            CONVERT(              DECIMAL(19,4),              b.monto_recuperado_dia          ) AS monto_recuperado_dia        INTO #DailyFlow        FROM Base AS b        LEFT JOIN Payers AS py          ON py.clave_fecha = b.clave_fecha         AND py.clave_cartera = b.clave_cartera        LEFT JOIN NewManaged AS nm          ON nm.clave_fecha = b.clave_fecha         AND nm.clave_cartera = b.clave_cartera        LEFT JOIN NewDirect AS nd          ON nd.clave_fecha = b.clave_fecha         AND nd.clave_cartera = b.clave_cartera;          /* ============================================================         12. STAGE DE PROMESAS         ============================================================ */        IF OBJECT_ID('tempdb..#PromiseStage') IS NOT NULL          DROP TABLE #PromiseStage;          ;WITH PromiseLike AS      (          SELECT              l.*,                CASE                  WHEN UPPER(ISNULL(l.estado_pdp, ''))                          LIKE '%VENCE HOY%'                      THEN 'DUE_TODAY'                    WHEN UPPER(ISNULL(l.estado_pdp, ''))                          LIKE '%CUMPLIO FUERA RANGO%'                      THEN 'FULFILLED_OUT_OF_RANGE'                    WHEN UPPER(ISNULL(l.estado_pdp, ''))                          LIKE '%CUMPLIO PARCIAL%'                      THEN 'PARTIAL'                    WHEN UPPER(ISNULL(l.estado_pdp, ''))                          LIKE '%CUMPLIO%'                      THEN 'FULFILLED'                    WHEN UPPER(ISNULL(l.estado_pdp, ''))                          LIKE '%POR CONFIRMAR%'                      THEN 'PENDING_CONFIRMATION'                    WHEN UPPER(ISNULL(l.estado_pdp, ''))                          LIKE '%CAIDO%'                      THEN 'BROKEN'                    WHEN UPPER(ISNULL(l.estado_pdp, ''))                          LIKE '%NO PDP%'                      THEN 'NO_PROMISE_NO_PAYMENT'                    WHEN UPPER(ISNULL(l.estado_pdp, ''))                          LIKE '%VIGENTE%'                      THEN 'ACTIVE'                    ELSE 'UNKNOWN'              END AS codigo_estado,                CONVERT(                  BIT,                  CASE                      WHEN l.source_valid_promise = 1                       AND l.monto_promesa > 0                       AND UPPER(ISNULL(l.estado_pdp, ''))                              NOT LIKE '%NO PDP%'                          THEN 1                        ELSE 0                  END              ) AS es_promesa_valida,                ROW_NUMBER() OVER              (                  PARTITION BY l.id_operacion_origen                    ORDER BY                      l.ultima_fecha_registro DESC,                      l.fecha_hora_gestion DESC              ) AS rn            FROM #Live AS l            WHERE l.id_operacion_origen IS NOT NULL              AND UPPER(ISNULL(l.estado_pdp, ''))                    NOT LIKE '%NO PDP%'              AND l.is_payment_only_row = 0              AND            (                l.source_valid_promise = 1                OR l.fecha_vencimiento_promesa IS NOT NULL                OR l.monto_promesa > 0                OR NULLIF(                      LTRIM(RTRIM(l.estado_pdp)),                      ''                   ) IS NOT NULL            )      )        SELECT          id_operacion_origen,          id_deudor_origen,          clave_cartera,          fecha_hora_gestion,          fecha_vencimiento_promesa,          monto_promesa,          monto_pagado,            CONVERT(              DATETIME2(3),              ultima_fecha_pago          ) AS fecha_ultimo_pago,            estado_pdp AS estado_origen,          codigo_estado,          es_promesa_valida,            CONVERT(              DATETIME2(3),              ultima_fecha_registro          ) AS fecha_actualizacion_origen        INTO #PromiseStage        FROM PromiseLike        WHERE rn = 1;          /* ============================================================         13. ESCRITURA ANALYTICS         ============================================================ */        BEGIN TRY            BEGIN TRANSACTION;              /* --------------------------------------------------------             13.1 Contacto debtor/day             -------------------------------------------------------- */            UPDATE f          SET              f.indicador_contacto_directo =                  s.indicador_contacto_directo,                f.indicador_contacto_indirecto =                  s.indicador_contacto_indirecto,                f.indicador_sin_contacto =                  s.indicador_sin_contacto,                f.fecha_corte_origen =                  @source_as_of_at,                f.fecha_carga =                  SYSUTCDATETIME()            FROM analitica.hecho_contacto_deudor_diario AS f            INNER JOIN #ContactDaily AS s              ON s.clave_fecha = f.clave_fecha             AND s.clave_cartera = f.clave_cartera             AND s.id_deudor_origen = f.id_deudor_origen            WHERE f.clave_cliente = @client_key            AND f.clave_campana = @campaign_key;              INSERT INTO analitica.hecho_contacto_deudor_diario          (              clave_fecha,              clave_cliente,              clave_campana,              clave_cartera,              id_deudor_origen,                indicador_contacto_directo,              indicador_contacto_indirecto,              indicador_sin_contacto,                fecha_corte_origen          )            SELECT              s.clave_fecha,              @client_key,              @campaign_key,              s.clave_cartera,              s.id_deudor_origen,                s.indicador_contacto_directo,              s.indicador_contacto_indirecto,              s.indicador_sin_contacto,                @source_as_of_at            FROM #ContactDaily AS s            WHERE NOT EXISTS          (              SELECT 1                FROM analitica.hecho_contacto_deudor_diario AS f                WHERE f.clave_fecha = s.clave_fecha                AND f.clave_cliente = @client_key                AND f.clave_campana = @campaign_key                AND f.clave_cartera = s.clave_cartera                AND f.id_deudor_origen = s.id_deudor_origen          );              DELETE f            FROM analitica.hecho_contacto_deudor_diario AS f            INNER JOIN analitica.dim_fecha AS d              ON d.clave_fecha = f.clave_fecha            WHERE f.clave_cliente = @client_key            AND f.clave_campana = @campaign_key              AND d.fecha_calendario >= @campaign_start            AND d.fecha_calendario <= @as_of_date              AND EXISTS              (                  SELECT 1                  FROM analitica.dim_cartera AS bp                  WHERE bp.clave_cartera = f.clave_cartera                    AND bp.clave_cliente = @client_key                    AND bp.unidad_negocio_origen = @business_unit_name              )              AND NOT EXISTS            (                SELECT 1                  FROM #ContactDaily AS s                  WHERE s.clave_fecha = f.clave_fecha                  AND s.clave_cartera = f.clave_cartera                  AND s.id_deudor_origen = f.id_deudor_origen            );              /* --------------------------------------------------------             13.2 Pagador debtor/day             -------------------------------------------------------- */            UPDATE f          SET              f.fecha_corte_origen =                  @source_as_of_at,                f.fecha_carga =                  SYSUTCDATETIME()            FROM analitica.hecho_pago_deudor_diario AS f            INNER JOIN #DebtorPayerDaily AS s              ON s.clave_fecha = f.clave_fecha             AND s.clave_cartera = f.clave_cartera             AND s.id_deudor_origen = f.id_deudor_origen            WHERE f.clave_cliente = @client_key            AND f.clave_campana = @campaign_key;              INSERT INTO analitica.hecho_pago_deudor_diario          (              clave_fecha,              clave_cliente,              clave_campana,              clave_cartera,              id_deudor_origen,              fecha_corte_origen          )            SELECT              s.clave_fecha,              @client_key,              @campaign_key,              s.clave_cartera,              s.id_deudor_origen,              @source_as_of_at            FROM #DebtorPayerDaily AS s            WHERE NOT EXISTS          (              SELECT 1                FROM analitica.hecho_pago_deudor_diario AS f                WHERE f.clave_fecha = s.clave_fecha                AND f.clave_cliente = @client_key                AND f.clave_campana = @campaign_key                AND f.clave_cartera = s.clave_cartera                AND f.id_deudor_origen = s.id_deudor_origen          );              DELETE f            FROM analitica.hecho_pago_deudor_diario AS f            INNER JOIN analitica.dim_fecha AS d              ON d.clave_fecha = f.clave_fecha            WHERE f.clave_cliente = @client_key            AND f.clave_campana = @campaign_key              AND d.fecha_calendario >= @campaign_start            AND d.fecha_calendario <= @as_of_date              AND EXISTS              (                  SELECT 1                  FROM analitica.dim_cartera AS bp                  WHERE bp.clave_cartera = f.clave_cartera                    AND bp.clave_cliente = @client_key                    AND bp.unidad_negocio_origen = @business_unit_name              )              AND NOT EXISTS            (                SELECT 1                  FROM #DebtorPayerDaily AS s                  WHERE s.clave_fecha = f.clave_fecha                  AND s.clave_cartera = f.clave_cartera                  AND s.id_deudor_origen = f.id_deudor_origen            );              /* --------------------------------------------------------             13.3 Filas fact_portfolio_daily requeridas               CAMBIO:             Antes se agregaban TODAS las dim_portfolio es_activo = 1.               Eso podía volver a crear una cartera residual como 34364.               Ahora solamente se agregan:               - filas requeridas por flows reales;               - carteras resolubles del scope de ESTA campaña.             -------------------------------------------------------- */            IF OBJECT_ID('tempdb..#RequiredFactRows') IS NOT NULL              DROP TABLE #RequiredFactRows;              SELECT DISTINCT              clave_fecha,              clave_cartera            INTO #RequiredFactRows            FROM #DailyFlow              UNION              SELECT              CONVERT(                  INT,                  CONVERT(                      CHAR(8),                      @as_of_date,                      112                  )              ),                p.clave_cartera            FROM #CampaignPortfolioMap AS p;              INSERT INTO analitica.hecho_cartera_diario          (              clave_fecha,              clave_cliente,              clave_campana,              clave_cartera,                clientes_asignados_corte,              clientes_gestionados_corte,              clientes_pendientes_corte,              clientes_contactados_corte,              contactos_directos_corte,                monto_asignado_corte,              monto_gestionado_corte,                tiene_corte_origen,              fecha_corte_origen          )            SELECT              r.clave_fecha,              @client_key,              @campaign_key,              r.clave_cartera,                ISNULL(                  prev.clientes_asignados_corte,                  0              ),                ISNULL(                  prev.clientes_gestionados_corte,                  0              ),                ISNULL(                  prev.clientes_pendientes_corte,                  0              ),                ISNULL(                  prev.clientes_contactados_corte,                  0              ),                ISNULL(                  prev.contactos_directos_corte,                  0              ),                ISNULL(                  prev.monto_asignado_corte,                  0              ),                ISNULL(                  prev.monto_gestionado_corte,                  0              ),                0,                prev.fecha_corte_origen            FROM #RequiredFactRows AS r            OUTER APPLY          (              SELECT TOP (1)                  f.clientes_asignados_corte,                  f.clientes_gestionados_corte,                  f.clientes_pendientes_corte,                  f.clientes_contactados_corte,                  f.contactos_directos_corte,                    f.monto_asignado_corte,                  f.monto_gestionado_corte,                    f.fecha_corte_origen                FROM analitica.hecho_cartera_diario AS f                WHERE f.clave_cliente = @client_key                AND f.clave_campana = @campaign_key                AND f.clave_cartera = r.clave_cartera                AND f.clave_fecha < r.clave_fecha                ORDER BY                  f.clave_fecha DESC            ) AS prev            WHERE NOT EXISTS          (              SELECT 1                FROM analitica.hecho_cartera_diario AS f                WHERE f.clave_fecha = r.clave_fecha                AND f.clave_cliente = @client_key                AND f.clave_campana = @campaign_key                AND f.clave_cartera = r.clave_cartera          );              /* --------------------------------------------------------             Reset completo de flows del rango MTD             -------------------------------------------------------- */            UPDATE f          SET              f.eventos_gestion_dia = 0,              f.nuevos_clientes_gestionados_dia = 0,              f.nuevos_contactos_directos_dia = 0,              f.cantidad_promesas_dia = 0,              f.monto_promesas_dia = 0,              f.cantidad_pagadores_dia = 0,              f.monto_recuperado_dia = 0,              f.fecha_carga = SYSUTCDATETIME()            FROM analitica.hecho_cartera_diario AS f            INNER JOIN analitica.dim_fecha AS d              ON d.clave_fecha = f.clave_fecha            WHERE f.clave_cliente = @client_key            AND f.clave_campana = @campaign_key              AND d.fecha_calendario >= @campaign_start            AND d.fecha_calendario <= @as_of_date              AND EXISTS              (                  SELECT 1                  FROM analitica.dim_cartera AS bp                  WHERE bp.clave_cartera = f.clave_cartera                    AND bp.clave_cliente = @client_key                    AND bp.unidad_negocio_origen = @business_unit_name              );              UPDATE f          SET              f.eventos_gestion_dia =                  s.eventos_gestion_dia,                f.nuevos_clientes_gestionados_dia =                  s.nuevos_clientes_gestionados_dia,                f.nuevos_contactos_directos_dia =                  s.nuevos_contactos_directos_dia,                f.cantidad_promesas_dia =                  s.cantidad_promesas_dia,                f.monto_promesas_dia =                  s.monto_promesas_dia,                f.cantidad_pagadores_dia =                  s.cantidad_pagadores_dia,                f.monto_recuperado_dia =                  s.monto_recuperado_dia,                f.fecha_carga =                  SYSUTCDATETIME()            FROM analitica.hecho_cartera_diario AS f            INNER JOIN #DailyFlow AS s              ON s.clave_fecha = f.clave_fecha             AND s.clave_cartera = f.clave_cartera            WHERE f.clave_cliente = @client_key            AND f.clave_campana = @campaign_key;              /* --------------------------------------------------------             13.4 Promise/PDP UPSERT             -------------------------------------------------------- */            UPDATE p          SET              p.clave_cartera =                  s.clave_cartera,                p.id_deudor_origen =                  s.id_deudor_origen,                p.fecha_hora_gestion =                  s.fecha_hora_gestion,                p.fecha_vencimiento_promesa =                  s.fecha_vencimiento_promesa,                p.monto_promesa =                  s.monto_promesa,                p.monto_pagado =                  s.monto_pagado,                p.fecha_ultimo_pago =                  s.fecha_ultimo_pago,                p.estado_origen =                  s.estado_origen,                p.codigo_estado =                  s.codigo_estado,                p.es_promesa_valida =                  s.es_promesa_valida,                p.fecha_actualizacion_origen =                  s.fecha_actualizacion_origen,                p.fecha_carga =                  SYSUTCDATETIME()            FROM analitica.hecho_promesa AS p            INNER JOIN #PromiseStage AS s              ON s.id_operacion_origen =                 p.id_operacion_origen            WHERE p.clave_cliente = @client_key;              INSERT INTO analitica.hecho_promesa          (              clave_cliente,              clave_campana,              clave_cartera,              clave_asesor,                id_operacion_origen,              id_deudor_origen,                fecha_hora_gestion,              fecha_vencimiento_promesa,                monto_promesa,              monto_pagado,              fecha_ultimo_pago,                estado_origen,              codigo_estado,              es_promesa_valida,                fecha_actualizacion_origen          )            SELECT              @client_key,              @campaign_key,              s.clave_cartera,              NULL,                s.id_operacion_origen,              s.id_deudor_origen,                s.fecha_hora_gestion,              s.fecha_vencimiento_promesa,                s.monto_promesa,              s.monto_pagado,              s.fecha_ultimo_pago,                s.estado_origen,              s.codigo_estado,              s.es_promesa_valida,                s.fecha_actualizacion_origen            FROM #PromiseStage AS s            WHERE NOT EXISTS          (              SELECT 1                FROM analitica.hecho_promesa AS p                WHERE p.clave_cliente = @client_key                AND p.id_operacion_origen =                    s.id_operacion_origen          );              /* --------------------------------------------------------             13.5 Watermark             -------------------------------------------------------- */            UPDATE integracion_datos_analitica.control_carga          SET              fecha_ultimo_exito =                  SYSUTCDATETIME(),                fecha_hora_ultimo_origen =                  @source_as_of_at,                id_ultimo_origen =                  @last_source_id,                dias_solapamiento =                  0,                fecha_actualizacion =                  SYSUTCDATETIME()            WHERE codigo_origen = @watermark_source_code;              IF @@ROWCOUNT = 0          BEGIN                INSERT INTO integracion_datos_analitica.control_carga              (                  codigo_origen,                  fecha_ultimo_exito,                  fecha_hora_ultimo_origen,                  id_ultimo_origen,                  dias_solapamiento              )                VALUES              (                  @watermark_source_code,                  SYSUTCDATETIME(),                  @source_as_of_at,                  @last_source_id,                  0              );            END;              COMMIT TRANSACTION;              /* ========================================================             14. RESUMEN VERIFICABLE             ======================================================== */            ;WITH ContactPairs AS          (              SELECT DISTINCT                  f.clave_cartera,                  f.id_deudor_origen,                    MAX(                      CONVERT(                          INT,                          f.indicador_contacto_directo                      )                  )                  OVER                  (                      PARTITION BY                          f.clave_cartera,                          f.id_deudor_origen                  ) AS has_cd,                    MAX(                      CONVERT(                          INT,                          f.indicador_contacto_indirecto                      )                  )                  OVER                  (                      PARTITION BY                          f.clave_cartera,                          f.id_deudor_origen                  ) AS has_ci,                    MAX(                      CONVERT(                          INT,                          f.indicador_sin_contacto                      )                  )                  OVER                  (                      PARTITION BY                          f.clave_cartera,                          f.id_deudor_origen                  ) AS has_nc                FROM analitica.hecho_contacto_deudor_diario AS f                INNER JOIN analitica.dim_fecha AS d                  ON d.clave_fecha = f.clave_fecha                INNER JOIN analitica.dim_cartera AS bp                  ON bp.clave_cartera = f.clave_cartera                 AND bp.clave_cliente = @client_key                 AND bp.unidad_negocio_origen = @business_unit_name                WHERE f.clave_cliente = @client_key                AND f.clave_campana = @campaign_key                  AND d.fecha_calendario >= @campaign_start                AND d.fecha_calendario <= @as_of_date          ),            ContactSummary AS          (              SELECT                  SUM(                      CASE                          WHEN has_cd = 1                            OR has_ci = 1                            OR has_nc = 1                              THEN 1                          ELSE 0                      END                  ) AS classifiable_pairs,                    SUM(                      CASE                          WHEN has_cd = 1                              THEN 1                          ELSE 0                      END                  ) AS direct_contact_pairs                FROM ContactPairs          ),            FlowSummary AS          (              SELECT                  SUM(                      f.eventos_gestion_dia                  ) AS eventos_gestion,                    SUM(                      f.cantidad_promesas_dia                  ) AS cantidad_promesas,                    SUM(                      f.monto_promesas_dia                  ) AS monto_promesas,                    SUM(                      f.cantidad_pagadores_dia                  ) AS payer_pairs_day_sum,                    SUM(                      f.monto_recuperado_dia                  ) AS monto_recuperado                FROM analitica.hecho_cartera_diario AS f                INNER JOIN analitica.dim_fecha AS d                  ON d.clave_fecha = f.clave_fecha                INNER JOIN analitica.dim_cartera AS bp                  ON bp.clave_cartera = f.clave_cartera                 AND bp.clave_cliente = @client_key                 AND bp.unidad_negocio_origen = @business_unit_name                WHERE f.clave_cliente = @client_key                AND f.clave_campana = @campaign_key                  AND d.fecha_calendario >= @campaign_start                AND d.fecha_calendario <= @as_of_date          ),            PromiseSummary AS          (              SELECT                  SUM(                      CASE                          WHEN p.es_promesa_valida = 1                           AND p.codigo_estado = 'DUE_TODAY'                              THEN 1                          ELSE 0                      END                  ) AS due_today_count,                    SUM(                      CASE                          WHEN p.es_promesa_valida = 1                           AND p.codigo_estado = 'DUE_TODAY'                              THEN p.monto_promesa                          ELSE 0                      END                  ) AS due_today_amount,                    SUM(                      CASE                          WHEN p.es_promesa_valida = 1                           AND p.codigo_estado = 'BROKEN'                              THEN 1                          ELSE 0                      END                  ) AS broken_count,                    SUM(                      CASE                          WHEN p.es_promesa_valida = 1                           AND p.codigo_estado = 'BROKEN'                              THEN p.monto_promesa                          ELSE 0                      END                  ) AS broken_amount                FROM analitica.hecho_promesa AS p                INNER JOIN analitica.dim_cartera AS bp                  ON bp.clave_cartera = p.clave_cartera                 AND bp.clave_cliente = @client_key                 AND bp.unidad_negocio_origen = @business_unit_name                WHERE p.clave_cliente = @client_key                AND p.clave_campana = @campaign_key          )            SELECT              @codigo_campana                  AS codigo_campana,                @codigo_unidad_negocio                  AS codigo_unidad_negocio,                @business_unit_name                  AS unidad_negocio_origen,                @fecha_corte                  AS requested_as_of_at,                @source_as_of_at                  AS fecha_corte_origen,                /*                 Ahora source_rows es SIEMPRE el conteo físico RAW                 de GESTION-COB2, antes del mapping de cartera.              */              @source_rows                  AS source_rows,                @ignored_non_promise_without_id                  AS ignored_non_promise_without_operation_id,                fs.eventos_gestion,                cs.classifiable_pairs,              cs.direct_contact_pairs,                CAST(                  1.0 * cs.direct_contact_pairs                  / NULLIF(                      cs.classifiable_pairs,                      0                  )                  AS DECIMAL(18,6)              ) AS tasa_rpc,                fs.cantidad_promesas,              fs.monto_promesas,              fs.monto_recuperado,                ps.due_today_count,              ps.due_today_amount,                ps.broken_count,              ps.broken_amount            FROM FlowSummary AS fs            CROSS JOIN ContactSummary AS cs            CROSS JOIN PromiseSummary AS ps;          END TRY        BEGIN CATCH            IF @@TRANCOUNT > 0              ROLLBACK TRANSACTION;            THROW;        END CATCH;    END;
GO
