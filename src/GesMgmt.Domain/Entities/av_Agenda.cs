
namespace GesMgmt.Domain.Entities
{
    public class av_Agenda
    {
        public int nid_agenda { get; set; }
        public DateTime? dFechNuevaGestion { get; set; }
        public int? nid_PersDeudor { get; set; }
        public string? Nombre { get; set; }
        public string? Cartera { get; set; }
        public int? nid_Cartera { get; set; }
        public int? nid_Cliente { get; set; }
        public int? nid_UsuOpe { get; set; }
        public DateTime? dFecRegistro { get; set; }
        public string? cUsr_Login { get; set; }
        public int? nId_TipoOpeCodCliOut { get; set; }
        public string? cRespuestaOpe { get; set; }
        public int? nId_OpeCodCliOut { get; set; }
    }
}