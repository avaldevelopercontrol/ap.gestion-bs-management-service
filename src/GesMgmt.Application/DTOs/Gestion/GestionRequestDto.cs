
using GesMgmt.Domain.Entities;

namespace GesMgmt.Application.DTOs.Gestion
{
    public class GestionRequestDto
    {
        public class GetGestionToDayRequestDto
        {
            public int nId_Cliente { get; set; } //ID_CLIENTE
            public int nId_Usuario { get; set; } //ID_USUARIO
        }

        public class GetGestionEstadoCuentaRequestDto
        {
            public int nId_Cliente { get; set; } //ID_CLIENTE
            public int nId_Cartera { get; set; } //ID_CARTERA
            public int nId_Persdeudor { get; set; } //ID_DEUDOR

            // 🔹 PAGINACIÓN
            public int PageNumber { get; set; } = 1;
            private int _pageSize = 10;

            public int PageSize
            {
                get => _pageSize;
                set => _pageSize = value > 1000 ? 1000 : value; // Máximo 50
            }
        }

        public class GetGestionMotivoNoPagoRequestDto
        {
            public int nId_Cliente { get; set; }
            public int nId_Cartera { get; set; }
        }

        public class GetGestionEstadoGestionClaroRequestDto
        {
            public int nId_Cliente { get; set; }
            public int nId_Cartera { get; set; }
            //public string GOB_EMP { get; set; }
        }

        public class GetGestionPaletaRespuestaRequestDto
        {
            public int nId_Cliente { get; set; }
            public int nId_Contrato { get; set; }
            public int nNivelPaleta { get; set; }
            public int? nId_SupOpeCodCliOut { get; set; }
            public int nId_TipoGestion { get; set; }
        }

        public class GetGestionEstadoGestionRequestDto
        {
            public int nId_Cliente { get; set; }
        }

        public class CreateGestionOpeRequestDto
        {
            public int? nId_DocxCobrarOpe { get; set; }
            public int? nId_Cliente { get; set; }
            public int? nId_Contrato { get; set; }
            public int? nId_Cartera { get; set; }
            public int nId_DocxCobrar { get; set; }
            public int nId_PersDeudor { get; set; }
            public int? nId_Usuario { get; set; }
            public string? cNOMBRECONTACTO { get; set; }
            public string? cCARGO { get; set; }
            public int? nNP0 { get; set; }
            public int? nNP1 { get; set; }
            public int? nNP2 { get; set; }
            public int? nESTADOGESTION { get; set; }
            public string cTELEFONO { get; set; }
            public int? nTIPOGESTION { get; set; }
            public int? nASIGNARGESTOR { get; set; }
            public DateTime? dFECHACOMPROMISO { get; set; }
            public decimal? nMONTOSOLES { get; set; }
            public decimal? nMONTODOLARES { get; set; }
            public string? dFECHANUEVAGESTION { get; set; }
            public string? cHORANUEVAGESTION { get; set; }
            public string? cMINUTONUEVAGESTION { get; set; }
            public string? dFECHAGESTION { get; set; }
            public string? cHORAGESTION { get; set; }
            public string? cMINUTOGESTION { get; set; }
            public string? cOBSERVACION { get; set; }
            public string? cSISTEMA { get; set; }
            public int? nESTADOGESTIONCLARO { get; set; }
            public int? nMOTIVONOPAGO { get; set; }
            public DateTime? dFechaInicioGestion { get; set; }
            public DateTime? dFechaFinGestion { get; set; }
            public bool? bEstado { get; set; }
        }

        public class CreateGestionOpeGesRequestDto
        {
            public int? nId_DocxCobrarOpe { get; set; }
            public int? nId_Cliente { get; set; }
            public int? nId_Contrato { get; set; }
            public int? nId_Cartera { get; set; }
            //public int nId_DocxCobrar { get; set; }
            public string nId_DocxCobrars { get; set; }
            public int nId_PersDeudor { get; set; }
            public int? nId_Usuario { get; set; }
            public string? cNOMBRECONTACTO { get; set; }
            public string? cCARGO { get; set; }
            public int? nNP0 { get; set; }
            public int? nNP1 { get; set; }
            public int? nNP2 { get; set; }
            public int? nESTADOGESTION {  get; set; } 
            public string cTELEFONO { get; set; }
            public int? nTIPOGESTION { get; set; }
            public int? nASIGNARGESTOR { get; set; }
            public DateTime? dFECHACOMPROMISO { get; set; }
            public decimal? nMONTOSOLES { get; set; }
            public decimal? nMONTODOLARES { get; set; }
            public string? dFECHANUEVAGESTION { get; set; }
            public string? cHORANUEVAGESTION { get; set; }
            public string? cMINUTONUEVAGESTION { get; set; }
            public string? dFECHAGESTION { get; set; }
            public string? cHORAGESTION { get; set; }
            public string? cMINUTOGESTION { get; set; }
            public string? cOBSERVACION { get; set; }
            public string? cSISTEMA { get; set; }
            public int? nESTADOGESTIONCLARO { get; set; }
            public int? nMOTIVONOPAGO { get; set; }
            public DateTime? dFechaInicioGestion { get; set; }
            public DateTime? dFechaFinGestion { get; set; }
            public bool? bEstado { get; set; }
        }

        public class GetGestionInformacionDeudorParamRequestDto
        {
            public int nId_Persdeudor { get; set; }
        }

        public class GetGestionInformacionDeudorRequestDto
        {
            public bool? bTipo_Cabecera { get; set; }
        }

        public class GetGestionPagosRequestDto
        {
            public int nId_Cliente { get; set; }
            public int nId_Cartera { get; set; }
            public int nId_Persdeudor { get; set; } //ID_DEUDOR
            // 🔹 PAGINACIÓN
            public int PageNumber { get; set; } = 1;
            private int _pageSize = 10;
            public int PageSize
            {
                get => _pageSize;
                set => _pageSize = value > 1000 ? 1000 : value; // Máximo 1000
            }
        }

        public class GetGestionAgendaRequestDto
        {
            public int nId_Cliente { get; set; }
            public int nId_Cartera { get; set; }
            public int nId_Persdeudor { get; set; } //ID_DEUDOR
            public int nId_PerfilUsuario { get; set; } //ID_PERFIL_USUARIO
            // 🔹 PAGINACIÓN
            public int PageNumber { get; set; } = 1;
            private int _pageSize = 10;
            public int PageSize
            {
                get => _pageSize;
                set => _pageSize = value > 1000 ? 1000 : value; // Máximo 1000
            }
        }

        public class GetGestionZonaCarteraCampannaRequestDto
        {
            public int nId_Cliente { get; set; }
            public int nId_Cartera { get; set; }
        }

        

        public class GetGestionGestionesCarteraDeudorRequestDto
        {
            public int nId_Cliente { get; set; } //ID_CLIENTE
            public int nId_Cartera { get; set; } //ID_CARTERA
            public int nId_Persdeudor { get; set; } //ID_DEUDOR
            public int nId_PerfilUsuario { get; set; } //ID_PERFIL_USUARIO

            // 🔹 PAGINACIÓN
            public int PageNumber { get; set; } = 1;
            private int _pageSize = 10;

            public int PageSize
            {
                get => _pageSize;
                set => _pageSize = value > 1000 ? 1000 : value; // Máximo 50
            }
        }

        public class GetGestionEstadoGestionCarteraDeudorRequestDto
        {
            public int nId_Cliente { get; set; } //ID_CLIENTE
            public int nId_Cartera { get; set; } //ID_CARTERA
            public int nId_Persdeudor { get; set; } //ID_DEUDOR

            // 🔹 PAGINACIÓN
            public int PageNumber { get; set; } = 1;
            private int _pageSize = 10;

            public int PageSize
            {
                get => _pageSize;
                set => _pageSize = value > 1000 ? 1000 : value; // Máximo 50
            }
        }

        public class GetGestionDocumentoRequestDto
        {
            public int nId_Cliente { get; set; } //ID_CLIENTE
            public int nId_Cartera { get; set; } //ID_CARTERA
            public int nId_Persdeudor { get; set; } //ID_DEUDOR

            // 🔹 PAGINACIÓN
            public int PageNumber { get; set; } = 1;
            private int _pageSize = 10;

            public int PageSize
            {
                get => _pageSize;
                set => _pageSize = value > 1000 ? 1000 : value; // Máximo 50
            }
        }

        public class GetGestionDeudorRequestDto
        {
            public int nId_Cliente { get; set; } //ID_CLIENTE
            public int nId_Cartera { get; set; } //ID_CARTERA
            public int nId_Persdeudor { get; set; } //ID_DEUDOR
        }

        public class GetGestionCabeceraRequestDto
        {
            public int nId_Cliente { get; set; }
            public int nId_Contrato { get; set; }
        }

        public class GetGestionCabeceraAdicionalRequestDto
        {
            public int nId_Cliente { get; set; }
            public int pantalla { get; set; }
        }

        public class GetGestionAdicionalRequestDto()
        {
            public int nId_Cliente { get; set; } //ID_CLIENTE
            public int nId_Cartera { get; set; } //ID_CARTERA
            public int nId_Persdeudor { get; set; } //ID_DEUDOR

            // 🔹 PAGINACIÓN
            public int PageNumber { get; set; } = 1;
            private int _pageSize = 10;

            public int PageSize
            {
                get => _pageSize;
                set => _pageSize = value > 1000 ? 1000 : value; // Máximo 50
            }
        }

        public class GestionCarteraDeudorHistoricaRequestDto()
        {
            public int nId_Cliente { get; set; }
            public int nId_Cartera { get; set; }
            public int nId_PersDeudor { get; set; }

            // 🔹 PAGINACIÓN
            public int PageNumber { get; set; } = 1;
            private int _pageSize = 10;

            public int PageSize
            {
                get => _pageSize;
                set => _pageSize = value > 1000 ? 1000 : value; // Máximo 50
            }
        }

        public class GestionCarteraDeudorEstadoHistoricaRequestDto()
        {
            public int nId_Cliente { get; set; }
            public int nId_Cartera { get; set; }
            public int nId_PersDeudor { get; set; }

            // 🔹 PAGINACIÓN
            public int PageNumber { get; set; } = 1;
            private int _pageSize = 10;

            public int PageSize
            {
                get => _pageSize;
                set => _pageSize = value > 1000 ? 1000 : value; // Máximo 50
            }
        }
    }
}