
namespace GesMgmt.Domain.Entities
{
    public class av_Divisional
    {
        public int nid_division { get; set; }
        public int? nid_ubigeo { get; set; }
        public int? nid_ubigeoProv { get; set; }
        public string? nom_divisional { get; set; }
        public string? nid_ubigeos { get; set; }
        public int? nid_cliente { get; set; }
        public int? nId_UsuarioCliente { get; set; }
        public string? jefeDivisional { get; set; }
    }
}