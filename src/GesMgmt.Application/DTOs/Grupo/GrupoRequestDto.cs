
namespace GesMgmt.Application.DTOs.Grupo
{
    public class GrupoRequestDto
    {

        public class CreateGrupoRequestDto
        {
            public int nId_Grupo { get; set; }
            public string? cNombre_Grupo { get; set; }
            public string? cSigla_Grupo { get; set; }
            public bool? bEstado { get; set; }
            public int? nCant_Grupo { get; set; }
            public int? nid_cliente { get; set; }
        }

        public class EditGrupoRequestDto
        {
            public int nId_Grupo { get; set; }
            public string? cNombre_Grupo { get; set; }
            public string? cNombre_GrupoNuevo { get; set; }
            public string? cSigla_Grupo { get; set; }
            public bool? bEstado { get; set; }
            public int? nCant_Grupo { get; set; }
            public int? nid_cliente { get; set; }
        }
    }
}