
namespace GesMgmt.Application.DTOs.Boton
{
    public class BotonRequestDto
    {
        #region "BOTONES CLARO"
        public class GetGestionBotonesRequestDto
        {
            public int nId_Cliente { get; set; } //ID_CLIENTE
            public int nId_Contrato { get; set; } //ID_CONTRATO
        }

        public class GetReportarCasosRequestDto
        {
            public int nId_Cliente { get; set; }
            public int nId_Cartera { get; set; }
            public int nId_PersDeudor { get; set; }
        }

        public class GetReportarCasosByIdRequestDto
        {
            public int nId_DocxCobrarOpeResult { get; set; }
            public int nId_Cartera { get; set; }
            public int nId_Cliente { get; set; }
            public int nId_PersDeudor { get; set; }
        }

        public class CreateReportarCasosRequestDto
        {
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

        public class EditReportarCasosRequestDto
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

        public class GetOperativasMafRequestDto
        {
            public int nId_PersDeudor { get; set; }
            public int nId_Cartera { get; set; }
            public int nId_Cliente { get; set; }
        }
        #endregion


        #region "CLARO"

        public class GetEstadoCuentaRequestDto
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

        public class GetPagosRequestDto
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

        public class GetAgendaRequestDto
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

        public class GetInformacionDeudorRequestDto
        {
            public bool? bTipo_Cabecera { get; set; }
        }

        public class GetInformacionDeudorParamRequestDto
        {
            public int nId_Persdeudor { get; set; }
        }
        #endregion
    }
}