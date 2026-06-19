
namespace GesMgmt.Application.DTOs.Gestion
{
    public class GetGestionDireResponseDto
    {
        public int nId_PersDirecc { get; set; }
        public string? direccion { get; set; }
        public string? referenciaUbicacion { get; set; }
        public string? tipoDeudor { get; set; }
        public string? nombre { get; set; }
        public string? estado { get; set; }
    }
}