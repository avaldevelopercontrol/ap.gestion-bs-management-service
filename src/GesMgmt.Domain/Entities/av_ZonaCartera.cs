
namespace GesMgmt.Domain.Entities
{
    public class av_ZonaCartera
    {
        public int nid_zona { get; set; }
        public string zona { get; set; }
        public int? nid_ubigeo { get; set; }
        public string? nid_ubigeoDistritos { get; set; }
        public string? secciones { get; set; }
        public int? nid_ubigeoProv { get; set; }
        public int? nid_division { get; set; }
        public av_Divisional av_Divisional { get; set; }
        public int? grupo { get; set; }
        public int nid_cliente { get; set; }
        //public av_Cliente av_Cliente { get; set; }
        public string? region { get; set; }
        public string? region_zona { get; set; }
        public string? tipo_gestion { get; set; }
        public string? telefono { get; set; }
        public string? ciu_responsable { get; set; }
        public string? direccion { get; set; }
        public int? nid_Departamento { get; set; }
        public int? nid_OficinaAval { get; set; }
        public av_OficinaAval av_OficinaAval { get; set; }
        public int? nId_Usuario { get; set; }
        public av_Usuario av_Usuario { get; set; }
        public string? sec_alejada { get; set; }
        public string? cli_Gz_Nombre { get; set; }
        public string? cli_Gz_Telefono { get; set; }
        public string? cli_Gz_Email { get; set; }
        public int? nid_UsuarioCliente { get; set; }
        public int? nId_SubZonaGen { get; set; }
        public av_SubZonaGeneral av_SubZonaGeneral { get; set; }
        public string? sec_alejada_2 { get; set; }
        public bool? bEstadoZona { get; set; }
    }
}