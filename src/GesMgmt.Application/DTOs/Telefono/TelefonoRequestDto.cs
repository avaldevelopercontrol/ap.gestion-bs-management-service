using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Telefono
{
    public class TelefonoRequestDto
    {
        public class CreateTelefonoRequestDto
        {
            public int nId_PersTelef { get; set; }
            public int? nId_PersDeudor { get; set; }
            public string? nTelef_Pre { get; set; }
            public string? nTelef_Nro { get; set; }
            public string? nTelef_Anexo { get; set; }
            public int? nId_PersRefUbi { get; set; } //en sisges Ubicación* 
            public int? nTelef_Prioridad { get; set; }
            public string? cTelef_Coment { get; set; }
            public int? nId_PersDeudorGestionHrs { get; set; } // en sisges Horario de Gestión 
            public int? nId_PersTelefOpe { get; set; } //en sisges Resultado* 
            public int? nId_Fuente { get; set; } //en sisges Fuente Búsqueda
            public int? nreferencia { get; set; }
            public int? nid_usuarioupd { get; set; }
            public int? nId_OperadorTelefonico { get; set; } //en sisges Operador Telefónico*
            public bool? bEstado { get; set; }
            public DateTime? dFecUlt_PerstelefOpe { get; set; }
            public DateTime? dFecCarga_PersTelef { get; set; }
            public bool? bReclamo { get; set; }
        }

        public class EditTelefonoRequestDto
        {
            public int nId_PersTelef { get; set; }
            public int? nId_PersDeudor { get; set; }
            public string? nTelef_Pre { get; set; }
            public string? nTelef_Nro { get; set; }
            public string? nTelef_Anexo { get; set; }
            public int? nId_PersRefUbi { get; set; } //en sisges Ubicación* 
            public int? nTelef_Prioridad { get; set; }
            public string? cTelef_Coment { get; set; }
            public int? nId_PersDeudorGestionHrs { get; set; } // en sisges Horario de Gestión 
            public int? nId_PersTelefOpe { get; set; } //en sisges Resultado* 
            public int? nId_Fuente { get; set; } //en sisges Fuente Búsqueda
            public int? nreferencia { get; set; }
            public int? nid_usuarioupd { get; set; }
            public int? nId_OperadorTelefonico { get; set; } //en sisges Operador Telefónico*
            public bool? bEstado { get; set; }
            public DateTime? dFecUlt_PerstelefOpe { get; set; }
            public DateTime? dFecCarga_PersTelef { get; set; }
            public bool? bReclamo { get; set; }
        }
    }
}