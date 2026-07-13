
namespace GesMgmt.Domain.Entities
{
    public class av_Usuario
    {
        public int nId_Usuario { get; set; }
        public string cUsr_NroDoc { get; set; }
        public string? cUsr_ApePat { get; set; }
        public string? cUsr_ApeMat { get; set; }
        public string? cUsr_Nombres { get; set; }
        public int bSexo { get; set; }
        public string cUsr_Login { get; set; }
        public string cUsr_Pass { get; set; }
        public bool bEstado { get; set; }
        public decimal? mUsr_CostoMes { get; set; }
        public int? nId_Horario { get; set; }
        public int? nUsr_CtaNroAcum { get; set; }
        public decimal? nUsr_CtaMontoAcum { get; set; }
        public decimal? nUsr_CtaMontoRecAcum { get; set; }
        public decimal? nUsr_CtaMontoRecEfi { get; set; }
        public string? cUsr_Anexo { get; set; }
        public string? cUsr_Celular { get; set; }
        public string? cUsr_Email { get; set; }
        public string? cUsr_Telef { get; set; }
        public int? nId_UTipo { get; set; }
        public int? nId_Cargo { get; set; }
        public DateTime? dUsr_FecNac { get; set; }
        public DateTime? dUsr_FecIngreso { get; set; }
        public int? nId_Mtabla { get; set; }
        public string? cUsr_Direcc { get; set; }
        public int nId_Ubigeo { get; set; }
        public string? cUsr_DireccRef { get; set; }
        public int? nId_Grupo { get; set; }
        public int? nId_Sucursal { get; set; }
        public DateTime? dUsr_FecSalida { get; set; }
        public int? nId_UEstado { get; set; }
        public int? nid_perfil { get; set; }
        public string? cod_Recau { get; set; }
        public string? nUsr_CiuGestor { get; set; }
        public string? nUsr_Zona { get; set; }
        public string? cComp_Zona { get; set; }
        public bool? bValidaGesAsterisk { get; set; }
        public string? cGestionaEstado { get; set; }
        public int? NroCampanaDiscador { get; set; }
        public string? cUsr_EmailPersonal { get; set; }
        public int? nId_ZonaGen { get; set; }
        public DateTime? dUsr_PassUpdate { get; set; }
        public int? nUsr_NroIntentoAcc { get; set; }
        public string? cUsr_EmailProfile { get; set; }
        public int? nId_PerfilGest { get; set; }
        public int? nId_ClientePri { get; set; }
        public int? nId_SubZonaGen { get; set; }
        public int? nBuscarReniec { get; set; }
        public int? nid_UsuSuper { get; set; }
        public DateTime? dUsr_FecCese { get; set; }
        public bool? bEmailVerificacion { get; set; }
        public string? cEmailVerificacion_codigo { get; set; }
        public string? cUsr_EmailVerificacion { get; set; }
        public DateTime? dFechaHora_Codigo { get; set; }
    }
}