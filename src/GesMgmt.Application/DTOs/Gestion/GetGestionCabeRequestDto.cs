
namespace GesMgmt.Application.DTOs.Gestion
{
    public class GetGestionCabeRequestDto
    {
        public GetGestionCabeRequestDto()
        {
            nId_Cliente = 0;
            nId_Contrato = 0;
        }

        public int nId_Cliente { get; set; }
        public int nId_Contrato { get; set; }
    }
}