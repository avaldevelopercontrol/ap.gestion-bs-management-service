using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Boton
{
    public class BotonResponseDto
    {
        #region "BOTONES MAF"
        public class GetGestionBotonesResponseDto
        {
            public int nId_Boton { get; set; }
            public int nId_Cliente { get; set; }
            public int nId_Contrato { get; set; }
            public string nombreBoton { get; set; }
            public string descripcionBoton { get; set; }
            public bool bEstado { get; set; }
            public int nCrea { get; set; }
            public DateTime dFechaCrea { get; set; }
            public int? nModifica { get; set; }
            public DateTime? dFechaModifica { get; set; }
        }

        public class GetReportarCasosResponseDto
        {
            public int Id { get; set; }
            public string Caso { get; set; }
            public string Descripcion { get; set; }
            public string Cartera { get; set; }
            public string Usuario { get; set; }
            public string? Fec_Ingreso { get; set; }
        }

        public class GetReportarCasosByIdResponseDto
        {
            public int nId_DocxCobrarOpeResult { get; set; }
            public int nId_DocxCobrar { get; set; }
            public DateTime? dDocCobOpe_FecIni { get; set; }
            public string? cDocOpeCobOut_Descr { get; set; }
            public int? nId_UsuOpe { get; set; }
            public int nId_PersDeudor { get; set; }
            public int nId_Cartera { get; set; }
            public int nId_Cliente { get; set; }
            public DateTime? dDoc_FecActual { get; set; }
            public string? cDocParam01 { get; set; }
            public string? cDocParam04 { get; set; }
        }

        public class CreateReportarCasosResponseDto
        {
            public int nId_DocxCobrarOpeResult { get; set; }
            public int nId_Cliente { get; set; }
            public int nId_Cartera { get; set; }
            public int nId_DocxCobrar { get; set; }
        }

        public class EditReportarCasosResponseDto
        {
            public int nId_DocxCobrarOpeResult { get; set; }
            public int nId_Cliente { get; set; }
            public int nId_Cartera { get; set; }
            public int nId_DocxCobrar { get; set; }
        }

        public class GetOperativasMafResponseDto
        {
            public int? numeroDiasNoContacto { get; set; }

            public DateTime? fechaUltimoContacto { get; set; }

            public int cantidadTotalVino { get; set; }

            public int cantidadTotalPago { get; set; }

            public int? cantidadTotalVino6Meses { get; set; }

            public int? cantidadTotalPago6Meses { get; set; }

            public string? cobertura { get; set; }

            public List<MejorGestionResponseDto> mejoresGestiones { get; set; }
                = new();

            public List<OperacionMafResponseDto> operaciones { get; set; }
                = new();
        }

        public class MejorGestionResponseDto
        {
            public int ventanaMeses { get; set; }

            // 1 = CALL
            // 2 = CAMPO
            public int canal { get; set; }

            public string canalNombre { get; set; } = string.Empty;

            public int? nId_DocxCobrarOpe { get; set; }

            public int? nId_DocxCobrar { get; set; }

            public DateTime? fecha { get; set; }

            public string? estatus { get; set; }

            public int? peso { get; set; }

            public string? telefono { get; set; }

            public string? comentario { get; set; }

            public int intentos { get; set; }

            public int intentosRobot { get; set; }

            public int contactosDirectos { get; set; }

            // Para CAMPO
            public string? origenDireccion { get; set; }

            public string? direccion { get; set; }
        }

        public class OperacionMafResponseDto
        {
            public string operacion { get; set; } = string.Empty;

            public string? placa { get; set; }

            public int? diasAtraso { get; set; }

            public int? nId_Ubigeo { get; set; }

            public string? estadoOperacion { get; set; }

            public string? avanceCredito { get; set; }

            public string? direccionLegal { get; set; }

            public string? distritoLegal { get; set; }

            public string? provinciaLegal { get; set; }

            public string? departamentoLegal { get; set; }
        }

        #endregion

        #region "BOTONES CLARO"
        public class GetEstadoCuentaResponseDto
        {
            public int nId_DocxCobrar { get; set; }
            public string? RUC { get; set; }
            public string? CODIGO { get; set; }
            public string? NRO_CUENTA { get; set; }
            public string? TIPO_DOCUMENTO { get; set; }
            public string? NUMERO_RECIBO { get; set; }
            public string? FECHA_EMISION { get; set; }
            public string? FECHA_VENCIMIENTO { get; set; }
            public string? MONEDA { get; set; }
            public string? MONTO_FACTURADO { get; set; }
            public string? IMPORTE_PENDIENTE { get; set; }
            public string? TIPO_SERVICIO { get; set; }
            public string? ESTADO { get; set; }
            public string? RAZON_SOCIAL { get; set; }
            public string? TIPO_IDENTIFICACION { get; set; }
            public string? MONTO_EN_DISPUTA { get; set; }
            public string? TRAMO { get; set; }
            public string? NRO_CONTRATO { get; set; }
            public string? NRO_PROCESO { get; set; }
        }

        public class GetPagosResponsetDto
        {
            public int nro { get; set; }
            public string? codigoCliente { get; set; }
            public string? nroDocumento { get; set; }
            public string? fechaPago { get; set; }
            public decimal? montoPago { get; set; }
            public string? moneda { get; set; }
            public string? zona { get; set; }
            public string? notaCredito { get; set; }
            public string? marca { get; set; }
        }

        public class GetAgendaResponseDto
        {
            public int nid_agenda { get; set; }
            public DateTime? fechaNuevaGestion { get; set; }
            public string tiempoVencido { get; set; }
            public string? cartera { get; set; }
            public string? deudor { get; set; }
            public string? respuestaOEstado { get; set; }
            public string? usuario { get; set; }
        }


        public class GetInformacionDeudorRespondeDto
        {
            public string? cNombre_Param01 { get; set; }
            public string? cNombre_Param02 { get; set; }
            public string? cNombre_Param03 { get; set; }
            public string? cNombre_Param04 { get; set; }
            public string? cNombre_Param05 { get; set; }
            public string? cNombre_Param06 { get; set; }
            public string? cNombre_Param07 { get; set; }
            public string? cNombre_Param08 { get; set; }
            public string? cNombre_Param09 { get; set; }
            public string? cNombre_Param10 { get; set; }
            public string? cNombre_Param11 { get; set; }
            public string? cNombre_Param12 { get; set; }
            public string? cNombre_Param13 { get; set; }
            public string? cNombre_Param14 { get; set; }
            public string? cNombre_Param15 { get; set; }
            public string? cNombre_Param16 { get; set; }
            public string? cNombre_Param17 { get; set; }
            public string? cNombre_Param18 { get; set; }
            public string? cNombre_Param19 { get; set; }
            public string? cNombre_Param20 { get; set; }
            public string? cNombre_Param21 { get; set; }
            public string? cNombre_Param22 { get; set; }
            public string? cNombre_Param23 { get; set; }
            public string? cNombre_Param24 { get; set; }
            public string? cNombre_Param25 { get; set; }
            public string? cNombre_Param26 { get; set; }
            public string? cNombre_Param27 { get; set; }
            public string? cNombre_Param28 { get; set; }
            public string? cNombre_Param29 { get; set; }
            public string? cNombre_Param30 { get; set; }
            public string? cNombre_Param31 { get; set; }
            public string? cNombre_Param32 { get; set; }
            public string? cNombre_Param33 { get; set; }
            public string? cNombre_Param34 { get; set; }
            public string? cNombre_Param35 { get; set; }
            public string? cNombre_Param36 { get; set; }
            public string? cNombre_Param37 { get; set; }
            public string? cNombre_Param38 { get; set; }
            public string? cNombre_Param39 { get; set; }
            public string? cNombre_Param40 { get; set; }
            public string? cNombre_Param41 { get; set; }
            public string? cNombre_Param42 { get; set; }
            public string? cNombre_Param43 { get; set; }
            public string? cNombre_Param44 { get; set; }
            public string? cNombre_Param45 { get; set; }
            public string? cNombre_Param46 { get; set; }
            public string? cNombre_Param47 { get; set; }
            public string? cNombre_Param48 { get; set; }
            public string? cNombre_Param49 { get; set; }
            public string? cNombre_Param50 { get; set; }
            public string? cNombre_Param51 { get; set; }
            public string? cNombre_Param52 { get; set; }
            public string? cNombre_Param53 { get; set; }
            public string? cNombre_Param54 { get; set; }
            public string? cNombre_Param55 { get; set; }
            public string? cNombre_Param56 { get; set; }
            public string? cNombre_Param57 { get; set; }
            public string? cNombre_Param58 { get; set; }
            public string? cNombre_Param59 { get; set; }
            public string? cNombre_Param60 { get; set; }
            public string? cNombre_Param61 { get; set; }
            public string? cNombre_Param62 { get; set; }
            public string? cNombre_Param63 { get; set; }
            public string? cNombre_Param64 { get; set; }
            public string? cNombre_Param65 { get; set; }
            public string? cNombre_Param66 { get; set; }
            public string? cNombre_Param67 { get; set; }
            public string? cNombre_Param68 { get; set; }
            public string? cNombre_Param69 { get; set; }
            public string? cNombre_Param70 { get; set; }
            public string? cNombre_Param71 { get; set; }
            public string? cNombre_Param72 { get; set; }
            public string? cNombre_Param73 { get; set; }
            public string? cNombre_Param74 { get; set; }
            public string? cNombre_Param75 { get; set; }
            public string? cNombre_Param76 { get; set; }
            public string? cNombre_Param77 { get; set; }
            public string? cNombre_Param78 { get; set; }
            public string? cNombre_Param79 { get; set; }
            public string? cNombre_Param80 { get; set; }
            public bool? bTipo_Cabecera { get; set; }
        }
        public class GetInformacionDeudorParamRespondeDto
        {
            public string? cPersInf_Param01 { get; set; }
            public string? cPersInf_Param02 { get; set; }
            public string? cPersInf_Param03 { get; set; }
            public string? cPersInf_Param04 { get; set; }
            public string? cPersInf_Param05 { get; set; }
            public string? cPersInf_Param06 { get; set; }
            public string? cPersInf_Param07 { get; set; }
            public string? cPersInf_Param08 { get; set; }
            public string? cPersInf_Param09 { get; set; }
            public string? cPersInf_Param10 { get; set; }
            public string? cPersInf_Param11 { get; set; }
            public string? cPersInf_Param12 { get; set; }
            public string? cPersInf_Param13 { get; set; }
            public string? cPersInf_Param14 { get; set; }
            public string? cPersInf_Param15 { get; set; }
            public string? cPersInf_Param16 { get; set; }
            public string? cPersInf_Param17 { get; set; }
            public string? cPersInf_Param18 { get; set; }
            public string? cPersInf_Param19 { get; set; }
            public string? cPersInf_Param20 { get; set; }
            public string? cPersInf_Param21 { get; set; }
            public string? cPersInf_Param22 { get; set; }
            public string? cPersInf_Param23 { get; set; }
            public string? cPersInf_Param24 { get; set; }
            public string? cPersInf_Param25 { get; set; }
            public string? cPersInf_Param26 { get; set; }
            public string? cPersInf_Param27 { get; set; }
            public string? cPersInf_Param28 { get; set; }
            public string? cPersInf_Param29 { get; set; }
            public string? cPersInf_Param30 { get; set; }
            public string? cPersInf_Param31 { get; set; }
            public string? cPersInf_Param32 { get; set; }
            public string? cPersInf_Param33 { get; set; }
            public string? cPersInf_Param34 { get; set; }
            public string? cPersInf_Param35 { get; set; }
            public string? cPersInf_Param36 { get; set; }
            public string? cPersInf_Param37 { get; set; }
            public string? cPersInf_Param38 { get; set; }
            public string? cPersInf_Param39 { get; set; }
            public string? cPersInf_Param40 { get; set; }
            public string? cPersInf_Param41 { get; set; }
            public string? cPersInf_Param42 { get; set; }
            public string? cPersInf_Param43 { get; set; }
            public string? cPersInf_Param44 { get; set; }
            public string? cPersInf_Param45 { get; set; }
            public string? cPersInf_Param46 { get; set; }
            public string? cPersInf_Param47 { get; set; }
            public string? cPersInf_Param48 { get; set; }
            public string? cPersInf_Param49 { get; set; }
            public string? cPersInf_Param50 { get; set; }
            public string? cPersInf_Param51 { get; set; }
            public string? cPersInf_Param52 { get; set; }
            public string? cPersInf_Param53 { get; set; }
            public string? cPersInf_Param54 { get; set; }
            public string? cPersInf_Param55 { get; set; }
            public string? cPersInf_Param56 { get; set; }
            public string? cPersInf_Param57 { get; set; }
            public string? cPersInf_Param58 { get; set; }
            public string? cPersInf_Param59 { get; set; }
            public string? cPersInf_Param60 { get; set; }
            public string? cPersInf_Param61 { get; set; }
            public string? cPersInf_Param62 { get; set; }
            public string? cPersInf_Param63 { get; set; }
            public string? cPersInf_Param64 { get; set; }
            public string? cPersInf_Param65 { get; set; }
            public string? cPersInf_Param66 { get; set; }
            public string? cPersInf_Param67 { get; set; }
            public string? cPersInf_Param68 { get; set; }
            public string? cPersInf_Param69 { get; set; }
            public string? cPersInf_Param70 { get; set; }
            public string? cPersInf_Param71 { get; set; }
            public string? cPersInf_Param72 { get; set; }
            public string? cPersInf_Param73 { get; set; }
            public string? cPersInf_Param74 { get; set; }
            public string? cPersInf_Param75 { get; set; }
            public string? cPersInf_Param76 { get; set; }
            public string? cPersInf_Param77 { get; set; }
            public string? cPersInf_Param78 { get; set; }
            public string? cPersInf_Param79 { get; set; }
            public string? cPersInf_Param80 { get; set; }
        }

        #endregion

    }
}