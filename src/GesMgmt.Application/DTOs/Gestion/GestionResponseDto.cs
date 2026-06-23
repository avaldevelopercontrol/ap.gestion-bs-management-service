
namespace GesMgmt.Application.DTOs.Gestion
{
    public class GestionResponseDto
    {
        public class GetGestionAgendaResponseDto
        {
            public int nid_agenda { get; set; }
            public DateTime? fechaNuevaGestion { get; set; }
            public string tiempoVencido { get; set; }
            public string? cartera { get; set; }
            public string? deudor { get; set; }
            public string? respuestaOEstado { get; set; }
            public string? usuario { get; set; }
        }

        public class GetGestionZonaCarteraCampannaResponseDto
        {
            public string Zona { get; set; }
            public string Ciudad { get; set; }
            public string cCar_Nombre { get; set; }
            public string cCampanna { get; set; }
        }

        public class GetGestionTelefonoResponseDto
        {
            public int nId_PersTelef { get; set; }
            public int? prioridad { get; set; }
            public string? nroTelefono { get; set; }
            public string? horario { get; set; }
            public string? referenciaUbicacion { get; set; }
            public string? estado { get; set; }
            public string? fechaEstado { get; set; }
            public string? fechaBase { get; set; }
            public string? contactados { get; set; }
            public int? noContactados { get; set; }
            public int? cantidadIvr { get; set; }
            public string? fuente { get; set; }
            public string? ordenSearch { get; set; }
        }

        public class GetGestionGestionesCarteraDeudorResponseDto
        {
            public int nId_DocxCobrarOpe { get; set; }
            public int nro { get; set; }
            public string? fechaGestion { get; set; }
            public string? gestor { get; set; }
            public string? documento { get; set; }
            public string? operacion { get; set; }
            public string? respuesta { get; set; }
            public string? comentario { get; set; }
        }

        public class GetGestionEstadoGestionCarteraDeudorResponseDto
        {
            public int nId_DocxCobrarOpe { get; set; }
            public int nro { get; set; }
            public string? fechaGestion { get; set; }
            public string? operador { get; set; }
            public string? documento { get; set; }
            public string? operacion { get; set; }
            public string? resultado { get; set; }
            public string? comentario { get; set; }
        }

        public class GetGestionDocumentoResponseDto
        {
            public int nId_DocxCobrar { get; set; } //1 Cabecera principal
            public int? mejorStatus { get; set; } //2 Cabecera principal
            public int nId_Moneda { get; set; } //2 Cabecera principal
            public int? bEstado { get; set; } //3 Cabecera principal
            public string? nZona { get; set; } //3 Cabecera principal
            public bool bSelected { get; set; } //4 Cabecera principal
            public int? nId_Estrategia { get; set; } //5 Cabecera principal
            public int nId_Cartera { get; set; } //6 Cabecera principal

            //-- Campos adicionales para la gestión
            //-- 95 CLARO
            public int nro { get; set; }
            public string? numeroDocumento { get; set; }
            public string? estado { get; set; }
            public string? fechaVencimiento { get; set; }
            public string? siglaMoneda { get; set; }
            public decimal? importeTotal { get; set; }
            public decimal? importeSaldo { get; set; }
            //public decimal deudaVencida { get; set; }
            public int diasAtrazo { get; set; }
            public string? servicio { get; set; }
            public string? comentario { get; set; }
            public string? codigoCliente { get; set; }
            public string? estadoDocumento { get; set; }
            public string? fechaEstadoDocumento { get; set; }
            public string? estadoPago { get; set; }
            public string? statusDocumento { get; set; }
            public string? fechaStatusDocumento { get; set; }
            public string? gestorCall { get; set; }
            public string? bajaProvabilidad { get; set; }
        }

        public class GetGestionDireccionResponseDto
        {
            public int nId_PersDirecc { get; set; }
            public string? direccion { get; set; }
            public string? referenciaUbicacion { get; set; }
            public string? tipoDeudor { get; set; }
            public string? nombre { get; set; }
            public string? estado { get; set; }
        }

        public class GetGestionDeudorResponseDto
        {
            public int nId_PersDeudor { get; set; }
            public string? dni { get; set; }
            public string? ruc { get; set; }
            public string? nombre { get; set; }
            public string? nombreCompleto { get; set; }
            public string? gradoInstruccion { get; set; }
            public string? edad { get; set; }
            public string? correo { get; set; }
            public bool? informacionAdicional { get; set; }
            public bool? pagos { get; set; }
            public bool? agendas { get; set; }
            public bool? llamadas { get; set; }
            public DateTime fechaConsulta { get; set; }
            public string? codigo { get; set; }
            public string? asesorPostVenta { get; set; }
            public string? correoAsesorPostVenta { get; set; }
            public string? asesorComercial { get; set; }
            public string? correoAsesorComercial { get; set; }
            public bool? validaCronograma { get; set; }
            public string? clientePorVision { get; set; }
            public string? clienteListaBlanca { get; set; }
            public string? clienteConSinPe { get; set; }
            public string? nGra_Instruccion { get; set; }
        }

        public class GetGestionCabeceraResponseDto
        {
            public int idCabeceraPantalla { get; set; }
            public string tituloCabeceraPantalla { get; set; }
            public string tipoDato { get; set; }
            public bool? operaTotal { get; set; }
            public bool? compromiso { get; set; }
            public int orden { get; set; }
            public int pantalla { get; set; }
            public string? alineacionHtml { get; set; }
            public int? nId_Contrato { get; set; }
            public int? nId_Cliente { get; set; }
        }

        public class GetGestionCabeceraAdicionalResponseDto
        {
            public int idCab { get; set; } //1 Cabecera principal
            public int? nId_Cliente { get; set; } //1 Cabecera principal
            public int? pantalla { get; set; } //1 Cabecera principal
            public string? recibo { get; set; }
            public string? telefono { get; set; }
            public string? servicio { get; set; }
            public string? estadoServicio { get; set; }
            public string? motivo { get; set; }
            public string? codigoCliente { get; set; }
        }

        public class GetGestionAdicionalResponseDto
        {
            public int nId_DocxCobrarAd { get; set; } //1 Cabecera principal
            public int? nId_DocxCobrar { get; set; } //2 Cabecera principal
            public int? nId_PersDeudor { get; set; } //2 Cabecera principal
            public int? nId_Cartera { get; set; } //3 Cabecera principal
            public int? nId_Cliente { get; set; } //5 Cabecera principal
                                                  //-- Campos adicionales para la gestión
                                                  //-- 95 CLARO
            public string? recibo { get; set; }
            public string? telefono { get; set; }
            public string? servicio { get; set; }
            public string? estadoServicio { get; set; }
            public string? motivo { get; set; }
            public string? codigoCliente { get; set; }
        }

        public class GestionCarteraDeudorHistoricaResponseDto()
        {
            public int nId_DocxCobrarOpe { get; set; }
            public int nro { get; set; }
            public string? cliente { get; set; }
            public string? cartera { get; set; }
            public string? campanna { get; set; }
            public string? fecha { get; set; }
            public string? gestor { get; set; }
            public string? documento { get; set; }
            public string? operacion { get; set; }
            public string? resultado { get; set; }
            public string? comentario { get; set; }
        }

        public class GestionCarteraDeudorEstadoHistoricaResponseDto
        {
            public int nId_DocxCobrarOpe { get; set; }
            public int nro { get; set; }
            public string? cliente { get; set; }
            public string? cartera { get; set; }
            public string? campanna { get; set; }
            public string? fecha { get; set; }
            public string? gestor { get; set; }
            public string? documento { get; set; }
            public string? operacion { get; set; }
            public string? resultado { get; set; }
            public string? comentario { get; set; }
        }
    }
}