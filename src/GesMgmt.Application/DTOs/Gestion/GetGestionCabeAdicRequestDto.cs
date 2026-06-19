
namespace GesMgmt.Application.DTOs.Gestion
{
    public class GetGestionCabeAdicRequestDto
    {
        public GetGestionCabeAdicRequestDto()
        {
            nId_Cliente = 0;
            pantalla = 0;
        }

        public int nId_Cliente { get; set; }
        public int pantalla { get; set; }
    }
}