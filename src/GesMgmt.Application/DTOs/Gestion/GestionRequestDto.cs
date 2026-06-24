
namespace GesMgmt.Application.DTOs.Gestion
{
    public class GestionRequestDto
    {
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

        public class GetGestionTelefonoRequestDto
        {

            public int nId_Cliente { get; set; } //ID_CLIENTE
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

        public class GetGestionDireccionRequestDto
        {
            public int nId_Cliente { get; set; } //ID_CLIENTE
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