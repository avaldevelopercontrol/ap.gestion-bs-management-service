/*
===============================================================================
CLARO PROD V2 - SHADOW / VALIDACION OLD VS PRODUCCION DIRECTA
===============================================================================
Objetivo
-------
Reconstruir, SIN ESCRIBIR EN analitica.*, la semantica operacional que hoy
llega a Analytics desde:

    aval_reporteria.dbo.rpt_gestiones_pagos_final
      -> dbo.vw_bi_gerencia_gestiones_pagos

pero leyendo directamente de aval_cob:

    dbo.av_Cartera
    dbo.av_DocxCobrar
    dbo.av_DocxCobrarParam
    dbo.av_DocxCobrarOpe
    dbo.av_DocxPago
    dbo.av_Usuario
    dbo.av_Perfil
    dbo.av_OpeCodCliOut
    dbo.av_TipoContacto

Reglas CLARO 95 confirmadas el 24/09/2026
-----------------------------------------
- nId_Cliente  = 95
- nId_Contrato = 182
- nId_Grupo    = 156
- DOCUMENTOS / IMPORTE_DESDE_PARAM = cDocParam33
- PAGOS / IMPORTE_DESDE_COLUMNA    = nDoc_ImpParam01
- TIPO_CAMBIO_MONTO_PROMESA        = 3.72
- FACTOR_TECHO_PROMESA             = 1.05
- Nombre PBI: cartera LIKE '%gob%' => CLARO GOBIERNO;
              resto                => CLARO CORPORATIVO
- Sin reglas pbi_carga_medio_pago para CLARO.

IMPORTANTE
----------
1) Este archivo es SHADOW: solo crea #temporales y SELECTs.
2) NO modifica facts, dimensiones, watermarks ni SP productivos.
3) La fuente OLD solo se usa al final para comparar. La construccion V2 no
   depende de aval_reporteria.
4) La deduplicacion de las filas sobrevivientes replica el hash OPERADOR del
   stage actual. Si CLARO tuviera un usuario humano configurado en un canal
   distinto de OPERADOR, la comparacion lo evidenciara antes de cualquier corte.
===============================================================================
*/

USE [aval_cob];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE
    @IdCliente       INT            = 95,
    @IdContrato      INT            = 182,
    @IdGrupo         INT            = 156,
    @Anio            SMALLINT       = 2026,
    @Mes             TINYINT        = 9,
    @FechaCorte      DATE           = CONVERT(DATE, GETDATE()),
    @TipoCambio      DECIMAL(18,6)  = 3.720000,
    @FactorTecho     DECIMAL(18,6)  = 1.050000;

IF @Mes NOT BETWEEN 1 AND 12
    THROW 53000, '@Mes debe estar entre 1 y 12.', 1;

DECLARE
    @InicioCampana DATE = DATEFROMPARTS(@Anio, @Mes, 1),
    @FinCampanaExclusivo DATE = DATEADD(MONTH, 1, DATEFROMPARTS(@Anio, @Mes, 1));

/* ==========================================================================
   1. ALCANCE FISICO CLARO
   ========================================================================== */

DROP TABLE IF EXISTS #Carteras;

SELECT
    ca.nId_Cartera,
    ca.nId_Cliente,
    ca.nId_Contrato,
    ca.nId_Grupo,
    ca.nAnioCar,
    ca.nCampCar,
    LTRIM(RTRIM(ca.cCar_Nombre)) AS cCar_Nombre,
    ca.dFecIniProceso,
    ca.dFecFinProceso,
    CASE
        WHEN LOWER(LTRIM(RTRIM(ca.cCar_Nombre))) LIKE '%gob%'
            THEN 'GOBIERNO'
        ELSE 'ADMINISTRATIVO'
    END AS codigo_unidad_negocio,
    CASE
        WHEN LOWER(LTRIM(RTRIM(ca.cCar_Nombre))) LIKE '%gob%'
            THEN 'CLARO GOBIERNO'
        ELSE 'CLARO CORPORATIVO'
    END AS cCli_Nombre_pbi
INTO #Carteras
FROM dbo.av_Cartera AS ca
WHERE ca.nId_Cliente = @IdCliente
  AND ca.nId_Contrato = @IdContrato
  AND ca.nId_Grupo = @IdGrupo
  AND ca.nAnioCar = @Anio
  AND ca.nCampCar = @Mes;

CREATE UNIQUE CLUSTERED INDEX CX_Carteras
    ON #Carteras(nId_Cartera);

IF NOT EXISTS (SELECT 1 FROM #Carteras)
    THROW 53001, 'No se encontraron carteras CLARO para el periodo indicado.', 1;

/* ==========================================================================
   2. DOCUMENTOS DIRECTOS

   Regla efectiva CLARO:
     IMPORTE_DESDE_PARAM = cDocParam33

   Misma cascada de reportería:
     cDocParam33 != 0
       -> nDoc_ImpTotal != 0
       -> nDoc_ImpSaldo != 0
       -> 0

   El stage actual exige que exista av_DocxCobrarParam (tiene_param = 1), por
   eso aqui se usa INNER JOIN al agregado de parametros.
   ========================================================================== */

DROP TABLE IF EXISTS #DocParam;

SELECT
    dp.nId_Cartera,
    dp.nId_Cliente,
    dp.nId_DocxCobrar,
    MAX(CONVERT(VARCHAR(500), dp.cDocParam33)) AS cDocParam33
INTO #DocParam
FROM dbo.av_DocxCobrarParam AS dp
INNER JOIN #Carteras AS c
    ON c.nId_Cartera = dp.nId_Cartera
WHERE dp.nId_Cliente = @IdCliente
GROUP BY
    dp.nId_Cartera,
    dp.nId_Cliente,
    dp.nId_DocxCobrar;

CREATE UNIQUE CLUSTERED INDEX CX_DocParam
    ON #DocParam(nId_Cartera, nId_DocxCobrar);

DROP TABLE IF EXISTS #Documentos;

SELECT
    dc.nId_Cartera,
    dc.nId_DocxCobrar,
    dc.nId_PersDeudor,
    dc.nId_Cliente,
    dc.nId_Moneda,
    CONVERT(DECIMAL(38,2), dc.nDoc_ImpTotal) AS nDoc_ImpTotal,
    CONVERT(DECIMAL(38,2), dc.nDoc_ImpSaldo) AS nDoc_ImpSaldo,
    CONVERT
    (
        DECIMAL(38,2),
        COALESCE
        (
            NULLIF
            (
                TRY_CONVERT
                (
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
INTO #Documentos
FROM dbo.av_DocxCobrar AS dc
INNER JOIN #Carteras AS c
    ON c.nId_Cartera = dc.nId_Cartera
INNER JOIN #DocParam AS dp
    ON dp.nId_Cartera = dc.nId_Cartera
   AND dp.nId_DocxCobrar = dc.nId_DocxCobrar
   AND dp.nId_Cliente = dc.nId_Cliente
WHERE dc.nId_Cliente = @IdCliente;

CREATE UNIQUE CLUSTERED INDEX CX_Documentos
    ON #Documentos(nId_Cartera, nId_DocxCobrar);
CREATE INDEX IX_Documentos_Deudor
    ON #Documentos(nId_Cartera, nId_PersDeudor)
    INCLUDE (importe_asignado);

DROP TABLE IF EXISTS #TechoDeudor;

SELECT
    d.nId_Cartera,
    d.nId_PersDeudor,
    CONVERT(DECIMAL(38,2), SUM(d.importe_asignado)) AS techo
INTO #TechoDeudor
FROM #Documentos AS d
WHERE d.nId_PersDeudor IS NOT NULL
GROUP BY
    d.nId_Cartera,
    d.nId_PersDeudor;

CREATE UNIQUE CLUSTERED INDEX CX_TechoDeudor
    ON #TechoDeudor(nId_Cartera, nId_PersDeudor);

/* ==========================================================================
   3. GESTIONES DIRECTAS ENRIQUECIDAS
   ========================================================================== */

DROP TABLE IF EXISTS #GestionesBase;

SELECT
    o.nId_Cartera,
    o.nId_DocxCobrarOpe,
    o.nId_DocxCobrar,
    o.nId_PersDeudor,
    o.nId_Cliente,

    COALESCE(u.nId_Usuario, o.nId_UsuOpe, 0) AS nId_Usuario,
    o.nId_UsuOpe,
    o.tip_gestion,

    CONVERT(DATETIME2(3), o.dDocCobOpe_FecIni) AS dDocCobOpe_FecIni,
    CONVERT(DATETIME2(3), o.dDocCobOpe_FecFin) AS dDocCobOpe_FecFin,
    CONVERT(DATETIME2(3), o.dDoc_FecIngresoGes) AS dDoc_FecIngresoGes,
    CONVERT(DATE, o.dFechCompromisoPago) AS dFechCompromisoPago,

    CONVERT
    (
        DECIMAL(38,4),
        ISNULL(o.monto_comp, 0)
        + (ISNULL(o.monto_compDolares, 0) * @TipoCambio)
    ) AS montoPromesa,

    CONVERT(VARCHAR(50), o.nTelef_Nro) AS nTelef_Nro,
    o.nId_GestionDisp,
    o.nId_OpeCodOut,
    o.nId_OpeCodOutNp2,
    CONVERT(VARCHAR(2000), o.cDocOpeCobOut_Descr) AS cDocOpeCobOut_Descr,

    cod.cNombre_OpeCodCliOut,
    cod.nId_TipoContacto,
    UPPER(LTRIM(RTRIM(ISNULL(tc.indicador_equiv, '')))) AS indicador_equiv,

    UPPER
    (
        LTRIM
        (
            RTRIM
            (
                CONCAT
                (
                    ISNULL(u.cUsr_ApePat, ''),
                    ' ',
                    ISNULL(u.cUsr_Nombres, '')
                )
            )
        )
    ) AS nombre_asesor,

    pf.per_Nombre AS cNombre_Cargo,
    u.nId_PerfilGest,

    c.cCar_Nombre,
    c.cCli_Nombre_pbi AS cCli_Nombre,
    c.nAnioCar AS anio,
    c.nCampCar,
    c.codigo_unidad_negocio,

    CONVERT
    (
        BIT,
        CASE WHEN ISNULL(cod.nId_TipoContacto, 1) = 2 THEN 1 ELSE 0 END
    ) AS marcaPromesa,

    CONVERT
    (
        BIT,
        CASE
            WHEN ISNULL(o.tip_gestion, 1) = 1
             AND u.nId_PerfilGest IS NULL
                THEN 1
            ELSE 0
        END
    ) AS marcaCall,

    CONVERT
    (
        BIT,
        CASE
            WHEN ISNULL(cod.nId_TipoContacto, 1) <= 3
             AND u.nId_PerfilGest IS NULL
                THEN 1
            ELSE 0
        END
    ) AS marcaCD

INTO #GestionesBase
FROM dbo.av_DocxCobrarOpe AS o
INNER JOIN #Carteras AS c
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
WHERE o.nId_Cliente = @IdCliente;

CREATE UNIQUE CLUSTERED INDEX CX_GestionesBase
    ON #GestionesBase(nId_Cartera, nId_DocxCobrarOpe);

/* ==========================================================================
   4. FILTROS FINALES + DEDUP OPERADOR

   Usuarios excluidos por el constructor de reportería:
     0      SIN ASIGNAR
     10214  COURIER
     10646  IVR
     10700  SISTEMAS
     12454  CORREO
     12455  SMS
     14550  WHATSAPP
     14771  BOT / ROBOT

   Para CLARO, después de retirar esos usuarios, se replica el hash OPERADOR
   observado en pbi_carga_lote_gestiones.
   ========================================================================== */

DROP TABLE IF EXISTS #GestionesElegibles;

SELECT
    g.*,
    HASHBYTES
    (
        'SHA2_256',
        CONCAT
        (
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
            CONVERT(VARCHAR(5), g.marcaCD), '|',
            'OPERADOR'
        )
    ) AS dup_hash
INTO #GestionesElegibles
FROM #GestionesBase AS g
WHERE g.anio = @Anio
  AND g.nId_PersDeudor IS NOT NULL
  AND g.dDocCobOpe_FecIni IS NOT NULL
  AND ISNULL(g.nId_GestionDisp, -1) <> 4
  AND g.nId_Usuario NOT IN
      (0, 10214, 10646, 10700, 12454, 12455, 14550, 14771)
  AND ISNULL(g.nId_UsuOpe, 0) NOT IN
      (0, 10214, 10646, 10700, 12454, 12455, 14550, 14771);

DROP TABLE IF EXISTS #GestionesDedup;

;WITH Rankeadas AS
(
    SELECT
        g.*,
        ROW_NUMBER() OVER
        (
            PARTITION BY
                g.nId_Cartera,
                g.cCar_Nombre,
                g.nId_PersDeudor,
                g.dup_hash
            ORDER BY
                g.dDoc_FecIngresoGes DESC,
                g.dDocCobOpe_FecFin DESC,
                g.nId_DocxCobrarOpe DESC
        ) AS rn
    FROM #GestionesElegibles AS g
)
SELECT *
INTO #GestionesDedup
FROM Rankeadas
WHERE rn = 1;

CREATE UNIQUE CLUSTERED INDEX CX_GestionesDedup
    ON #GestionesDedup(nId_Cartera, nId_DocxCobrarOpe);

/* ==========================================================================
   5. PROMESA: DATOS COMPLETOS VS PROMESA VALIDA
   ========================================================================== */

DROP TABLE IF EXISTS #Gestiones;

SELECT
    g.*,
    CONVERT
    (
        BIT,
        CASE
            WHEN g.indicador_equiv = 'CD'
             AND g.montoPromesa > 0
             AND g.dFechCompromisoPago IS NOT NULL
                THEN 1
            ELSE 0
        END
    ) AS es_promesa_datos,
    CONVERT
    (
        BIT,
        CASE
            WHEN g.indicador_equiv = 'CD'
             AND g.montoPromesa > 0
             AND g.dFechCompromisoPago IS NOT NULL
             AND CONVERT(DECIMAL(38,2), g.montoPromesa)
                 <= t.techo * @FactorTecho
                THEN 1
            ELSE 0
        END
    ) AS es_promesa
INTO #Gestiones
FROM #GestionesDedup AS g
LEFT JOIN #TechoDeudor AS t
    ON t.nId_Cartera = g.nId_Cartera
   AND t.nId_PersDeudor = g.nId_PersDeudor;

CREATE UNIQUE CLUSTERED INDEX CX_Gestiones
    ON #Gestiones(nId_Cartera, nId_DocxCobrarOpe);
CREATE INDEX IX_Gestiones_Deudor
    ON #Gestiones(nId_Cartera, nId_PersDeudor, dDocCobOpe_FecIni, dFechCompromisoPago)
    INCLUDE (es_promesa, nId_Usuario, montoPromesa);

/* ==========================================================================
   6. PAGOS DIRECTOS

   Regla CLARO 95:
     IMPORTE_DESDE_COLUMNA = nDoc_ImpParam01

   Igual que el cargador actual:
     - la fila origen entra solo si nDoc_ImpPago no es NULL;
     - el importe publicado se toma de nDoc_ImpParam01.
   ========================================================================== */

DROP TABLE IF EXISTS #Pagos;

SELECT
    p.nId_DocxPago,
    p.nId_Cartera,
    p.nId_DocxCobrar,
    p.nId_PersDeudor,
    p.nId_Cliente,
    CONVERT(DATETIME2(3), p.dDoc_FecPago) AS dDoc_FecPago,
    CONVERT(DECIMAL(38,4), p.nDoc_ImpParam01) AS nDoc_ImpPago,
    c.cCar_Nombre,
    c.cCli_Nombre_pbi AS cCli_Nombre,
    c.nAnioCar AS anio,
    c.nCampCar
INTO #Pagos
FROM dbo.av_DocxPago AS p
INNER JOIN #Carteras AS c
    ON c.nId_Cartera = p.nId_Cartera
WHERE p.nId_Cliente = @IdCliente
  AND p.nDoc_ImpPago IS NOT NULL;

CREATE UNIQUE CLUSTERED INDEX CX_Pagos
    ON #Pagos(nId_Cartera, nId_DocxPago);
CREATE INDEX IX_Pagos_DeudorFecha
    ON #Pagos(nId_Cartera, nId_PersDeudor, dDoc_FecPago)
    INCLUDE (nDoc_ImpPago);

DROP TABLE IF EXISTS #PagoBase;

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
INTO #PagoBase
FROM #Pagos AS p
WHERE p.nId_PersDeudor IS NOT NULL
  AND p.dDoc_FecPago IS NOT NULL
GROUP BY
    p.nId_PersDeudor,
    p.nId_Cliente,
    p.nId_Cartera,
    p.dDoc_FecPago,
    p.cCar_Nombre;

CREATE UNIQUE CLUSTERED INDEX CX_PagoBase
    ON #PagoBase(id_unico);
CREATE INDEX IX_PagoBase_Deudor
    ON #PagoBase(nId_Cartera, nId_PersDeudor, dDoc_FecPago);

/* ==========================================================================
   7. ADJUDICACION DE PAGOS A PROMESAS
   ========================================================================== */

DROP TABLE IF EXISTS #PagosCandidatos;

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
    g.nombre_asesor,
    g.dDocCobOpe_FecIni AS fecha_gestion,
    g.dDocCobOpe_FecFin AS fecha_fin_gestion,
    g.dDoc_FecIngresoGes AS fecha_ingreso_gestion,
    g.dFechCompromisoPago AS fecha_promesa,
    g.montoPromesa AS monto_promesa,

    CASE
        WHEN CONVERT(DATE, p.dDoc_FecPago) <= g.dFechCompromisoPago
            THEN 'cumplido'
        ELSE 'fuera_rango'
    END AS estado_candidato_pago,

    CASE
        WHEN CONVERT(DATE, p.dDoc_FecPago) <= g.dFechCompromisoPago
            THEN 1
        ELSE 2
    END AS estado_rank

INTO #PagosCandidatos
FROM #PagoBase AS p
INNER JOIN #Gestiones AS g
    ON g.nId_Cartera = p.nId_Cartera
   AND g.nId_PersDeudor = p.nId_PersDeudor
   AND g.es_promesa = 1
   AND g.dFechCompromisoPago >= CONVERT(DATE, g.dDocCobOpe_FecIni)
   AND CONVERT(DATE, p.dDoc_FecPago) >= CONVERT(DATE, g.dDocCobOpe_FecIni);

CREATE INDEX IX_PagosCandidatos_Pago
    ON #PagosCandidatos(id_unico, estado_rank, nId_Usuario);
CREATE INDEX IX_PagosCandidatos_Promesa
    ON #PagosCandidatos(nId_DocxCobrarOpe);

DROP TABLE IF EXISTS #PagosAsignados;

;WITH Asesores AS
(
    SELECT
        id_unico,
        nId_PersDeudor,
        nId_Cartera,
        dDoc_FecPago,
        estado_candidato_pago,
        COUNT(DISTINCT nId_Usuario) AS cantidad_asesores
    FROM #PagosCandidatos
    GROUP BY
        id_unico,
        nId_PersDeudor,
        nId_Cartera,
        dDoc_FecPago,
        estado_candidato_pago
),
Rankeados AS
(
    SELECT
        p.*,
        a.cantidad_asesores,
        ROW_NUMBER() OVER
        (
            PARTITION BY
                p.id_unico,
                p.nId_PersDeudor,
                p.nId_Cartera,
                p.dDoc_FecPago
            ORDER BY
                p.estado_rank,
                CASE WHEN a.cantidad_asesores = 1 THEN p.fecha_promesa END DESC,
                CASE WHEN a.cantidad_asesores = 1 THEN p.fecha_gestion END DESC,
                CASE WHEN a.cantidad_asesores = 1 THEN p.nId_DocxCobrarOpe END DESC,
                CASE WHEN a.cantidad_asesores > 1 THEN p.fecha_promesa END ASC,
                CASE WHEN a.cantidad_asesores > 1 THEN p.fecha_gestion END ASC,
                CASE WHEN a.cantidad_asesores > 1 THEN p.nId_DocxCobrarOpe END ASC
        ) AS rn
    FROM #PagosCandidatos AS p
    INNER JOIN Asesores AS a
        ON a.id_unico = p.id_unico
       AND a.nId_PersDeudor = p.nId_PersDeudor
       AND a.nId_Cartera = p.nId_Cartera
       AND a.dDoc_FecPago = p.dDoc_FecPago
       AND a.estado_candidato_pago = p.estado_candidato_pago
)
SELECT *
INTO #PagosAsignados
FROM Rankeados
WHERE rn = 1;

CREATE INDEX IX_PagosAsignados_Pago
    ON #PagosAsignados(id_unico);
CREATE INDEX IX_PagosAsignados_Promesa
    ON #PagosAsignados(nId_DocxCobrarOpe);

DROP TABLE IF EXISTS #PromesasNoAdjudicadas;

SELECT
    p.nId_DocxCobrarOpe,
    COUNT(*) AS cantidad_competencias
INTO #PromesasNoAdjudicadas
FROM #PagosCandidatos AS p
LEFT JOIN #PagosAsignados AS a
    ON a.id_unico = p.id_unico
   AND a.nId_DocxCobrarOpe = p.nId_DocxCobrarOpe
WHERE a.id_unico IS NULL
GROUP BY p.nId_DocxCobrarOpe;

CREATE UNIQUE CLUSTERED INDEX CX_PromesasNoAdjudicadas
    ON #PromesasNoAdjudicadas(nId_DocxCobrarOpe);

DROP TABLE IF EXISTS #PagosPorPromesa;

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
INTO #PagosPorPromesa
FROM #PagosAsignados
GROUP BY nId_DocxCobrarOpe;

CREATE UNIQUE CLUSTERED INDEX CX_PagosPorPromesa
    ON #PagosPorPromesa(nId_DocxCobrarOpe);

/* ==========================================================================
   8. FINAL V2 DIRECTO - MISMO CONTRATO CONCEPTUAL DE LA VISTA OLD
   ========================================================================== */

DROP TABLE IF EXISTS #V2;

CREATE TABLE #V2
(
    ultima_fecha_pago       DATETIME2(3) NULL,
    ultima_fecha_registro   DATETIME2(3) NULL,
    cCli_Nombre             VARCHAR(150) NULL,
    cCar_Nombre             VARCHAR(150) NOT NULL,
    nCampCar                INT NULL,
    nombre_asesor           VARCHAR(150) NULL,
    anio                    INT NULL,
    estado_pdp              VARCHAR(100) NULL,
    nId_PersDeudor          INT NULL,
    indicador_equiv         VARCHAR(100) NULL,
    montoPromesa            DECIMAL(38,4) NULL,
    total_pagado            DECIMAL(38,4) NULL,
    marca_promesa_valida    BIT NOT NULL,
    cNombre_Cargo           VARCHAR(150) NULL,
    dFechCompromisoPago     DATE NULL,
    dDocCobOpe_FecIni       DATETIME2(3) NULL,
    nId_DocxCobrarOpe       BIGINT NULL,
    detalle_gestion         VARCHAR(500) NULL,
    nTelef_Nro              VARCHAR(50) NULL,
    codigo_unidad_negocio   VARCHAR(20) NOT NULL,
    tipo_fila               VARCHAR(20) NOT NULL
);

DECLARE @CarteraId INT,
        @CarteraNombre VARCHAR(150),
        @Unidad VARCHAR(20),
        @FechaMaxPago DATE,
        @FechaFinProceso DATE;

DECLARE cur_cartera CURSOR LOCAL FAST_FORWARD FOR
SELECT
    nId_Cartera,
    cCar_Nombre,
    codigo_unidad_negocio
FROM #Carteras
ORDER BY nId_Cartera;

OPEN cur_cartera;
FETCH NEXT FROM cur_cartera
INTO @CarteraId, @CarteraNombre, @Unidad;

WHILE @@FETCH_STATUS = 0
BEGIN
    SELECT
        @FechaMaxPago = MAX(CONVERT(DATE, p.dDoc_FecPago))
    FROM #Pagos AS p
    WHERE p.nId_Cartera = @CarteraId;

    SELECT
        @FechaFinProceso = MAX(CONVERT(DATE, c.dFecFinProceso))
    FROM #Carteras AS c
    WHERE c.nId_Cartera = @CarteraId;

    /* Gestiones reales. */
    INSERT INTO #V2
    (
        ultima_fecha_pago,
        ultima_fecha_registro,
        cCli_Nombre,
        cCar_Nombre,
        nCampCar,
        nombre_asesor,
        anio,
        estado_pdp,
        nId_PersDeudor,
        indicador_equiv,
        montoPromesa,
        total_pagado,
        marca_promesa_valida,
        cNombre_Cargo,
        dFechCompromisoPago,
        dDocCobOpe_FecIni,
        nId_DocxCobrarOpe,
        detalle_gestion,
        nTelef_Nro,
        codigo_unidad_negocio,
        tipo_fila
    )
    SELECT
        pp.ultima_fecha_pago,
        g.dDoc_FecIngresoGes,
        g.cCli_Nombre,
        g.cCar_Nombre,
        g.nCampCar,
        g.nombre_asesor,
        g.anio,
        CASE
            WHEN pp.nId_DocxCobrarOpe IS NOT NULL
                THEN pp.estado_pdp
            WHEN pna.nId_DocxCobrarOpe IS NOT NULL
                THEN '8. Caido'
            WHEN g.es_promesa = 1
             AND g.dFechCompromisoPago > @FechaCorte
                THEN '1. Vigente'
            WHEN g.es_promesa = 1
             AND g.dFechCompromisoPago = @FechaCorte
                THEN '2. Vence Hoy'
            WHEN g.es_promesa = 1
             AND g.dFechCompromisoPago < @FechaCorte
             AND
             (
                 (@FechaFinProceso IS NOT NULL AND @FechaCorte > @FechaFinProceso)
                 OR
                 (@FechaMaxPago IS NOT NULL AND @FechaMaxPago > g.dFechCompromisoPago)
             )
                THEN '8. Caido'
            WHEN g.es_promesa = 1
             AND g.dFechCompromisoPago < @FechaCorte
                THEN '7. Por Confirmar'
            ELSE '9. No PdP y No Pagos'
        END AS estado_pdp,
        g.nId_PersDeudor,
        g.indicador_equiv,
        g.montoPromesa,
        pp.total_pagado,
        g.es_promesa,
        g.cNombre_Cargo,
        g.dFechCompromisoPago,
        g.dDocCobOpe_FecIni,
        g.nId_DocxCobrarOpe,
        g.cNombre_OpeCodCliOut,
        g.nTelef_Nro,
        g.codigo_unidad_negocio,
        'GESTION'
    FROM #Gestiones AS g
    LEFT JOIN #PagosPorPromesa AS pp
        ON pp.nId_DocxCobrarOpe = g.nId_DocxCobrarOpe
    LEFT JOIN #PromesasNoAdjudicadas AS pna
        ON pna.nId_DocxCobrarOpe = g.nId_DocxCobrarOpe
    WHERE g.nId_Cartera = @CarteraId
      AND g.dDocCobOpe_FecIni < DATEADD(DAY, 1, CONVERT(DATETIME2(3), @FechaCorte))
      /* Replica la purga de datos de promesa incompletos. */
      AND NOT
      (
          g.es_promesa_datos = 0
          AND pp.nId_DocxCobrarOpe IS NULL
          AND
          (
              ISNULL(g.montoPromesa, 0) > 0
              OR g.dFechCompromisoPago IS NOT NULL
          )
      );

    /* Pagos que no fueron adjudicados a una promesa. */
    INSERT INTO #V2
    (
        ultima_fecha_pago,
        ultima_fecha_registro,
        cCli_Nombre,
        cCar_Nombre,
        nCampCar,
        nombre_asesor,
        anio,
        estado_pdp,
        nId_PersDeudor,
        indicador_equiv,
        montoPromesa,
        total_pagado,
        marca_promesa_valida,
        cNombre_Cargo,
        dFechCompromisoPago,
        dDocCobOpe_FecIni,
        nId_DocxCobrarOpe,
        detalle_gestion,
        nTelef_Nro,
        codigo_unidad_negocio,
        tipo_fila
    )
    SELECT
        p.dDoc_FecPago,
        p.dDoc_FecPago,
        p.cCli_Nombre,
        p.cCar_Nombre,
        p.nCampCar,
        'BOLSA',
        p.anio,
        '6. Pago Sin Promesa',
        p.nId_PersDeudor,
        'NC',
        NULL,
        p.nDoc_ImpPago,
        CONVERT(BIT, 0),
        'BOLSA',
        NULL,
        p.dDoc_FecPago,
        NULL,
        NULL,
        NULL,
        @Unidad,
        'PAGO_SIN_PROMESA'
    FROM #PagoBase AS p
    LEFT JOIN #PagosAsignados AS a
        ON a.id_unico = p.id_unico
    WHERE p.nId_Cartera = @CarteraId
      AND a.id_unico IS NULL
      AND p.dDoc_FecPago < DATEADD(DAY, 1, CONVERT(DATETIME2(3), @FechaCorte));

    FETCH NEXT FROM cur_cartera
    INTO @CarteraId, @CarteraNombre, @Unidad;
END;

CLOSE cur_cartera;
DEALLOCATE cur_cartera;

CREATE INDEX IX_V2_Operacion
    ON #V2(nId_DocxCobrarOpe);
CREATE INDEX IX_V2_CarteraDeudorFecha
    ON #V2(cCar_Nombre, nId_PersDeudor, dDocCobOpe_FecIni);

/* ==========================================================================
   9. OLD - SNAPSHOT DE COMPARACION
   ========================================================================== */

DROP TABLE IF EXISTS #Old;

SELECT
    CONVERT(DATETIME2(3), f.ultima_fecha_pago) AS ultima_fecha_pago,
    CONVERT(DATETIME2(3), f.ultima_fecha_registro) AS ultima_fecha_registro,
    CONVERT(VARCHAR(150), f.cCli_Nombre) COLLATE DATABASE_DEFAULT AS cCli_Nombre,
    CONVERT(VARCHAR(150), f.cCar_Nombre) COLLATE DATABASE_DEFAULT AS cCar_Nombre,
    f.nCampCar,
    CONVERT(VARCHAR(150), f.nombre_asesor) COLLATE DATABASE_DEFAULT AS nombre_asesor,
    f.anio,
    CONVERT(VARCHAR(100), f.estado_pdp) COLLATE DATABASE_DEFAULT AS estado_pdp,
    f.nId_PersDeudor,
    CONVERT(VARCHAR(100), f.indicador_equiv) COLLATE DATABASE_DEFAULT AS indicador_equiv,
    CONVERT(DECIMAL(38,4), f.montoPromesa) AS montoPromesa,
    CONVERT(DECIMAL(38,4), f.total_pagado) AS total_pagado,
    CONVERT(BIT, ISNULL(f.marca_promesa_valida, 0)) AS marca_promesa_valida,
    CONVERT(VARCHAR(150), f.cNombre_Cargo) COLLATE DATABASE_DEFAULT AS cNombre_Cargo,
    CONVERT(DATE, f.dFechCompromisoPago) AS dFechCompromisoPago,
    CONVERT(DATETIME2(3), f.dDocCobOpe_FecIni) AS dDocCobOpe_FecIni,
    CONVERT(BIGINT, f.nId_DocxCobrarOpe) AS nId_DocxCobrarOpe,
    CONVERT(VARCHAR(500), f.detalle_gestion) COLLATE DATABASE_DEFAULT AS detalle_gestion,
    CONVERT(VARCHAR(50), f.nTelef_Nro) COLLATE DATABASE_DEFAULT AS nTelef_Nro,
    CASE
        WHEN f.cCli_Nombre COLLATE DATABASE_DEFAULT = 'CLARO GOBIERNO'
            THEN 'GOBIERNO'
        ELSE 'ADMINISTRATIVO'
    END AS codigo_unidad_negocio,
    CASE WHEN f.nId_DocxCobrarOpe IS NULL
         THEN 'PAGO_SIN_PROMESA'
         ELSE 'GESTION'
    END AS tipo_fila
INTO #Old
FROM
    [172.23.1.180\MSSQLSERVER,51601]
    .[aval_reporteria].[dbo].[vw_bi_gerencia_gestiones_pagos] AS f
WHERE f.cCli_Nombre COLLATE DATABASE_DEFAULT IN
      (
          'CLARO CORPORATIVO' COLLATE DATABASE_DEFAULT,
          'CLARO GOBIERNO' COLLATE DATABASE_DEFAULT
      )
  AND f.anio = @Anio
  AND f.nCampCar = @Mes
  AND f.dDocCobOpe_FecIni < DATEADD(DAY, 1, CONVERT(DATETIME2(3), @FechaCorte));

CREATE INDEX IX_Old_Operacion
    ON #Old(nId_DocxCobrarOpe);
CREATE INDEX IX_Old_CarteraDeudorFecha
    ON #Old(cCar_Nombre, nId_PersDeudor, dDocCobOpe_FecIni);

/* ==========================================================================
   10. RESULTADOS DE VALIDACION
   ========================================================================== */

/* A. Alcance y configuración efectiva. */
SELECT
    @IdCliente AS nId_Cliente,
    @IdContrato AS nId_Contrato,
    @IdGrupo AS nId_Grupo,
    @Anio AS anio,
    @Mes AS mes,
    @FechaCorte AS fecha_corte,
    @TipoCambio AS tipo_cambio_monto_promesa,
    @FactorTecho AS factor_techo_promesa,
    COUNT(*) AS carteras,
    SUM(CASE WHEN codigo_unidad_negocio = 'ADMINISTRATIVO' THEN 1 ELSE 0 END)
        AS carteras_administrativo,
    SUM(CASE WHEN codigo_unidad_negocio = 'GOBIERNO' THEN 1 ELSE 0 END)
        AS carteras_gobierno
FROM #Carteras;

/* B. Volumen OLD vs V2 por tipo de fila / BU. */
SELECT
    fuente,
    codigo_unidad_negocio,
    tipo_fila,
    filas,
    operaciones,
    deudores,
    monto_promesas,
    monto_pagado
FROM
(
    SELECT
        'OLD' AS fuente,
        codigo_unidad_negocio,
        tipo_fila,
        COUNT_BIG(*) AS filas,
        COUNT_BIG(DISTINCT nId_DocxCobrarOpe) AS operaciones,
        COUNT_BIG(DISTINCT nId_PersDeudor) AS deudores,
        SUM(ISNULL(montoPromesa, 0)) AS monto_promesas,
        SUM(ISNULL(total_pagado, 0)) AS monto_pagado
    FROM #Old
    GROUP BY codigo_unidad_negocio, tipo_fila

    UNION ALL

    SELECT
        'V2',
        codigo_unidad_negocio,
        tipo_fila,
        COUNT_BIG(*),
        COUNT_BIG(DISTINCT nId_DocxCobrarOpe),
        COUNT_BIG(DISTINCT nId_PersDeudor),
        SUM(ISNULL(montoPromesa, 0)),
        SUM(ISNULL(total_pagado, 0))
    FROM #V2
    GROUP BY codigo_unidad_negocio, tipo_fila
) AS x
ORDER BY codigo_unidad_negocio, tipo_fila, fuente;

/* C. Paridad de IDs de operación. */
SELECT
    SUM(CASE WHEN v.nId_DocxCobrarOpe IS NOT NULL
              AND o.nId_DocxCobrarOpe IS NOT NULL THEN 1 ELSE 0 END)
        AS operaciones_en_ambos,
    SUM(CASE WHEN v.nId_DocxCobrarOpe IS NOT NULL
              AND o.nId_DocxCobrarOpe IS NULL THEN 1 ELSE 0 END)
        AS solo_v2,
    SUM(CASE WHEN v.nId_DocxCobrarOpe IS NULL
              AND o.nId_DocxCobrarOpe IS NOT NULL THEN 1 ELSE 0 END)
        AS solo_old
FROM
(
    SELECT DISTINCT nId_DocxCobrarOpe
    FROM #V2
    WHERE nId_DocxCobrarOpe IS NOT NULL
) AS v
FULL OUTER JOIN
(
    SELECT DISTINCT nId_DocxCobrarOpe
    FROM #Old
    WHERE nId_DocxCobrarOpe IS NOT NULL
) AS o
    ON o.nId_DocxCobrarOpe = v.nId_DocxCobrarOpe;

/* D. Paridad campo a campo para operaciones comunes. */
SELECT
    COUNT_BIG(*) AS operaciones_comunes,

    SUM(CASE WHEN ISNULL(v.nId_PersDeudor, -1) = ISNULL(o.nId_PersDeudor, -1)
             THEN 1 ELSE 0 END) AS mismo_deudor,

    SUM(CASE WHEN ISNULL(v.cCar_Nombre, '') COLLATE DATABASE_DEFAULT
                   = ISNULL(o.cCar_Nombre, '') COLLATE DATABASE_DEFAULT
             THEN 1 ELSE 0 END) AS misma_cartera,

    SUM(CASE WHEN ISNULL(v.indicador_equiv, '') COLLATE DATABASE_DEFAULT
                   = ISNULL(o.indicador_equiv, '') COLLATE DATABASE_DEFAULT
             THEN 1 ELSE 0 END) AS mismo_indicador,

    SUM(CASE WHEN ISNULL(v.dDocCobOpe_FecIni, CONVERT(DATETIME2(3), '19000101'))
                   = ISNULL(o.dDocCobOpe_FecIni, CONVERT(DATETIME2(3), '19000101'))
             THEN 1 ELSE 0 END) AS misma_fecha_gestion,

    SUM(CASE WHEN ISNULL(v.dFechCompromisoPago, CONVERT(DATE, '19000101'))
                   = ISNULL(o.dFechCompromisoPago, CONVERT(DATE, '19000101'))
             THEN 1 ELSE 0 END) AS misma_fecha_compromiso,

    SUM(CASE WHEN ISNULL(v.montoPromesa, CONVERT(DECIMAL(38,4), -1))
                   = ISNULL(o.montoPromesa, CONVERT(DECIMAL(38,4), -1))
             THEN 1 ELSE 0 END) AS mismo_monto_promesa,

    SUM(CASE WHEN ISNULL(v.total_pagado, CONVERT(DECIMAL(38,4), -1))
                   = ISNULL(o.total_pagado, CONVERT(DECIMAL(38,4), -1))
             THEN 1 ELSE 0 END) AS mismo_total_pagado,

    SUM(CASE WHEN v.marca_promesa_valida = o.marca_promesa_valida
             THEN 1 ELSE 0 END) AS misma_marca_promesa,

    SUM(CASE WHEN ISNULL(v.estado_pdp, '') COLLATE DATABASE_DEFAULT
                   = ISNULL(o.estado_pdp, '') COLLATE DATABASE_DEFAULT
             THEN 1 ELSE 0 END) AS mismo_estado_pdp

FROM #V2 AS v
INNER JOIN #Old AS o
    ON o.nId_DocxCobrarOpe = v.nId_DocxCobrarOpe
WHERE v.nId_DocxCobrarOpe IS NOT NULL;

/* E. Distribución de estados OLD vs V2. */
SELECT
    fuente,
    codigo_unidad_negocio,
    estado_pdp,
    cantidad,
    monto_promesas,
    monto_pagado
FROM
(
    SELECT
        'OLD' AS fuente,
        codigo_unidad_negocio,
        estado_pdp,
        COUNT_BIG(*) AS cantidad,
        SUM(ISNULL(montoPromesa, 0)) AS monto_promesas,
        SUM(ISNULL(total_pagado, 0)) AS monto_pagado
    FROM #Old
    GROUP BY codigo_unidad_negocio, estado_pdp

    UNION ALL

    SELECT
        'V2',
        codigo_unidad_negocio,
        estado_pdp,
        COUNT_BIG(*),
        SUM(ISNULL(montoPromesa, 0)),
        SUM(ISNULL(total_pagado, 0))
    FROM #V2
    GROUP BY codigo_unidad_negocio, estado_pdp
) AS x
ORDER BY codigo_unidad_negocio, estado_pdp, fuente;

/* F. Primeras diferencias de contenido para operaciones comunes. */
SELECT TOP (300)
    v.nId_DocxCobrarOpe,
    v.codigo_unidad_negocio,
    v.cCar_Nombre,
    v.nId_PersDeudor,

    v.indicador_equiv AS v2_indicador,
    o.indicador_equiv AS old_indicador,

    v.montoPromesa AS v2_monto_promesa,
    o.montoPromesa AS old_monto_promesa,

    v.total_pagado AS v2_total_pagado,
    o.total_pagado AS old_total_pagado,

    v.marca_promesa_valida AS v2_promesa_valida,
    o.marca_promesa_valida AS old_promesa_valida,

    v.estado_pdp AS v2_estado_pdp,
    o.estado_pdp AS old_estado_pdp,

    v.dDocCobOpe_FecIni AS v2_fecha_gestion,
    o.dDocCobOpe_FecIni AS old_fecha_gestion,

    v.dFechCompromisoPago AS v2_fecha_compromiso,
    o.dFechCompromisoPago AS old_fecha_compromiso

FROM #V2 AS v
INNER JOIN #Old AS o
    ON o.nId_DocxCobrarOpe = v.nId_DocxCobrarOpe
WHERE v.nId_DocxCobrarOpe IS NOT NULL
  AND
  (
       ISNULL(v.indicador_equiv, '') COLLATE DATABASE_DEFAULT
           <> ISNULL(o.indicador_equiv, '') COLLATE DATABASE_DEFAULT
    OR ISNULL(v.montoPromesa, CONVERT(DECIMAL(38,4), -1))
           <> ISNULL(o.montoPromesa, CONVERT(DECIMAL(38,4), -1))
    OR ISNULL(v.total_pagado, CONVERT(DECIMAL(38,4), -1))
           <> ISNULL(o.total_pagado, CONVERT(DECIMAL(38,4), -1))
    OR v.marca_promesa_valida <> o.marca_promesa_valida
    OR ISNULL(v.estado_pdp, '') COLLATE DATABASE_DEFAULT
           <> ISNULL(o.estado_pdp, '') COLLATE DATABASE_DEFAULT
    OR ISNULL(v.dDocCobOpe_FecIni, CONVERT(DATETIME2(3), '19000101'))
           <> ISNULL(o.dDocCobOpe_FecIni, CONVERT(DATETIME2(3), '19000101'))
    OR ISNULL(v.dFechCompromisoPago, CONVERT(DATE, '19000101'))
           <> ISNULL(o.dFechCompromisoPago, CONVERT(DATE, '19000101'))
  )
ORDER BY v.nId_DocxCobrarOpe;

/* G. Solo V2: ayuda a separar late arrivals / dedup / reglas faltantes. */
SELECT TOP (300)
    v.*
FROM #V2 AS v
LEFT JOIN #Old AS o
    ON o.nId_DocxCobrarOpe = v.nId_DocxCobrarOpe
WHERE v.nId_DocxCobrarOpe IS NOT NULL
  AND o.nId_DocxCobrarOpe IS NULL
ORDER BY v.nId_DocxCobrarOpe;

/* H. Solo OLD. Debe tender a cero una vez alineado el mismo corte. */
SELECT TOP (300)
    o.*
FROM #Old AS o
LEFT JOIN #V2 AS v
    ON v.nId_DocxCobrarOpe = o.nId_DocxCobrarOpe
WHERE o.nId_DocxCobrarOpe IS NOT NULL
  AND v.nId_DocxCobrarOpe IS NULL
ORDER BY o.nId_DocxCobrarOpe;

/* I. Pago sin promesa: comparar agregado porque no tiene id de operación. */
SELECT
    fuente,
    codigo_unidad_negocio,
    COUNT_BIG(*) AS filas_pago_sin_promesa,
    COUNT_BIG(DISTINCT nId_PersDeudor) AS deudores,
    SUM(ISNULL(total_pagado, 0)) AS total_pagado
FROM
(
    SELECT
        'OLD' AS fuente,
        codigo_unidad_negocio,
        nId_PersDeudor,
        total_pagado
    FROM #Old
    WHERE nId_DocxCobrarOpe IS NULL
      AND estado_pdp = '6. Pago Sin Promesa'

    UNION ALL

    SELECT
        'V2',
        codigo_unidad_negocio,
        nId_PersDeudor,
        total_pagado
    FROM #V2
    WHERE nId_DocxCobrarOpe IS NULL
      AND estado_pdp = '6. Pago Sin Promesa'
) AS x
GROUP BY fuente, codigo_unidad_negocio
ORDER BY codigo_unidad_negocio, fuente;

/* J. Sanidad V2. */
SELECT
    (SELECT COUNT(*) FROM #Carteras) AS carteras,
    (SELECT COUNT_BIG(*) FROM #Documentos) AS documentos,
    (SELECT COUNT_BIG(*) FROM #GestionesBase) AS gestiones_origen,
    (SELECT COUNT_BIG(*) FROM #GestionesElegibles) AS gestiones_tras_filtro_tecnico,
    (SELECT COUNT_BIG(*) FROM #GestionesDedup) AS gestiones_tras_dedup,
    (SELECT COUNT_BIG(*) FROM #Gestiones WHERE es_promesa = 1) AS promesas_validas,
    (SELECT COUNT_BIG(*) FROM #Pagos) AS pagos_origen,
    (SELECT COUNT_BIG(*) FROM #PagoBase) AS pagos_deudor_fecha,
    (SELECT COUNT_BIG(*) FROM #PagosAsignados) AS pagos_adjudicados,
    (SELECT COUNT_BIG(*) FROM #V2) AS filas_v2,
    (SELECT COUNT_BIG(*) FROM #Old) AS filas_old;
GO
