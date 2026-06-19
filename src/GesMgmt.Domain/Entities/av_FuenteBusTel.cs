
namespace GesMgmt.Domain.Entities
{
    public class av_FuenteBusTel
    {
        public int nId_Fuente { get; set; }
        public string? cDescripcion { get; set; }
        public int? nId_Cliente_Ref { get; set; }
        public string? nId_Referencia { get; set; }
        public string? cNombre_Referencia { get; set; }
    }
}