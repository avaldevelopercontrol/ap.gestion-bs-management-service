using GesMgmt.Domain.Entities;

namespace GesMgmt.Application.DTOs.Grupo
{
    public class GrupoResponseDto
    {
        public class GetGrupoListResponseDto
        {
            public int nId_Grupo { get; set; }
            public string cNombre_Grupo { get; set; }
        }

        public class GetGruposResponseDto
        {
            public int nId_Grupo { get; set; }
            public string? cNombre_Grupo { get; set; }
            public string? cSigla_Grupo { get; set; }
            public bool? bEstado { get; set; }
            public int? nCant_Grupo { get; set; }
            public int? nid_cliente { get; set; }
            public string? cCli_Nombre { get; set; }
        }

        public class GetGrupoByIdResponseDto
        {
            public int nId_Grupo { get; set; }
            public string? cNombre_Grupo { get; set; }
            public string? cSigla_Grupo { get; set; }
            public bool? bEstado { get; set; }
            public int? nCant_Grupo { get; set; }
            public int? nid_cliente { get; set; }
        }

        public class CreateGrupoResponseDto
        {
            public int nId_Grupo { get; set; }
            public string? cNombre_Grupo { get; set; }
            public int? nid_cliente { get; set; }
        }

        public class EditGrupoResponseDto
        {
            public int nId_Grupo { get; set; }
            public string? cNombre_Grupo { get; set; }
            public int? nid_cliente { get; set; }
        }
    }
}