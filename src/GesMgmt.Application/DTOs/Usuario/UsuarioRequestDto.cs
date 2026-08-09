
namespace GesMgmt.Application.DTOs.Usuario
{
    public class UsuarioRequestDto
    {
        public class GetCampannaDiscadorlListRequestDto
        {
            public int nId_Usuario { get; set; }
        }

        public class EditUsuarioRequestDto
        {
            public int nId_Usuario { get; set; }
            public string cUsr_NroDoc { get; set; }
            public string cUsr_NroDocNew { get; set; }
            public string cUsr_ApePat { get; set; }
            public string cUsr_ApeMat { get; set; }
            public string cUsr_Nombres { get; set; }
            public string cUsr_Login { get; set; }
            public string cUsr_LoginNew { get; set; }
            public bool bCambioPass { get; set; }
            public string cUsr_Pass { get; set; }
            public string cUsr_PassNew { get; set; }
            public int nid_perfil { get; set; }
            public int nId_Grupo { get; set; }
            public string? cod_Recau { get; set; }
            public bool bEstado { get; set; }
            public DateTime? dUsr_FecNac { get; set; }
            public int bSexo { get; set; }
            public int nId_Ubigeo { get; set; }
            public string? nUsr_CiuGestor { get; set; }
            public int? nId_SubZonaGen { get; set; }
            public string? cUsr_Celular { get; set; }
            public string? cUsr_Anexo { get; set; }
            public string? cUsr_AnexoNew { get; set; }
            public string? cUsr_Email { get; set; }
            public string? cUsr_EmailPersonal { get; set; }
            public int? NroCampanaDiscador { get; set; }
        }

        public class CreateUsuarioRequestDto
        {
            public int nId_Usuario { get; set; }
            public string cUsr_NroDoc { get; set; }
            public string cUsr_ApePat { get; set; }
            public string cUsr_ApeMat { get; set; }
            public string cUsr_Nombres { get; set; }
            public string cUsr_Login { get; set; }
            public string cUsr_Pass { get; set; }
            public int nid_perfil { get; set; }
            public int nId_Grupo { get; set; }
            public string? cod_Recau { get; set; }
            public bool bEstado { get; set; }
            public DateTime? dUsr_FecNac { get; set; }
            public int bSexo { get; set; }
            public int nId_Ubigeo { get; set; }
            public string? nUsr_CiuGestor { get; set; }
            public int? nId_SubZonaGen { get; set; }
            public string? cUsr_Celular { get; set; }
            public string? cUsr_Anexo { get; set; }
            public string? cUsr_Email { get; set; }
            public string? cUsr_EmailPersonal { get; set; }
            public int? NroCampanaDiscador { get; set; }
        }

        public class GetUsuarioLoginRequestDto
        {
            public string cUsr_Login { get; set; }
            public string cUsr_Pass { get; set; }
        }

        
    }
}