
namespace GesMgmt.Application.DTOs.Gestion
{
    public class GetGestionRequestDto
    {
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