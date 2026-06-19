
namespace GesMgmt.Application.DTOs.Gestion
{
    public class GetGestionEstaGestCartDeudResponseDto
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
}