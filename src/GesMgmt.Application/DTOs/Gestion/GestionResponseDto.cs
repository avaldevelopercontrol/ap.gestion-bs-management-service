using GesMgmt.Application.DTOs.Gestion.Converters;
using System.Text.Json.Serialization;

namespace GesMgmt.Application.DTOs.Gestion
{
    public class GestionResponseDto
    {
        public class GetGestionToDayResponseDto
        {
            public string? Hora { get; set; }
            public int Total { get; set; }
            public int Ges4 { get; set; }
            public int Ges15 { get; set; }
            public int Ges13 { get; set; }
            public int Ges4b { get; set; }
            public int Ges0 { get; set; }
        }

        public class GetGestionMotivoNoPagoResponseDto
        {
            public int nId_MotivoNoPago { get; set; }
            public string cNombreMotivoNoPago { get; set; }
        }

        public class GetGestionEstadoGestionClaroResponseDto
        {
            public int nId_OpeCodCliOut { get; set; }
            public string cNombre_OpeCodCliOut { get; set; }
        }

        public class GetGestionPaletaRespuestaResponseDto
        {
            public int nId_OpeCodCliOut { get; set; }
            public string cNombre_OpeCodCliOut { get; set; }
        }

        public class GetGestionEstadoGestionResponseDto
        {
            public int nId_OpeCodCliOut { get; set; }
            public string cNombre_OpeCodCliOut { get; set; }
        }

        public class GetGestionTipoGestionResponseDto
        {
            public int nId_TipoGestion { get; set; }
            public string cNomTipoGestion{ get; set; }
        }

        public class CreateGestionOpeResponseDto
        {
            public int? nId_DocxCobrarOpe { get; set; }
            public int? nId_Cliente { get; set; }
            public int? nId_Contrato { get; set; }
            public int? nId_Cartera { get; set; }
            public int nId_DocxCobrar { get; set; }
            public int nId_PersDeudor { get; set; }
            public int? nId_Usuario { get; set; }
        }

        public class CreateGestionOpeGesResponseDto
        {
            public int? nro { get; set; }
            public int? nId_DocxCobrarOpeGes { get; set; }
            public int? nId_DocxCobrarOpe { get; set; }
            public int? nId_Cliente { get; set; }
            public int? nId_Contrato { get; set; }
            public int? nId_Cartera { get; set; }
            public int nId_DocxCobrar { get; set; }
            public int nId_PersDeudor { get; set; }
            public int? nId_Usuario { get; set; }
        }

        public class GetGestionZonaCarteraCampannaResponseDto
        {
            public string Zona { get; set; }
            public string Ciudad { get; set; }
            public string cCar_Nombre { get; set; }
            public string cCampanna { get; set; }
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

        [JsonConverter(typeof(GetGestionDocumentoResponseDtoJsonConverter))]
        public class GetGestionDocumentoResponseDto
        {
            // ============================================================
            // CAMPOS COMUNES - TODOS LOS CLIENTES
            // ============================================================

            // =========================================================
            // PROPIEDAD AUXILIAR PARA EL CONVERTER
            // NO SE MOSTRARÁ EN EL JSON
            // =========================================================
            [JsonIgnore]
            public int nId_ClienteJson { get; set; }


            // =========================================================
            // CAMPOS CABECERA - COMUNES
            // =========================================================

            public int nId_DocxCobrar { get; set; }
            public int? mejorStatus { get; set; }
            public int nId_Moneda { get; set; }
            public int? bEstado { get; set; }
            public string? nZona { get; set; }
            public bool bSelected { get; set; }
            public int? nId_Estrategia { get; set; }
            public int nId_Cartera { get; set; }

            // =========================================================
            // CAMPOS ADICIONALES / COMUNES DE GESTIÓN
            // =========================================================

            public string? tramo { get; set; }
            public int nro { get; set; }
            public string? numeroDocumento { get; set; }
            public string? estado { get; set; }
            public string? fechaVencimiento { get; set; }
            public string? siglaMoneda { get; set; }
            public decimal? importeTotal { get; set; }
            public decimal? importeSaldo { get; set; }
            public int diasAtrazo { get; set; }
            public string? gestorCall { get; set; }

            // =========================================================
            // CLIENTE 95 - CLARO
            // =========================================================

            public string? servicio { get; set; }
            public string? comentario { get; set; }
            public string? codigoCliente { get; set; }
            public string? estadoDocumento { get; set; }

            public string? fechaEstadoDocumento { get; set; }

            public string? estadoPago { get; set; }

            public string? statusDocumento { get; set; }

            public string? fechaStatusDocumento { get; set; }

            public string? bajaProvabilidad { get; set; }


            // =========================================================
            // CLIENTE 59 - MAF
            // =========================================================

            public string? numeroCuota { get; set; }

            // IMPORTANTE: nullable
            public decimal? deudaVencida { get; set; }

            public string? tipoCredito { get; set; }

            public string? COD_ACC_PREV { get; set; }

            public string? COD_ACC_PREJU { get; set; }

            public string? ultimoTramo { get; set; }

            public string? ultimoFechaPago { get; set; }

            public string? categoria { get; set; }

            public string? numeroReprogramaciones { get; set; }

            public string? gWhatsApp { get; set; }

            public string? cuotaActual { get; set; }

            public string? interesActual { get; set; }

            public string? placa { get; set; }

            public string? numeroCuenta { get; set; }

            public string? MARCA_ESPECIAL { get; set; }

            public string? plazoReprogramado { get; set; }

            public string? plazoMaximoReprogramado { get; set; }

            public string? MARCA_ESPECIAL2 { get; set; }

            public string? COMENTARIO_REPROG { get; set; }

            public string? TASA_INTERES { get; set; }

            public string? CAPITAL_ACTUAL { get; set; }
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