using GesMgmt.Domain.Entities.Analitica;
using GesMgmt.Domain.Entities.Analitica.CentroControlCartera;
using GesMgmt.Domain.Entities.Analitica.SesionesPowerBi;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Persistence
{
    public class AnaliticaDbContext : DbContext
    {
        public DbSet<ConfiguracionOpcionAnalitica> ConfiguracionesOpcionAnalitica { get; set; }
        public DbSet<AlcanceOpcionClienteAnalitica> AlcancesOpcionClienteAnalitica { get; set; }
        public DbSet<AlcanceOpcionGrupoAnalitica> AlcancesOpcionGrupoAnalitica { get; set; }
        public DbSet<AlcanceUsuarioOpcionAnalitica> AlcancesUsuarioOpcionAnalitica { get; set; }
        public DbSet<DimensionClienteAnalitica> ClientesAnalitica { get; set; }
        public DbSet<DimensionCampanaAnalitica> CampanasAnalitica { get; set; }
        public DbSet<ResumenDiarioCampanaAnalitica> ResumenesDiariosCampanaAnalitica { get; set; }
        public DbSet<EvolucionDiariaCampanaAnalitica> EvolucionDiariaCampanaAnalitica { get; set; }
        public DbSet<EvolucionDiariaCarteraAnalitica> EvolucionDiariaCarteraAnalitica { get; set; }
        public DbSet<HechoDiarioCarteraAnalitica> HechosDiariosCarteraAnalitica { get; set; }
        public DbSet<HechoEvolucionDiariaCarteraAnalitica> HechosEvolucionDiariaCarteraAnalitica { get; set; }
        public DbSet<DimensionCarteraAnalitica> CarterasAnalitica { get; set; }
        public DbSet<DimensionFechaAnalitica> FechasAnalitica { get; set; }
        public DbSet<AtribucionDiariaSupervisorAsesorAnalitica> AtribucionesDiariasSupervisorAsesorAnalitica { get; set; }
        public DbSet<ContactoDiarioDeudorSupervisorAnalitica> ContactoDiarioDeudorSupervisorAnalitica { get; set; }
        public DbSet<PromesaOperativaSupervisorAnalitica> PromesaOperativaSupervisorAnalitica { get; set; }
        public DbSet<PagoDiarioDeudorSupervisorAnalitica> PagoDiarioDeudorSupervisorAnalitica { get; set; }
        public DbSet<SupervisorActualAsesorAnalitica> SupervisorActualAsesorAnalitica { get; set; }
        public DbSet<EstadoResumenDiarioCarteraAnalitica> EstadoResumenDiarioCarteraAnalitica { get; set; }
        public DbSet<MetricaDiariaCarteraAnalitica> MetricasDiariasCarteraAnalitica { get; set; }
        public DbSet<HechoMetaMensualAnalitica> HechosMetaMensualAnalitica { get; set; }
        public DbSet<HechoContactoDiarioDeudorAnalitica> HechosContactoDiarioDeudorAnalitica { get; set; }
        public DbSet<HechoPromesaAnalitica> PromesasAnalitica { get; set; }
        public DbSet<HechoPagoDiarioDeudorAnalitica> HechosPagoDiarioDeudorAnalitica { get; set; }
        public DbSet<ControlCargaAnalitica> ControlesCargaAnalitica { get; set; }
        public DbSet<AvanceMetaCampanaAnalitica> AvanceMetaCampanaAnalitica { get; set; }
        public DbSet<PromesaOperativaAnalitica> PromesaOperativaAnalitica { get; set; }
        public DbSet<AnaliticaAlcanceReporteClienteEntrada> AlcancesReporteClienteAnalitica { get; set; }
        public DbSet<AnaliticaIncrustacionReporteClienteEntrada> IncrustacionesReporteClienteAnalitica { get; set; }
        public DbSet<AnaliticaCatalogoReporteClienteEntrada> CatalogoReporteClienteAnalitica { get; set; }
        public DbSet<SesionPowerBiAnalitica> SesionesPowerBiAnalitica { get; set; }
        public DbSet<EventoSesionPowerBiAnalitica> EventosSesionPowerBiAnalitica { get; set; }

        public AnaliticaDbContext(DbContextOptions<AnaliticaDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConfiguracionOpcionAnalitica>(builder =>
            {
                builder.ToTable("configuracion_opcion", "acceso_analitica");
                builder.HasKey(option => option.IdOpcion);
                builder.Property(option => option.IdOpcion)
                    .HasColumnName("id_opcion")
                    .ValueGeneratedNever();
                builder.Property(option => option.CodigoOpcion).HasColumnName("codigo_opcion");
                builder.Property(option => option.NombreOpcion).HasColumnName("nombre_opcion");
                builder.Property(option => option.EsActivo).HasColumnName("es_activo");
                builder.Property(option => option.CreadoPor).HasColumnName("creado_por");
                builder.Property(option => option.FechaCreacion).HasColumnName("fecha_creacion");
                builder.Property(option => option.ActualizadoPor).HasColumnName("actualizado_por");
                builder.Property(option => option.FechaActualizacion).HasColumnName("fecha_actualizacion");
            });

            modelBuilder.Entity<AlcanceOpcionClienteAnalitica>(builder =>
            {
                builder.ToTable("alcance_opcion_cliente", "acceso_analitica");
                builder.HasKey(scope => new { scope.IdOpcion, scope.IdClienteCrm });
                builder.Property(scope => scope.IdOpcion).HasColumnName("id_opcion");
                builder.Property(scope => scope.IdClienteCrm).HasColumnName("id_cliente_crm");
                builder.Property(scope => scope.EsActivo).HasColumnName("es_activo");
                builder.Property(scope => scope.CreadoPor).HasColumnName("creado_por");
                builder.Property(scope => scope.FechaCreacion).HasColumnName("fecha_creacion");
                builder.Property(scope => scope.ActualizadoPor).HasColumnName("actualizado_por");
                builder.Property(scope => scope.FechaActualizacion).HasColumnName("fecha_actualizacion");
            });

            modelBuilder.Entity<AlcanceOpcionGrupoAnalitica>(builder =>
            {
                builder.ToTable("alcance_opcion_grupo", "acceso_analitica");
                builder.HasKey(scope => new { scope.IdOpcion, scope.IdGrupoSisges });
                builder.Property(scope => scope.IdOpcion).HasColumnName("id_opcion");
                builder.Property(scope => scope.IdGrupoSisges).HasColumnName("id_grupo_sisges");
                builder.Property(scope => scope.EsActivo).HasColumnName("es_activo");
                builder.Property(scope => scope.CreadoPor).HasColumnName("creado_por");
                builder.Property(scope => scope.FechaCreacion).HasColumnName("fecha_creacion");
                builder.Property(scope => scope.ActualizadoPor).HasColumnName("actualizado_por");
                builder.Property(scope => scope.FechaActualizacion).HasColumnName("fecha_actualizacion");
            });

            modelBuilder.Entity<AlcanceUsuarioOpcionAnalitica>(builder =>
            {
                builder.ToTable("alcance_usuario_opcion", "acceso_analitica");
                builder.HasKey(scope => new { scope.IdUsuario, scope.IdOpcion });
                builder.Property(scope => scope.IdUsuario).HasColumnName("id_usuario");
                builder.Property(scope => scope.IdOpcion).HasColumnName("id_opcion");
                builder.Property(scope => scope.EsActivo).HasColumnName("es_activo");
                builder.Property(scope => scope.CreadoPor).HasColumnName("creado_por");
                builder.Property(scope => scope.FechaCreacion).HasColumnName("fecha_creacion");
                builder.Property(scope => scope.ActualizadoPor).HasColumnName("actualizado_por");
                builder.Property(scope => scope.FechaActualizacion).HasColumnName("fecha_actualizacion");
            });

            modelBuilder.Entity<DimensionClienteAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("dim_cliente", "analitica");
                builder.Property(client => client.ClaveCliente).HasColumnName("clave_cliente");
                builder.Property(client => client.IdClienteCrm).HasColumnName("id_cliente_crm");
                builder.Property(client => client.CodigoCliente).HasColumnName("codigo_cliente");
                builder.Property(client => client.NombreCliente).HasColumnName("nombre_cliente");
            });

            modelBuilder.Entity<DimensionCampanaAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("dim_campana", "analitica");
                builder.Property(campana => campana.ClaveCampana).HasColumnName("clave_campana");
                builder.Property(campana => campana.ClaveCliente).HasColumnName("clave_cliente");
                builder.Property(campana => campana.CodigoCampana).HasColumnName("codigo_campana");
                builder.Property(campana => campana.NombreCampana).HasColumnName("nombre_campana");
                builder.Property(campana => campana.FechaInicio).HasColumnName("fecha_inicio");
                builder.Property(campana => campana.FechaFin).HasColumnName("fecha_fin");
            });

            modelBuilder.Entity<ResumenDiarioCampanaAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_resumen_diario_campana", "analitica");
                builder.Property(row => row.ClaveCliente).HasColumnName("clave_cliente");
                builder.Property(row => row.ClaveCampana).HasColumnName("clave_campana");
                builder.Property(row => row.FechaCalendario).HasColumnName("fecha_calendario");
                builder.Property(row => row.FechaCarga).HasColumnName("fecha_carga");
            });

            modelBuilder.Entity<EvolucionDiariaCampanaAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_evolucion_diaria_campana", "analitica");
                builder.Property(row => row.ClaveCliente).HasColumnName("clave_cliente");
                builder.Property(row => row.ClaveCampana).HasColumnName("clave_campana");
                builder.Property(row => row.FechaCalendario).HasColumnName("fecha_calendario");
                builder.Property(row => row.ClientesAsignados).HasColumnName("clientes_asignados");
                builder.Property(row => row.ClientesGestionados).HasColumnName("clientes_gestionados");
                builder.Property(row => row.ClientesPendientes).HasColumnName("clientes_pendientes");
                builder.Property(row => row.ClientesContactados).HasColumnName("clientes_contactados");
                builder.Property(row => row.MontoRecuperadoAcumulado).HasColumnName("monto_recuperado_acumulado");
                builder.Property(row => row.FechaCarga).HasColumnName("fecha_carga");
            });

            modelBuilder.Entity<EvolucionDiariaCarteraAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_evolucion_diaria_cartera", "analitica");
                builder.Property(row => row.ClaveCliente).HasColumnName("clave_cliente");
                builder.Property(row => row.ClaveCampana).HasColumnName("clave_campana");
                builder.Property(row => row.ClaveCartera).HasColumnName("clave_cartera");
                builder.Property(row => row.FechaCalendario).HasColumnName("fecha_calendario");
                builder.Property(row => row.ClientesAsignados).HasColumnName("clientes_asignados");
                builder.Property(row => row.ClientesGestionados).HasColumnName("clientes_gestionados");
                builder.Property(row => row.ClientesPendientes).HasColumnName("clientes_pendientes");
                builder.Property(row => row.MontoRecuperadoAcumulado).HasColumnName("monto_recuperado_acumulado");
                builder.Property(row => row.FechaCarga).HasColumnName("fecha_carga");
            });

            modelBuilder.Entity<HechoDiarioCarteraAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("hecho_cartera_diario", "analitica");
                builder.Property(row => row.ClaveCliente).HasColumnName("clave_cliente");
                builder.Property(row => row.ClaveCampana).HasColumnName("clave_campana");
                builder.Property(row => row.ClaveCartera).HasColumnName("clave_cartera");
                builder.Property(row => row.ClaveFecha).HasColumnName("clave_fecha");
                builder.Property(row => row.ClientesAsignadosCorte).HasColumnName("clientes_asignados_corte");
                builder.Property(row => row.ClientesGestionadosCorte).HasColumnName("clientes_gestionados_corte");
                builder.Property(row => row.ClientesPendientesCorte).HasColumnName("clientes_pendientes_corte");
                builder.Property(row => row.ClientesContactadosCorte).HasColumnName("clientes_contactados_corte");
                builder.Property(row => row.TieneCorteOrigen).HasColumnName("tiene_corte_origen");
                builder.Property(row => row.FechaCarga).HasColumnName("fecha_carga");
            });

            modelBuilder.Entity<HechoEvolucionDiariaCarteraAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("hecho_evolucion_cartera_diario", "analitica");
                builder.Property(row => row.ClaveCliente).HasColumnName("clave_cliente");
                builder.Property(row => row.ClaveCampana).HasColumnName("clave_campana");
                builder.Property(row => row.ClaveCartera).HasColumnName("clave_cartera");
                builder.Property(row => row.ClaveFecha).HasColumnName("clave_fecha");
                builder.Property(row => row.ClientesAsignados).HasColumnName("clientes_asignados");
                builder.Property(row => row.ClientesGestionados).HasColumnName("clientes_gestionados");
                builder.Property(row => row.ClientesPendientes).HasColumnName("clientes_pendientes");
                builder.Property(row => row.FechaCarga).HasColumnName("fecha_carga");
            });

            modelBuilder.Entity<DimensionCarteraAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("dim_cartera", "analitica");
                builder.Property(portfolio => portfolio.ClaveCartera).HasColumnName("clave_cartera");
                builder.Property(portfolio => portfolio.IdCarteraOrigen).HasColumnName("id_cartera_origen");
                builder.Property(portfolio => portfolio.NombreCartera).HasColumnName("nombre_cartera");
                builder.Property(portfolio => portfolio.UnidadNegocioOrigen).HasColumnName("unidad_negocio_origen");
            });

            modelBuilder.Entity<DimensionFechaAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("dim_fecha", "analitica");
                builder.Property(date => date.ClaveFecha).HasColumnName("clave_fecha");
                builder.Property(date => date.FechaCalendario).HasColumnName("fecha_calendario");
            });

            modelBuilder.Entity<AtribucionDiariaSupervisorAsesorAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_asignacion_diaria_supervisor_asesor", "analitica");
                builder.Property(row => row.ClaveCliente).HasColumnName("clave_cliente");
                builder.Property(row => row.ClaveCampana).HasColumnName("clave_campana");
                builder.Property(row => row.ClaveCartera).HasColumnName("clave_cartera");
                builder.Property(row => row.ClaveAsesor).HasColumnName("clave_asesor");
                builder.Property(row => row.NombreAsesor).HasColumnName("nombre_asesor");
                builder.Property(row => row.ClaveSupervisor).HasColumnName("clave_supervisor");
                builder.Property(row => row.NombreSupervisor).HasColumnName("nombre_supervisor");
                builder.Property(row => row.FechaCalendario).HasColumnName("fecha_calendario");
                builder.Property(row => row.EventosGestion).HasColumnName("eventos_gestion");
                builder.Property(row => row.MontoRecuperado).HasColumnName("monto_recuperado");
                builder.Property(row => row.FechaCarga).HasColumnName("fecha_carga");
            });

            modelBuilder.Entity<ContactoDiarioDeudorSupervisorAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_contacto_diario_deudor_supervisor", "analitica");
                builder.Property(row => row.ClaveCliente).HasColumnName("clave_cliente");
                builder.Property(row => row.ClaveCampana).HasColumnName("clave_campana");
                builder.Property(row => row.ClaveCartera).HasColumnName("clave_cartera");
                builder.Property(row => row.IdDeudorOrigen).HasColumnName("id_deudor_origen");
                builder.Property(row => row.ClaveAsesor).HasColumnName("clave_asesor");
                builder.Property(row => row.ClaveSupervisor).HasColumnName("clave_supervisor");
                builder.Property(row => row.FechaCalendario).HasColumnName("fecha_calendario");
                builder.Property(row => row.TuvoContactoDirecto).HasColumnName("indicador_contacto_directo");
                builder.Property(row => row.TuvoContactoIndirecto).HasColumnName("indicador_contacto_indirecto");
                builder.Property(row => row.TuvoSinContacto).HasColumnName("indicador_sin_contacto");
                builder.Property(row => row.FechaCarga).HasColumnName("fecha_carga");
            });

            modelBuilder.Entity<PromesaOperativaSupervisorAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_promesa_operativa_supervisor", "analitica");
                builder.Property(row => row.ClaveHechoPromesa).HasColumnName("clave_hecho_promesa");
                builder.Property(row => row.ClaveSupervisorAsesor).HasColumnName("clave_supervisor_asesor");
                builder.Property(row => row.ClaveCliente).HasColumnName("clave_cliente");
                builder.Property(row => row.ClaveCampana).HasColumnName("clave_campana");
                builder.Property(row => row.ClaveCartera).HasColumnName("clave_cartera");
                builder.Property(row => row.IdDeudorOrigen).HasColumnName("id_deudor_origen");
                builder.Property(row => row.FechaVencimientoPromesa).HasColumnName("fecha_vencimiento_promesa");
                builder.Property(row => row.ClaveAsesor).HasColumnName("clave_asesor");
                builder.Property(row => row.NombreAsesor).HasColumnName("nombre_asesor");
                builder.Property(row => row.ClaveSupervisor).HasColumnName("clave_supervisor");
                builder.Property(row => row.NombreSupervisor).HasColumnName("nombre_supervisor");
                builder.Property(row => row.EsPromesaValida).HasColumnName("es_promesa_valida");
                builder.Property(row => row.FechaGestion).HasColumnName("fecha_hora_gestion");
                builder.Property(row => row.CodigoEstado).HasColumnName("codigo_estado");
                builder.Property(row => row.MontoPagado).HasColumnName("monto_pagado");
                builder.Property(row => row.MontoPromesa).HasColumnName("monto_promesa");
                builder.Property(row => row.FechaCarga).HasColumnName("fecha_carga");
            });

            modelBuilder.Entity<PagoDiarioDeudorSupervisorAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_pago_diario_deudor_supervisor", "analitica");
                builder.Property(row => row.ClaveCliente).HasColumnName("clave_cliente");
                builder.Property(row => row.ClaveCampana).HasColumnName("clave_campana");
                builder.Property(row => row.ClaveCartera).HasColumnName("clave_cartera");
                builder.Property(row => row.IdDeudorOrigen).HasColumnName("id_deudor_origen");
                builder.Property(row => row.ClaveAsesor).HasColumnName("clave_asesor");
                builder.Property(row => row.ClaveSupervisor).HasColumnName("clave_supervisor");
                builder.Property(row => row.FechaCalendario).HasColumnName("fecha_calendario");
                builder.Property(row => row.FechaCarga).HasColumnName("fecha_carga");
            });

            modelBuilder.Entity<SupervisorActualAsesorAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_supervisor_actual_asesor", "analitica");
                builder.Property(row => row.ClaveCliente).HasColumnName("clave_cliente");
                builder.Property(row => row.ClaveAsesor).HasColumnName("clave_asesor");
                builder.Property(row => row.ClaveSupervisor).HasColumnName("clave_supervisor");
                builder.Property(row => row.NombreSupervisor).HasColumnName("nombre_supervisor");
            });

            modelBuilder.Entity<EstadoResumenDiarioCarteraAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_estado_resumen_diario_cartera", "analitica");
                builder.Property(row => row.ClaveCliente).HasColumnName("clave_cliente");
                builder.Property(row => row.ClaveCampana).HasColumnName("clave_campana");
                builder.Property(row => row.ClaveCartera).HasColumnName("clave_cartera");
                builder.Property(row => row.FechaCalendario).HasColumnName("fecha_calendario");
            });

            modelBuilder.Entity<MetricaDiariaCarteraAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_metricas_diarias_cartera", "analitica");
                builder.Property(row => row.ClaveCliente).HasColumnName("clave_cliente");
                builder.Property(row => row.ClaveCampana).HasColumnName("clave_campana");
                builder.Property(row => row.ClaveCartera).HasColumnName("clave_cartera");
                builder.Property(row => row.FechaCalendario).HasColumnName("fecha_calendario");
                builder.Property(row => row.TieneCorteOrigen).HasColumnName("tiene_corte_origen");
                builder.Property(row => row.ClientesAsignadosCorte).HasColumnName("clientes_asignados_corte");
                builder.Property(row => row.ClientesGestionadosCorte).HasColumnName("clientes_gestionados_corte");
                builder.Property(row => row.ClientesPendientesCorte).HasColumnName("clientes_pendientes_corte");
                builder.Property(row => row.ClientesContactadosCorte).HasColumnName("clientes_contactados_corte");
                builder.Property(row => row.EventosGestionDia).HasColumnName("eventos_gestion_dia");
                builder.Property(row => row.MontoRecuperadoDia).HasColumnName("monto_recuperado_dia");
                builder.Property(row => row.FechaCarga).HasColumnName("fecha_carga");
            });

            modelBuilder.Entity<HechoMetaMensualAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("hecho_meta_mensual", "analitica");
                builder.Property(row => row.ClaveCliente).HasColumnName("clave_cliente");
                builder.Property(row => row.ClaveCampana).HasColumnName("clave_campana");
                builder.Property(row => row.ClaveCartera).HasColumnName("clave_cartera");
                builder.Property(row => row.MontoMetaRecuperacion).HasColumnName("meta_monto_recuperado");
                builder.Property(row => row.FechaCorteOrigen).HasColumnName("fecha_corte_origen");
            });

            modelBuilder.Entity<HechoContactoDiarioDeudorAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("hecho_contacto_deudor_diario", "analitica");
                builder.Property(row => row.ClaveCliente).HasColumnName("clave_cliente");
                builder.Property(row => row.ClaveCampana).HasColumnName("clave_campana");
                builder.Property(row => row.ClaveCartera).HasColumnName("clave_cartera");
                builder.Property(row => row.IdDeudorOrigen).HasColumnName("id_deudor_origen");
                builder.Property(row => row.ClaveFecha).HasColumnName("clave_fecha");
                builder.Property(row => row.TuvoContactoDirecto).HasColumnName("indicador_contacto_directo");
                builder.Property(row => row.TuvoContactoIndirecto).HasColumnName("indicador_contacto_indirecto");
                builder.Property(row => row.TuvoSinContacto).HasColumnName("indicador_sin_contacto");
                builder.Property(row => row.FechaCarga).HasColumnName("fecha_carga");
            });

            modelBuilder.Entity<HechoPromesaAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("hecho_promesa", "analitica");
                builder.Property(row => row.ClaveCliente).HasColumnName("clave_cliente");
                builder.Property(row => row.ClaveCampana).HasColumnName("clave_campana");
                builder.Property(row => row.ClaveCartera).HasColumnName("clave_cartera");
                builder.Property(row => row.IdDeudorOrigen).HasColumnName("id_deudor_origen");
                builder.Property(row => row.EsPromesaValida).HasColumnName("es_promesa_valida");
                builder.Property(row => row.FechaGestion).HasColumnName("fecha_hora_gestion");
                builder.Property(row => row.CodigoEstado).HasColumnName("codigo_estado");
                builder.Property(row => row.MontoPagado).HasColumnName("monto_pagado");
                builder.Property(row => row.MontoPromesa).HasColumnName("monto_promesa");
                builder.Property(row => row.FechaCarga).HasColumnName("fecha_carga");
            });

            modelBuilder.Entity<HechoPagoDiarioDeudorAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("hecho_pago_deudor_diario", "analitica");
                builder.Property(row => row.ClaveCliente).HasColumnName("clave_cliente");
                builder.Property(row => row.ClaveCampana).HasColumnName("clave_campana");
                builder.Property(row => row.ClaveCartera).HasColumnName("clave_cartera");
                builder.Property(row => row.IdDeudorOrigen).HasColumnName("id_deudor_origen");
                builder.Property(row => row.ClaveFecha).HasColumnName("clave_fecha");
                builder.Property(row => row.FechaCarga).HasColumnName("fecha_carga");
            });

            modelBuilder.Entity<ControlCargaAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("control_carga", "integracion_datos_analitica");
                builder.Property(row => row.CodigoOrigen).HasColumnName("codigo_origen");
                builder.Property(row => row.FechaHoraUltimoOrigen).HasColumnName("fecha_hora_ultimo_origen");
                builder.Property(row => row.FechaUltimoExito).HasColumnName("fecha_ultimo_exito");
            });

            modelBuilder.Entity<AvanceMetaCampanaAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_avance_meta_campana", "analitica");
                builder.Property(row => row.ClaveCliente).HasColumnName("clave_cliente");
                builder.Property(row => row.ClaveCampana).HasColumnName("clave_campana");
                builder.Property(row => row.FechaCalendario).HasColumnName("fecha_calendario");
                builder.Property(row => row.MontoMetaRecuperacion).HasColumnName("meta_monto_recuperado");
                builder.Property(row => row.MontoEsperadoAcumulado).HasColumnName("monto_recuperado_esperado_acumulado");
                builder.Property(row => row.FechaCorteMetaOrigen).HasColumnName("fecha_corte_origen_meta");
            });

            modelBuilder.Entity<PromesaOperativaAnalitica>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("v_promesa_operativa", "analitica");
                builder.Property(row => row.ClaveHechoPromesa).HasColumnName("clave_hecho_promesa");
                builder.Property(row => row.ClaveCliente).HasColumnName("clave_cliente");
                builder.Property(row => row.ClaveCampana).HasColumnName("clave_campana");
                builder.Property(row => row.ClaveCartera).HasColumnName("clave_cartera");
                builder.Property(row => row.IdDeudorOrigen).HasColumnName("id_deudor_origen");
                builder.Property(row => row.FechaVencimientoPromesa).HasColumnName("fecha_vencimiento_promesa");
                builder.Property(row => row.FechaUltimoPago).HasColumnName("fecha_ultimo_pago");
                builder.Property(row => row.ClaveAsesor).HasColumnName("clave_asesor");
                builder.Property(row => row.EsPromesaValida).HasColumnName("es_promesa_valida");
                builder.Property(row => row.CodigoEstado).HasColumnName("codigo_estado");
                builder.Property(row => row.VenceHoy).HasColumnName("vence_hoy");
                builder.Property(row => row.EstaRota).HasColumnName("es_incumplida");
                builder.Property(row => row.EstaCumplidaOParcial).HasColumnName("es_cumplida_o_parcial");
                builder.Property(row => row.MontoPromesa).HasColumnName("monto_promesa");
                builder.Property(row => row.MontoPagado).HasColumnName("monto_pagado");
                builder.Property(row => row.FechaCarga).HasColumnName("fecha_carga");
            });

            modelBuilder.Entity<AnaliticaAlcanceReporteClienteEntrada>(builder =>
            {
                builder.ToTable("alcance_cliente_reporte_power_bi", "acceso_analitica");
                builder.HasKey(scope => new
                {
                    scope.IdOpcion,
                    scope.IdClienteCrm,
                    scope.ClienteReporte,
                    scope.IdGrupoSisges
                });
                builder.Property(scope => scope.IdOpcion).HasColumnName("id_opcion");
                builder.Property(scope => scope.IdClienteCrm).HasColumnName("id_cliente_crm");
                builder.Property(scope => scope.ClienteReporte)
                    .HasColumnName("valor_cliente_reporte")
                    .HasMaxLength(150)
                    .IsUnicode(false);
                builder.Property(scope => scope.IdGrupoSisges).HasColumnName("id_grupo_sisges");
                builder.Property(scope => scope.EsActivo).HasColumnName("es_activo");
                builder.Property(scope => scope.CreadoPor).HasColumnName("creado_por");
                builder.Property(scope => scope.FechaCreacion).HasColumnName("fecha_creacion");
                builder.Property(scope => scope.ActualizadoPor).HasColumnName("actualizado_por");
                builder.Property(scope => scope.FechaActualizacion).HasColumnName("fecha_actualizacion");
            });

            modelBuilder.Entity<AnaliticaIncrustacionReporteClienteEntrada>(builder =>
            {
                builder.ToTable("incrustacion_cliente_reporte_power_bi", "acceso_analitica");
                builder.HasKey(embed => new
                {
                    embed.IdOpcion,
                    embed.IdClienteCrm,
                    embed.ClienteReporte
                });
                builder.Property(embed => embed.IdOpcion).HasColumnName("id_opcion");
                builder.Property(embed => embed.IdClienteCrm).HasColumnName("id_cliente_crm");
                builder.Property(embed => embed.ClienteReporte)
                    .HasColumnName("valor_cliente_reporte")
                    .HasMaxLength(150)
                    .IsUnicode(false);
                builder.Property(embed => embed.UrlIncrustacion)
                    .HasColumnName("url_incrustacion")
                    .HasMaxLength(2048)
                    .IsUnicode(false);
                builder.Property(embed => embed.EsActivo).HasColumnName("es_activo");
                builder.Property(embed => embed.CreadoPor).HasColumnName("creado_por");
                builder.Property(embed => embed.FechaCreacion).HasColumnName("fecha_creacion");
                builder.Property(embed => embed.ActualizadoPor).HasColumnName("actualizado_por");
                builder.Property(embed => embed.FechaActualizacion).HasColumnName("fecha_actualizacion");
            });

            modelBuilder.Entity<AnaliticaCatalogoReporteClienteEntrada>(builder =>
            {
                builder.HasNoKey();
                builder.ToTable("catalogo_cliente_reporte_power_bi", "acceso_analitica");
                builder.Property(catalog => catalog.IdOpcion).HasColumnName("id_opcion");
                builder.Property(catalog => catalog.IdClienteCrm).HasColumnName("id_cliente_crm");
                builder.Property(catalog => catalog.ClienteReporte)
                    .HasColumnName("valor_cliente_reporte")
                    .HasMaxLength(150)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SesionPowerBiAnalitica>(builder =>
            {
                builder.ToTable("sesion_power_bi", "telemetria_analitica");
                builder.HasKey(row => row.IdSesion);
                builder.Property(row => row.IdSesion)
                    .HasColumnName("id_sesion")
                    .ValueGeneratedNever();
                builder.Property(row => row.IdUsuario).HasColumnName("nId_Usuario");
                builder.Property(row => row.IdOpcionReporte).HasColumnName("nId_OpcionReporte");
                builder.Property(row => row.IdCliente).HasColumnName("nId_Cliente");
                builder.Property(row => row.UsuarioLogin)
                    .HasColumnName("usuario_login")
                    .HasMaxLength(100);
                builder.Property(row => row.UsuarioNombre)
                    .HasColumnName("usuario_nombre")
                    .HasMaxLength(250);
                builder.Property(row => row.ReporteNombre)
                    .HasColumnName("reporte_nombre")
                    .HasMaxLength(250);
                builder.Property(row => row.ClienteNombre)
                    .HasColumnName("cliente_nombre")
                    .HasMaxLength(250);
                builder.Property(row => row.FechaInicioUtc).HasColumnName("fecha_inicio_utc");
                builder.Property(row => row.FechaUltimoHeartbeatUtc).HasColumnName("fecha_ultimo_heartbeat_utc");
                builder.Property(row => row.FechaFinUtc).HasColumnName("fecha_fin_utc");
                builder.Property(row => row.SegundosVisibles).HasColumnName("segundos_visibles");
                builder.Property(row => row.EstaVisible).HasColumnName("esta_visible");
                builder.Property(row => row.Estado)
                    .HasColumnName("estado")
                    .HasMaxLength(12)
                    .IsUnicode(false);
                builder.Property(row => row.MotivoCierre)
                    .HasColumnName("motivo_cierre")
                    .HasMaxLength(30)
                    .IsUnicode(false);
                builder.Property(row => row.FechaCreacionUtc).HasColumnName("fecha_creacion_utc");
                builder.Property(row => row.FechaActualizacionUtc).HasColumnName("fecha_actualizacion_utc");
            });

            modelBuilder.Entity<EventoSesionPowerBiAnalitica>(builder =>
            {
                builder.ToTable("sesion_power_bi_evento", "telemetria_analitica");
                builder.HasKey(row => row.IdEvento);
                builder.Property(row => row.IdEvento)
                    .HasColumnName("id_evento")
                    .ValueGeneratedOnAdd();
                builder.Property(row => row.IdSesion).HasColumnName("id_sesion");
                builder.Property(row => row.TipoEvento)
                    .HasColumnName("tipo_evento")
                    .HasMaxLength(10)
                    .IsUnicode(false);
                builder.Property(row => row.FechaEventoUtc).HasColumnName("fecha_evento_utc");
                builder.Property(row => row.SegundosVisibles).HasColumnName("segundos_visibles");
                builder.Property(row => row.Origen)
                    .HasColumnName("origen")
                    .HasMaxLength(10)
                    .IsUnicode(false);
                builder.Property(row => row.Detalle)
                    .HasColumnName("detalle")
                    .HasMaxLength(500);
            });
        }
    }
}
