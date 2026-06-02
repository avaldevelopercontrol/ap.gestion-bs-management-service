using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_PersTelef
    {
        public int nId_PersTelef { get; set; }

        public int? nId_PersDeudor { get; set; }
        public int? nId_PersDeudorGestionHrs { get; set; }
        public int? nId_PersRefUbi { get; set; }
        public int? nId_PersTelefOpe { get; set; }

        public av_PersDeudor av_PersDeudor { get; set; }
        public av_PersDeudorGestionHrs av_PersDeudorGestionHrs { get; set; }
        public av_PersRefUbi av_PersRefUbi { get; set; }
        public av_PersTelefOpe av_PersTelefOpe { get; set; }

        public string? nTelef_Pre { get; set; }
        public string? nTelef_Nro { get; set; }
        public string? nTelef_Anexo { get; set; }
        public string? cTelef_Coment { get; set; }
        public bool? bEstado { get; set; }
        public int? nId_PersDirecc { get; set; }
        public int? nTelef_Prioridad { get; set; }
        public DateTime? dFecUlt_PerstelefOpe { get; set; }
        public DateTime? dFecCarga_PersTelef { get; set; }
        public string? cDireccionTEMPORAL { get; set; }
        public int? ncontactados { get; set; }
        public string? base_telf { get; set; } //**
        public string? cbus { get; set; }
        public int? nfuenteBus { get; set; }
        public int? nreferencia { get; set; }
        public int? nid_usuarioupd { get; set; }
        public int? nId_OperadorTelefonico { get; set; }
        public int? nId_EstadoAstkProv { get; set; }
        public DateTime? dFec_EstadoAstkProv { get; set; }
        public int? nId_TipoTelefono { get; set; }
        public int? nNoContactados { get; set; }
        public int? nCant_Ivr { get; set; }
        public int? nOrden_Act { get; set; }
        public bool? bReclamo { get; set; }
        public string? c_osiptel { get; set; }
        public string? c_modalidad_osiptel { get; set; }
        public string? c_operadora_osiptel { get; set; }
        public DateTime? f_estado_osiptel { get; set; }
        public string? Nombre { get; set; }
        public string? Contacto { get; set; }
        public string? Parentesco { get; set; }
    }
}