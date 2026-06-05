using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_PersDirecc
    {
        public int nId_PersDirecc { get; set; }
        public int nId_PersDeudor { get; set; }
        public av_PersDeudor av_PersDeudor { get; set; }
        public string? cDirecc_Nomb { get; set; }
        public int? nId_ubigeo { get; set; }
        public int? nId_PersRefUbi { get; set; }
        public av_PersRefUbi? av_PersRefUbi { get; set; }
        public string? cDirecc_Coment { get; set; }
        public bool? bEstado { get; set; }
        public string? cCli_UbigeoCod { get; set; }
        public string? cCli_UbigeoDpto { get; set; }
        public string? cCli_UbigeoProv { get; set; }
        public string? cCli_UbigeoDistr { get; set; }
        public string? cCli_UbigeoPais { get; set; }
        public DateTime? dFecCarga_PersDirecc { get; set; }
        public bool? bEstado_Activo { get; set; }
        public DateTime? dFec_UltVisita { get; set; }
        public int? nId_Cliente { get; set; }
        public av_Cliente? av_Cliente { get; set; }
        public bool? bOrigen_Base { get; set; }
        public int? nId_PersTitDeudor { get; set; }
        public string? cTipoCoDeudor { get; set; }
        public DateTime? dFec_Actualizacion { get; set; }
        public int? nid_CalifDirecc { get; set; }
        public string? cDescrip_Fija { get; set; }
        public int? nid_usuarioUpd { get; set; }
    }
}