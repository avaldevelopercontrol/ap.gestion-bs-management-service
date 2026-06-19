
namespace GesMgmt.Application.DTOs.Gestion
{
    public class GetGestionCabeResponseDto
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
}