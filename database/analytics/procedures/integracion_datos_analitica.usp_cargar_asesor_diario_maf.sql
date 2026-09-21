/*
===============================================================================
MAF ADVISOR DAILY - DEFINICIÓN CANÓNICA VERSIONADA
Procedimiento: integracion_datos_analitica.usp_cargar_asesor_diario_maf

Regla de atribución de pagos MAF:
- la fuente no informa nId_UsuarioCob/nId_Usuario para los pagos observados;
- se atribuye únicamente cuando existe una gestión humana del mismo cliente,
  cartera, documento y deudor, anterior o simultánea al pago;
- gana la gestión humana más reciente;
- pagos sin evidencia suficiente permanecen no atribuibles;
- el grano diario conserva días con pago atribuible aunque no exista gestión
  del asesor exactamente ese mismo día.

Validado para septiembre 2026 con reproceso idempotente de MAF.
===============================================================================
*/

/* ============================================================================    MAF - REANUDAR BACKFILL ADVISOR DESDE 2026-08     Corrige el conflicto:      UX_puente_supervisor_asesor_clave_asesor_vigente_desde      duplicate key (110, 2026-09-01)     Causa:      durante el backfill histórico se intentaba crear una relación CURRENT      desde el día siguiente a cada cierre mensual. Agosto intentaba crear      2026-09-01, fecha que ya existe como observación histórica de septiembre.     Solución:      - histórico: @CrearJerarquiaActual = 0      - último corte operativo: @CrearJerarquiaActual = 1     NO reprocesa enero 2024 - julio 2026.    ============================================================================ */  CREATE OR ALTER PROCEDURE     integracion_datos_analitica.usp_cargar_asesor_diario_maf (     @FechaCorte DATE,     @Aplicar BIT = 1,     @MostrarResultado BIT = 0,     @CrearJerarquiaActual BIT = 1 ) AS BEGIN     SET NOCOUNT ON;     SET XACT_ABORT ON;      DECLARE         @IdCliente INT = 59,         @IdContrato INT = 246,         @IdGrupo INT = 194,         @ClaveCliente INT,         @ClaveCampana INT,         @FechaInicio DATE,         @FechaHasta DATETIME2(3),         @AhoraUtc DATETIME2(3) = SYSUTCDATETIME(),         @FechaHoraUltimoOrigen DATETIME2(3),         @IdUltimoOrigen BIGINT;      IF @FechaCorte IS NULL         THROW 52600, 'Debe indicar @FechaCorte.', 1;      IF @FechaCorte > CONVERT(DATE, GETDATE())         THROW 52601, 'La fecha de corte no puede estar en el futuro.', 1;      SET @FechaInicio = DATEFROMPARTS(YEAR(@FechaCorte), MONTH(@FechaCorte), 1);     SET @FechaHasta = DATEADD(DAY, 1, CONVERT(DATETIME2(3), @FechaCorte));      SELECT @ClaveCliente = clave_cliente     FROM analitica.dim_cliente     WHERE id_cliente_crm = @IdCliente       AND es_activo = 1;      IF @ClaveCliente IS NULL         THROW 52602, 'MAF no existe o no está activo en analitica.dim_cliente.', 1;      SELECT @ClaveCampana = clave_campana     FROM analitica.dim_campana     WHERE clave_cliente = @ClaveCliente       AND anio_campana = YEAR(@FechaCorte)       AND mes_campana = MONTH(@FechaCorte);      IF @ClaveCampana IS NULL         THROW 52603, 'No existe la campaña MAF del corte solicitado.', 1;      EXEC integracion_datos_analitica.usp_asegurar_rango_fechas         @FechaInicio,         @FechaCorte;       /* ------------------------------------------------------------------------        Carteras físicas productivas del mes.        ------------------------------------------------------------------------ */     IF OBJECT_ID('tempdb..#Carteras') IS NOT NULL DROP TABLE #Carteras;      SELECT         ca.nId_Cartera,         dc.clave_cartera     INTO #Carteras     FROM dbo.av_Cartera ca     INNER JOIN analitica.dim_cartera dc         ON dc.clave_cliente = @ClaveCliente        AND dc.id_cartera_origen = ca.nId_Cartera     WHERE ca.nId_Cliente = @IdCliente       AND ca.nId_Contrato = @IdContrato       AND ca.nId_Grupo = @IdGrupo       AND ca.nAnioCar = YEAR(@FechaCorte)       AND ca.nCampCar = MONTH(@FechaCorte)       AND ca.cCiclo IN ('CICLO 04','CICLO 11','CICLO 18','CICLO 25')       AND UPPER(ISNULL(ca.cCar_Nombre,'')) NOT LIKE '%BORRADOR%';      IF NOT EXISTS (SELECT 1 FROM #Carteras)         THROW 52604, 'No existen carteras MAF productivas para la campaña.', 1;       /* ------------------------------------------------------------------------        Deudores elegibles al corte. Mismo universo documental del ETL MAF.        ------------------------------------------------------------------------ */     IF OBJECT_ID('tempdb..#BaseDeudor') IS NOT NULL DROP TABLE #BaseDeudor;      SELECT DISTINCT         c.nId_Cartera,         c.clave_cartera,         dc.nId_PersDeudor     INTO #BaseDeudor     FROM #Carteras c     INNER JOIN dbo.av_DocxCobrar dc         ON dc.nId_Cliente = @IdCliente        AND dc.nId_Cartera = c.nId_Cartera     INNER JOIN dbo.av_DocxCobrarParam dp         ON dp.nId_Cliente = dc.nId_Cliente        AND dp.nId_Cartera = dc.nId_Cartera        AND dp.nId_DocxCobrar = dc.nId_DocxCobrar     WHERE ISNULL(dc.nId_DocxCobrarEst,1) <> 8       AND UPPER(LTRIM(RTRIM(ISNULL(dc.cCampo4,'')))) <> 'RETIRADO'       AND dc.dDoc_FecIngreso < @FechaHasta;       /* ------------------------------------------------------------------------        Fuente humana MTD.        ------------------------------------------------------------------------ */     IF OBJECT_ID('tempdb..#Source') IS NOT NULL DROP TABLE #Source;      SELECT         f.clave_fecha,         c.clave_cartera,         o.nId_Cartera,         CONVERT(INT, o.nId_UsuOpe) AS id_asesor_origen,         CONVERT(BIGINT, o.nId_DocxCobrarOpe) AS id_operacion_origen,         CONVERT(BIGINT, o.nId_PersDeudor) AS id_deudor_origen,         COALESCE(o.dDoc_FecIngresoGes, o.dDocCobOpe_FecIni)             AS fecha_hora_gestion,          NULLIF(LTRIM(RTRIM(u.cUsr_NroDoc)),'')             AS documento_asesor,          NULLIF         (             LTRIM(RTRIM(CONCAT(                 ISNULL(u.cUsr_Nombres,''),                 CASE WHEN NULLIF(LTRIM(RTRIM(u.cUsr_ApePat)),'') IS NOT NULL THEN ' ' ELSE '' END,                 ISNULL(u.cUsr_ApePat,''),                 CASE WHEN NULLIF(LTRIM(RTRIM(u.cUsr_ApeMat)),'') IS NOT NULL THEN ' ' ELSE '' END,                 ISNULL(u.cUsr_ApeMat,'')             ))),             ''         ) AS nombre_asesor,          CONVERT(BIT, ISNULL(u.bEstado,0)) AS asesor_es_activo,          COALESCE         (             NULLIF(o.nId_UsrLider,0),             NULLIF(u.nid_UsuSuper,0)         ) AS id_supervisor_origen,          UPPER         (             COALESCE             (                 NULLIF(LTRIM(RTRIM(cod.cParam04)),''),                 NULLIF(LTRIM(RTRIM(tc.indicador_equiv)),''),                 ''             )         ) AS codigo_contacto      INTO #Source      FROM dbo.av_DocxCobrarOpe o      INNER JOIN #BaseDeudor b         ON b.nId_Cartera = o.nId_Cartera        AND b.nId_PersDeudor = o.nId_PersDeudor      INNER JOIN #Carteras c         ON c.nId_Cartera = o.nId_Cartera      INNER JOIN dbo.av_Usuario u         ON u.nId_Usuario = o.nId_UsuOpe        AND u.nId_PerfilGest IS NULL      INNER JOIN dbo.av_OpeCodCliOut cod         ON cod.nId_Cliente = o.nId_Cliente        AND cod.nId_OpeCodCliOut = o.nId_OpeCodOut      LEFT JOIN dbo.av_TipoContacto tc         ON tc.nId_TipoContacto = cod.nId_TipoContacto      INNER JOIN analitica.dim_fecha f         ON f.fecha_calendario =             CONVERT(DATE, COALESCE(o.dDoc_FecIngresoGes, o.dDocCobOpe_FecIni))      WHERE o.nId_Cliente = @IdCliente       AND o.tip_gestion IN (1,2,4)       AND o.nId_UsuOpe IS NOT NULL       AND COALESCE(o.dDoc_FecIngresoGes, o.dDocCobOpe_FecIni) >= @FechaInicio       AND COALESCE(o.dDoc_FecIngresoGes, o.dDocCobOpe_FecIni) < @FechaHasta;      IF NOT EXISTS (SELECT 1 FROM #Source)         THROW 52605, 'No existen gestiones humanas MAF para el corte solicitado.', 1;      SELECT         @FechaHoraUltimoOrigen = MAX(fecha_hora_gestion),         @IdUltimoOrigen = MAX(id_operacion_origen)     FROM #Source;       /* ------------------------------------------------------------------------        Dimensión asesor.        ------------------------------------------------------------------------ */     IF OBJECT_ID('tempdb..#Asesores') IS NOT NULL DROP TABLE #Asesores;      ;WITH Ranked AS     (         SELECT             id_asesor_origen,             documento_asesor,             nombre_asesor,             asesor_es_activo,             fecha_hora_gestion,             id_operacion_origen,             ROW_NUMBER() OVER             (                 PARTITION BY id_asesor_origen                 ORDER BY fecha_hora_gestion DESC, id_operacion_origen DESC             ) AS rn         FROM #Source     )     SELECT         id_asesor_origen,         documento_asesor,         COALESCE(nombre_asesor, CONCAT('Usuario ', id_asesor_origen))             AS nombre_asesor,         asesor_es_activo     INTO #Asesores     FROM Ranked     WHERE rn = 1;      UPDATE a     SET         a.documento_asesor = s.documento_asesor,         a.nombre_asesor = s.nombre_asesor,         a.nombre_rol = 'GESTOR MAF',         a.es_activo = s.asesor_es_activo,         a.fecha_actualizacion = @AhoraUtc     FROM analitica.dim_asesor a     INNER JOIN #Asesores s         ON a.clave_cliente = @ClaveCliente        AND a.id_asesor_origen = CONVERT(VARCHAR(50), s.id_asesor_origen);      INSERT INTO analitica.dim_asesor     (         clave_cliente,         id_asesor_origen,         documento_asesor,         nombre_asesor,         nombre_rol,         es_activo     )     SELECT         @ClaveCliente,         CONVERT(VARCHAR(50), s.id_asesor_origen),         s.documento_asesor,         s.nombre_asesor,         'GESTOR MAF',         s.asesor_es_activo     FROM #Asesores s     WHERE NOT EXISTS     (         SELECT 1         FROM analitica.dim_asesor a         WHERE a.clave_cliente = @ClaveCliente           AND a.id_asesor_origen = CONVERT(VARCHAR(50), s.id_asesor_origen)     );      IF OBJECT_ID('tempdb..#AdvisorMap') IS NOT NULL DROP TABLE #AdvisorMap;      SELECT         TRY_CONVERT(INT, a.id_asesor_origen) AS id_asesor_origen,         a.clave_asesor     INTO #AdvisorMap     FROM analitica.dim_asesor a     WHERE a.clave_cliente = @ClaveCliente       AND TRY_CONVERT(INT, a.id_asesor_origen) IN           (SELECT id_asesor_origen FROM #Asesores);       /* ------------------------------------------------------------------------        Supervisores observados/current.        ------------------------------------------------------------------------ */     IF OBJECT_ID('tempdb..#SupervisorSource') IS NOT NULL DROP TABLE #SupervisorSource;      SELECT DISTINCT         s.id_supervisor_origen,         NULLIF(LTRIM(RTRIM(su.cUsr_NroDoc)),'') AS documento_supervisor,         NULLIF         (             LTRIM(RTRIM(CONCAT(                 ISNULL(su.cUsr_Nombres,''),                 CASE WHEN NULLIF(LTRIM(RTRIM(su.cUsr_ApePat)),'') IS NOT NULL THEN ' ' ELSE '' END,                 ISNULL(su.cUsr_ApePat,''),                 CASE WHEN NULLIF(LTRIM(RTRIM(su.cUsr_ApeMat)),'') IS NOT NULL THEN ' ' ELSE '' END,                 ISNULL(su.cUsr_ApeMat,'')             ))),             ''         ) AS nombre_supervisor,         CONVERT(BIT, ISNULL(su.bEstado,0)) AS es_activo     INTO #SupervisorSource     FROM #Source s     INNER JOIN dbo.av_Usuario su         ON su.nId_Usuario = s.id_supervisor_origen     WHERE s.id_supervisor_origen IS NOT NULL;      /* current que no haya aparecido aún como líder en una operación MTD */     INSERT INTO #SupervisorSource     (         id_supervisor_origen,         documento_supervisor,         nombre_supervisor,         es_activo     )     SELECT DISTINCT         su.nId_Usuario,         NULLIF(LTRIM(RTRIM(su.cUsr_NroDoc)),''),         NULLIF         (             LTRIM(RTRIM(CONCAT(                 ISNULL(su.cUsr_Nombres,''),                 CASE WHEN NULLIF(LTRIM(RTRIM(su.cUsr_ApePat)),'') IS NOT NULL THEN ' ' ELSE '' END,                 ISNULL(su.cUsr_ApePat,''),                 CASE WHEN NULLIF(LTRIM(RTRIM(su.cUsr_ApeMat)),'') IS NOT NULL THEN ' ' ELSE '' END,                 ISNULL(su.cUsr_ApeMat,'')             ))),             ''         ),         CONVERT(BIT, ISNULL(su.bEstado,0))     FROM #Asesores x     INNER JOIN dbo.av_Usuario u         ON u.nId_Usuario = x.id_asesor_origen     INNER JOIN dbo.av_Usuario su         ON su.nId_Usuario = u.nid_UsuSuper     WHERE u.nid_UsuSuper IS NOT NULL       AND NOT EXISTS       (           SELECT 1           FROM #SupervisorSource q           WHERE q.id_supervisor_origen = su.nId_Usuario       );      UPDATE d     SET         d.documento_supervisor = s.documento_supervisor,         d.nombre_supervisor = COALESCE(s.nombre_supervisor, d.nombre_supervisor),         d.es_activo = s.es_activo,         d.fecha_actualizacion = @AhoraUtc     FROM analitica.dim_supervisor d     INNER JOIN #SupervisorSource s         ON d.clave_cliente = @ClaveCliente        AND d.id_supervisor_origen =            CONVERT(VARCHAR(50), s.id_supervisor_origen);      INSERT INTO analitica.dim_supervisor     (         clave_cliente,         id_supervisor_origen,         documento_supervisor,         nombre_supervisor,         es_activo     )     SELECT         @ClaveCliente,         CONVERT(VARCHAR(50), s.id_supervisor_origen),         s.documento_supervisor,         COALESCE(s.nombre_supervisor, CONCAT('Supervisor ', s.id_supervisor_origen)),         s.es_activo     FROM #SupervisorSource s     WHERE NOT EXISTS     (         SELECT 1         FROM analitica.dim_supervisor d         WHERE d.clave_cliente = @ClaveCliente           AND d.id_supervisor_origen =               CONVERT(VARCHAR(50), s.id_supervisor_origen)     );       /* ------------------------------------------------------------------------        Contacto detallado asesor/deudor/día.        ------------------------------------------------------------------------ */     IF OBJECT_ID('tempdb..#ContactoDetalle') IS NOT NULL DROP TABLE #ContactoDetalle;      SELECT         s.clave_fecha,         s.clave_cartera,         a.clave_asesor,         s.id_deudor_origen,         CONVERT(BIT, MAX(CASE WHEN s.codigo_contacto = 'CD' THEN 1 ELSE 0 END))             AS indicador_contacto_directo,         CONVERT(BIT, MAX(CASE WHEN s.codigo_contacto = 'CI' THEN 1 ELSE 0 END))             AS indicador_contacto_indirecto,         CONVERT(BIT, MAX(CASE WHEN s.codigo_contacto = 'NC' THEN 1 ELSE 0 END))             AS indicador_sin_contacto     INTO #ContactoDetalle     FROM #Source s     INNER JOIN #AdvisorMap a         ON a.id_asesor_origen = s.id_asesor_origen     GROUP BY         s.clave_fecha,         s.clave_cartera,         a.clave_asesor,         s.id_deudor_origen;       /* ------------------------------------------------------------------------        Promesa diaria por asesor, usando el hecho_promesa ya validado.        ------------------------------------------------------------------------ */     IF OBJECT_ID('tempdb..#PromesaDiaria') IS NOT NULL DROP TABLE #PromesaDiaria;      SELECT         s.clave_fecha,         s.clave_cartera,         a.clave_asesor,         COUNT_BIG(*) AS cantidad_promesas,         CONVERT(DECIMAL(19,4), SUM(ISNULL(p.monto_promesa,0))) AS monto_promesas     INTO #PromesaDiaria     FROM analitica.hecho_promesa p     INNER JOIN #Source s         ON s.id_operacion_origen = p.id_operacion_origen     INNER JOIN #AdvisorMap a         ON a.id_asesor_origen = s.id_asesor_origen     WHERE p.clave_cliente = @ClaveCliente       AND p.clave_campana = @ClaveCampana       AND p.es_promesa_valida = 1     GROUP BY         s.clave_fecha,         s.clave_cartera,         a.clave_asesor;       /* ------------------------------------------------------------------------
       Pagos atribuibles a asesor para MAF.

       La fuente MAF no informa av_DocxPago.nId_UsuarioCob ni nId_Usuario,
       y tampoco existe cobertura útil mediante nId_DocxPagoComp.

       Regla de atribución conservadora:
       - mismo cliente;
       - misma cartera;
       - mismo documento por cobrar;
       - mismo deudor;
       - gestión humana tip_gestion IN (1,2,4);
       - nId_UsuOpe > 0 y usuario humano (nId_PerfilGest IS NULL);
       - gestión ocurrida antes o en la fecha/hora del pago;
       - gana la gestión humana más reciente.

       Si no existe una gestión humana que cumpla estas condiciones,
       el pago permanece NO ATRIBUIDO y no se inventa un asesor.
       ------------------------------------------------------------------------ */
    IF OBJECT_ID('tempdb..#PagoDetalle') IS NOT NULL DROP TABLE #PagoDetalle;

    SELECT
        f.clave_fecha,
        c.clave_cartera,
        a.clave_asesor,
        CONVERT(BIGINT, p.nId_PersDeudor) AS id_deudor_origen,
        CONVERT
        (
            DECIMAL(19,4),
            SUM
            (
                CASE
                    WHEN p.nId_MonPago = 1
                        THEN ISNULL(p.nDoc_ImpPago,0)
                    WHEN p.nId_MonPago = 2
                        THEN ISNULL(p.nDoc_ImpPago,0)
                             * COALESCE
                               (
                                   CASE
                                       WHEN rawtc.tc76 BETWEEN 2.5 AND 5.0
                                           THEN rawtc.tc76
                                   END,
                                   CASE
                                       WHEN fb.tipo_cambio BETWEEN 2.5 AND 5.0
                                           THEN fb.tipo_cambio
                                   END
                               )
                    ELSE 0
                END
            )
        ) AS monto_recuperado
    INTO #PagoDetalle
    FROM dbo.av_DocxPago p
    INNER JOIN #Carteras c
        ON c.nId_Cartera = p.nId_Cartera
    INNER JOIN dbo.av_DocxCobrar dc
        ON dc.nId_Cliente = p.nId_Cliente
       AND dc.nId_Cartera = p.nId_Cartera
       AND dc.nId_DocxCobrar = p.nId_DocxCobrar
    INNER JOIN dbo.av_DocxCobrarParam dp
        ON dp.nId_Cliente = dc.nId_Cliente
       AND dp.nId_Cartera = dc.nId_Cartera
       AND dp.nId_DocxCobrar = dc.nId_DocxCobrar
    OUTER APPLY
    (
        SELECT TOP (1)
            o.nId_UsuOpe
        FROM dbo.av_DocxCobrarOpe o
        INNER JOIN dbo.av_Usuario u
            ON u.nId_Usuario = o.nId_UsuOpe
           AND u.nId_PerfilGest IS NULL
        WHERE o.nId_Cliente = @IdCliente
          AND o.nId_Cartera = p.nId_Cartera
          AND o.nId_DocxCobrar = p.nId_DocxCobrar
          AND o.nId_PersDeudor = p.nId_PersDeudor
          AND o.tip_gestion IN (1,2,4)
          AND ISNULL(o.nId_UsuOpe,0) > 0
          AND COALESCE(o.dDoc_FecIngresoGes, o.dDocCobOpe_FecIni)
                <= p.dDoc_FecPago
        ORDER BY
            COALESCE(o.dDoc_FecIngresoGes, o.dDocCobOpe_FecIni) DESC,
            o.nId_DocxCobrarOpe DESC
    ) AS g
    INNER JOIN analitica.dim_asesor a
        ON a.clave_cliente = @ClaveCliente
       AND TRY_CONVERT(INT, a.id_asesor_origen) = g.nId_UsuOpe
    INNER JOIN analitica.dim_fecha f
        ON f.fecha_calendario = CONVERT(DATE, p.dDoc_FecPago)
    LEFT JOIN integracion_datos_analitica.tipo_cambio_fallback_maf fb
        ON fb.nId_Cartera = p.nId_Cartera
    OUTER APPLY
    (
        SELECT TRY_CONVERT
        (
            DECIMAL(19,6),
            NULLIF(REPLACE(LTRIM(RTRIM(dp.cDocParam76)), ',', '.'), '')
        ) AS tc76
    ) rawtc
    WHERE p.nId_Cliente = @IdCliente
      AND p.dDoc_FecPago >= @FechaInicio
      AND p.dDoc_FecPago < @FechaHasta
      AND ISNULL(dc.nId_DocxCobrarEst,1) <> 8
      AND UPPER(LTRIM(RTRIM(ISNULL(dc.cCampo4,'')))) <> 'RETIRADO'
      AND
      (
          p.nId_MonPago = 1
          OR
          (
              p.nId_MonPago = 2
              AND COALESCE
              (
                  CASE WHEN rawtc.tc76 BETWEEN 2.5 AND 5.0 THEN rawtc.tc76 END,
                  CASE WHEN fb.tipo_cambio BETWEEN 2.5 AND 5.0 THEN fb.tipo_cambio END
              ) IS NOT NULL
          )
      )
    GROUP BY
        f.clave_fecha,
        c.clave_cartera,
        a.clave_asesor,
        p.nId_PersDeudor;

    /* ------------------------------------------------------------------------        Producción diaria por asesor.        ------------------------------------------------------------------------ */     IF OBJECT_ID('tempdb..#AdvisorDaily') IS NOT NULL DROP TABLE #AdvisorDaily;      ;WITH Gestion AS
    (
        SELECT
            s.clave_fecha,
            s.clave_cartera,
            a.clave_asesor,
            COUNT_BIG(DISTINCT s.id_operacion_origen) AS eventos_gestion
        FROM #Source s
        INNER JOIN #AdvisorMap a
            ON a.id_asesor_origen = s.id_asesor_origen
        GROUP BY s.clave_fecha, s.clave_cartera, a.clave_asesor
    ),
    Contacto AS
    (
        SELECT
            clave_fecha,
            clave_cartera,
            clave_asesor,
            SUM(CASE WHEN indicador_contacto_directo = 1 THEN 1 ELSE 0 END)
                AS clientes_contacto_directo,
            SUM
            (
                CASE
                    WHEN indicador_contacto_directo = 0
                     AND indicador_contacto_indirecto = 1
                        THEN 1 ELSE 0
                END
            ) AS clientes_contacto_indirecto,
            SUM
            (
                CASE
                    WHEN indicador_contacto_directo = 0
                     AND indicador_contacto_indirecto = 0
                     AND indicador_sin_contacto = 1
                        THEN 1 ELSE 0
                END
            ) AS clientes_sin_contacto
        FROM #ContactoDetalle
        GROUP BY clave_fecha, clave_cartera, clave_asesor
    ),
    Pago AS
    (
        SELECT
            clave_fecha,
            clave_cartera,
            clave_asesor,
            COUNT_BIG(*) AS cantidad_pagadores,
            CONVERT(DECIMAL(19,4), SUM(monto_recuperado)) AS monto_recuperado
        FROM #PagoDetalle
        GROUP BY clave_fecha, clave_cartera, clave_asesor
    ),
    Grain AS
    (
        /*
          La fact diaria debe conservar también combinaciones
          fecha/cartera/asesor que tengan pago atribuible aunque ese asesor
          no haya registrado una gestión exactamente el día del pago.
        */
        SELECT clave_fecha, clave_cartera, clave_asesor
        FROM Gestion

        UNION

        SELECT clave_fecha, clave_cartera, clave_asesor
        FROM Pago
    )
    SELECT
        k.clave_fecha,
        k.clave_cartera,
        k.clave_asesor,
        ISNULL(g.eventos_gestion,0) AS eventos_gestion,
        ISNULL(c.clientes_contacto_directo,0) AS clientes_contacto_directo,
        ISNULL(c.clientes_contacto_indirecto,0) AS clientes_contacto_indirecto,
        ISNULL(c.clientes_sin_contacto,0) AS clientes_sin_contacto,
        ISNULL(pr.cantidad_promesas,0) AS cantidad_promesas,
        ISNULL(pr.monto_promesas,0) AS monto_promesas,
        ISNULL(pg.cantidad_pagadores,0) AS cantidad_pagadores,
        ISNULL(pg.monto_recuperado,0) AS monto_recuperado
    INTO #AdvisorDaily
    FROM Grain k
    LEFT JOIN Gestion g
        ON g.clave_fecha = k.clave_fecha
       AND g.clave_cartera = k.clave_cartera
       AND g.clave_asesor = k.clave_asesor
    LEFT JOIN Contacto c
        ON c.clave_fecha = k.clave_fecha
       AND c.clave_cartera = k.clave_cartera
       AND c.clave_asesor = k.clave_asesor
    LEFT JOIN #PromesaDiaria pr
        ON pr.clave_fecha = k.clave_fecha
       AND pr.clave_cartera = k.clave_cartera
       AND pr.clave_asesor = k.clave_asesor
    LEFT JOIN Pago pg
        ON pg.clave_fecha = k.clave_fecha
       AND pg.clave_cartera = k.clave_cartera
       AND pg.clave_asesor = k.clave_asesor;

    /* ------------------------------------------------------------------------        Jerarquía por día: última observación del día gana.        ------------------------------------------------------------------------ */     IF OBJECT_ID('tempdb..#JerarquiaDia') IS NOT NULL DROP TABLE #JerarquiaDia;      ;WITH Ranked AS     (         SELECT             s.clave_fecha,             CONVERT(DATE, s.fecha_hora_gestion) AS fecha_calendario,             a.clave_asesor,             s.id_supervisor_origen,             ROW_NUMBER() OVER             (                 PARTITION BY s.clave_fecha, a.clave_asesor                 ORDER BY s.fecha_hora_gestion DESC, s.id_operacion_origen DESC             ) AS rn         FROM #Source s         INNER JOIN #AdvisorMap a             ON a.id_asesor_origen = s.id_asesor_origen         WHERE s.id_supervisor_origen IS NOT NULL     )     SELECT         r.fecha_calendario,         r.clave_asesor,         d.clave_supervisor     INTO #JerarquiaDia     FROM Ranked r     INNER JOIN analitica.dim_supervisor d         ON d.clave_cliente = @ClaveCliente        AND d.id_supervisor_origen =            CONVERT(VARCHAR(50), r.id_supervisor_origen)     WHERE r.rn = 1;       IF @MostrarResultado = 1
    BEGIN
        SELECT
            COUNT(DISTINCT d.clave_asesor) AS asesores,
            COUNT_BIG(*) AS filas_asesor_dia_cartera,
            (
                SELECT COUNT(DISTINCT j.clave_supervisor)
                FROM #JerarquiaDia AS j
                WHERE j.clave_supervisor IS NOT NULL
            ) AS supervisores,
            SUM(d.eventos_gestion) AS gestiones,
            SUM(d.cantidad_promesas) AS promesas,
            SUM(d.cantidad_pagadores) AS pagadores_atribuidos,
            CONVERT(DECIMAL(19,4), SUM(d.monto_recuperado))
                AS recaudo_atribuible
        FROM #AdvisorDaily AS d;
    END;      IF @Aplicar = 0         RETURN;       /* ------------------------------------------------------------------------        Persistencia idempotente MTD.        ------------------------------------------------------------------------ */     BEGIN TRY         BEGIN TRANSACTION;          /* Completar asesor de promesas MAF */         UPDATE p         SET             p.clave_asesor = a.clave_asesor,             p.fecha_carga = @AhoraUtc         FROM analitica.hecho_promesa p         INNER JOIN #Source s             ON s.id_operacion_origen = p.id_operacion_origen         INNER JOIN #AdvisorMap a             ON a.id_asesor_origen = s.id_asesor_origen         WHERE p.clave_cliente = @ClaveCliente           AND p.clave_campana = @ClaveCampana;          DELETE f         FROM analitica.hecho_contacto_deudor_asesor_diario f         INNER JOIN analitica.dim_fecha d             ON d.clave_fecha = f.clave_fecha         WHERE f.clave_cliente = @ClaveCliente           AND f.clave_campana = @ClaveCampana           AND d.fecha_calendario >= @FechaInicio           AND d.fecha_calendario <= @FechaCorte;          INSERT INTO analitica.hecho_contacto_deudor_asesor_diario         (             clave_fecha,             clave_cliente,             clave_campana,             clave_cartera,             clave_asesor,             id_deudor_origen,             indicador_contacto_directo,             indicador_contacto_indirecto,             indicador_sin_contacto,             fecha_corte_origen         )         SELECT             clave_fecha,             @ClaveCliente,             @ClaveCampana,             clave_cartera,             clave_asesor,             id_deudor_origen,             indicador_contacto_directo,             indicador_contacto_indirecto,             indicador_sin_contacto,             @FechaHoraUltimoOrigen         FROM #ContactoDetalle;          DELETE f         FROM analitica.hecho_pago_deudor_asesor_diario f         INNER JOIN analitica.dim_fecha d             ON d.clave_fecha = f.clave_fecha         WHERE f.clave_cliente = @ClaveCliente           AND f.clave_campana = @ClaveCampana           AND d.fecha_calendario >= @FechaInicio           AND d.fecha_calendario <= @FechaCorte;          INSERT INTO analitica.hecho_pago_deudor_asesor_diario         (             clave_fecha,             clave_cliente,             clave_campana,             clave_cartera,             clave_asesor,             id_deudor_origen,             fecha_corte_origen         )         SELECT             clave_fecha,             @ClaveCliente,             @ClaveCampana,             clave_cartera,             clave_asesor,             id_deudor_origen,             @FechaHoraUltimoOrigen         FROM #PagoDetalle;          DELETE f         FROM analitica.hecho_asesor_diario f         INNER JOIN analitica.dim_fecha d             ON d.clave_fecha = f.clave_fecha         WHERE f.clave_cliente = @ClaveCliente           AND f.clave_campana = @ClaveCampana           AND d.fecha_calendario >= @FechaInicio           AND d.fecha_calendario <= @FechaCorte;          INSERT INTO analitica.hecho_asesor_diario         (             clave_fecha,             clave_cliente,             clave_campana,             clave_cartera,             clave_asesor,             eventos_gestion,             clientes_contacto_directo,             clientes_contacto_indirecto,             clientes_sin_contacto,             cantidad_promesas,             monto_promesas,             cantidad_pagadores,             monto_recuperado,             fecha_corte_origen         )         SELECT             clave_fecha,             @ClaveCliente,             @ClaveCampana,             clave_cartera,             clave_asesor,             eventos_gestion,             clientes_contacto_directo,             clientes_contacto_indirecto,             clientes_sin_contacto,             cantidad_promesas,             monto_promesas,             cantidad_pagadores,             monto_recuperado,             @FechaHoraUltimoOrigen         FROM #AdvisorDaily;          /* Rehacer jerarquía MTD para asesores MAF de la campaña */         DELETE b         FROM analitica.puente_supervisor_asesor b         INNER JOIN analitica.dim_asesor a             ON a.clave_asesor = b.clave_asesor            AND a.clave_cliente = @ClaveCliente         WHERE             b.es_actual = 1             OR             (                 b.vigente_desde >= @FechaInicio                 AND b.vigente_desde <= @FechaCorte             );          INSERT INTO analitica.puente_supervisor_asesor         (             clave_supervisor,             clave_asesor,             vigente_desde,             vigente_hasta,             es_actual         )         SELECT             clave_supervisor,             clave_asesor,             fecha_calendario,             fecha_calendario,             0         FROM #JerarquiaDia;          /*            Current: solo en carga operativa/final.             Durante un backfill histórico NO debe insertarse una relación            "actual" desde el día siguiente al corte histórico, porque puede            colisionar con una observación histórica ya materializada del mes            siguiente (UX_puente_supervisor_asesor_clave_asesor_vigente_desde).             El wrapper diario conserva el comportamiento anterior porque el            parámetro por defecto es @CrearJerarquiaActual = 1.         */         IF @CrearJerarquiaActual = 1         BEGIN             INSERT INTO analitica.puente_supervisor_asesor             (                 clave_supervisor,                 clave_asesor,                 vigente_desde,                 vigente_hasta,                 es_actual             )             SELECT                 ds.clave_supervisor,                 da.clave_asesor,                 DATEADD(DAY,1,@FechaCorte),                 NULL,                 1             FROM analitica.dim_asesor da             INNER JOIN dbo.av_Usuario u                 ON u.nId_Usuario = TRY_CONVERT(INT, da.id_asesor_origen)             INNER JOIN analitica.dim_supervisor ds                 ON ds.clave_cliente = @ClaveCliente                AND ds.id_supervisor_origen =                    CONVERT(VARCHAR(50), u.nid_UsuSuper)             WHERE da.clave_cliente = @ClaveCliente               AND da.clave_asesor IN (SELECT clave_asesor FROM #AdvisorMap)               AND u.nid_UsuSuper IS NOT NULL;         END;          UPDATE integracion_datos_analitica.control_carga         SET             fecha_ultimo_exito = @AhoraUtc,             fecha_hora_ultimo_origen = @FechaHoraUltimoOrigen,             id_ultimo_origen = @IdUltimoOrigen,             dias_solapamiento = 1,             fecha_actualizacion = @AhoraUtc         WHERE codigo_origen = 'MAF_ADVISOR_DAILY';          IF @@ROWCOUNT = 0         BEGIN             INSERT INTO integracion_datos_analitica.control_carga             (                 codigo_origen,                 fecha_ultimo_exito,                 fecha_hora_ultimo_origen,                 id_ultimo_origen,                 dias_solapamiento             )             VALUES             (                 'MAF_ADVISOR_DAILY',                 @AhoraUtc,                 @FechaHoraUltimoOrigen,                 @IdUltimoOrigen,                 1             );         END;          COMMIT TRANSACTION;     END TRY     BEGIN CATCH         IF @@TRANCOUNT > 0             ROLLBACK TRANSACTION;         THROW;     END CATCH; END;
GO
