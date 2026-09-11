
namespace GesMgmt.Domain.Entities
{
    public class av_BotonCliente
    {
        public int nId_Boton { get; set; }
        public int nId_Cliente { get; set; }
        public av_Cliente av_Cliente { get; set; }
        public int nId_Contrato { get; set; }
        public av_Contrato av_Contrato { get; set; }
        public string nombreBoton { get; set; }
        public string descripcionBoton { get; set; }
        public bool bEstado { get; set; }
        public int nCrea { get; set; }
        public DateTime dFechaCrea { get; set; }
        public int? nModifica { get; set; }
        public DateTime? dFechaModifica { get; set; }
    }
}