namespace GesMgmt.Domain.Entities
{
    public class av_TipoContacto
    {
        public int nId_TipoContacto { get; set; }
        public string? cNomTipoContacto { get; set; }
        public string? cGrupoNivel2 { get; set; }
        public string? cGrupoNivel1 { get; set; }
        public int? nOrdenPresentacion { get; set; }
        public int? nId_StatusAltitude { get; set; }
        public string? cLlaveAltitude { get; set; }
        public string? indicador_equiv { get; set; }
        public string? indicador_equiv_desc { get; set; }
    }
}