
namespace GesMgmt.Domain.Entities
{
    public class av_PasswordHis
    {
        public int nId_PasswordHis { get; set; }
        public DateTime dFecRegistro { get; set; }
        public int nId_Usuario { get; set; }
        public av_Usuario av_Usuario { get; set; }
        public string cUsr_Pass { get; set; }
        public int nId_UsuarioReg { get; set; }
    }
}