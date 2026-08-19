
namespace GesMgmt.Application.DTOs.Cartera
{
    public class CarteraResponseDto
    {
        public class  GetAnioByIdClienteResponseDto
        {
            public int Anio { get; set; }
        }

        public class GetCarterasParametrosByIdClienteAnnioResponseDto
        {
            public int campanna { get; set; }
            public int anio { get; set; }
            public string desEstado { get; set; }
            public int numero { get; set; }
        }
    }
}