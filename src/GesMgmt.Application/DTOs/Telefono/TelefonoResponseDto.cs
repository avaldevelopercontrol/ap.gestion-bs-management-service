using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Telefono
{
    public class TelefonoResponseDto
    {
        public class GetTelefonoResultados
        {
            public int nId_PersTelefOpe { get; set; }
            public string? cNombre_PersTelefOpe { get; set; }
            public string? cSigla_PersTelefOpe { get; set; }
            public bool? bEstado { get; set; }
        }

        public class GetTelefonoOperadores
        {
            public int nId_OperadorTelefonico { get; set; }
            public string? cNombreOperadorTelef { get; set; }
            public string? cAbrevOperadorTelef { get; set; }
            public bool? bEstado { get; set; }
        }

        public class GetTelefonoUbicaciones
        {
            public int nId_PersRefUbi { get; set; }
            public string? cNombre_PersRefUbi { get; set; }
            public string? cSigla_PersRefUbi { get; set; }
            public bool? bEstado { get; set; }
            public int? nGestionMovil { get; set; }
        }

        public class GetTelefonoHorarioGestion
        {
            public int nId_PersDeudorGestionHrs { get; set; }
            public string? cNombren_PersDeudorGestionHrs { get; set; }
            public string? cSigla_PersDeudorGestionHrs { get; set; }
            public bool? bEstado { get; set; }
            public int? nHr_ini { get; set; }
            public int? nHr_fin { get; set; }
        }

        public class GetTelefonoFuenteBusqueda
        {
            public int nId_Fuente { get; set; }
            public string? cDescripcion { get; set; }
            public int? nId_Cliente_Ref { get; set; }
            public string? nId_Referencia { get; set; }
            public string? cNombre_Referencia { get; set; }
        }

        public class GetTelefonoAsync
        {
            public int nId_PersTelef { get; set; }
            public int? nId_PersDeudor { get; set; }
            public av_PersDeudor? av_PersDeudor { get; set; }
            public string? nTelef_Pre { get; set; }
            public string? nTelef_Nro { get; set; }
            public string? nTelef_Anexo { get; set; }
            public int? nId_PersRefUbi { get; set; } //en sisges Ubicación* 
            //public av_PersRefUbi? av_PersRefUbi { get; set; }
            public string? cTelef_Coment { get; set; }
            public bool? bEstado { get; set; }
            public int? nId_PersDirecc { get; set; }
            public int? nTelef_Prioridad { get; set; }
            public int? nId_PersTelefOpe { get; set; } //en sisges Resultado* 
            //public av_PersTelefOpe av_PersTelefOpe { get; set; }
            public int? nId_PersDeudorGestionHrs { get; set; } // en sisges Horario de Gestión 
            //public av_PersDeudorGestionHrs? av_PersDeudorGestionHrs { get; set; }
            public string? dFecUlt_PerstelefOpe { get; set; }
            public string? dFecCarga_PersTelef { get; set; }
            public string? cDireccionTEMPORAL { get; set; }
            public int? ncontactados { get; set; }
            public string? baseTelef { get; set; }
            public string? cbus { get; set; }
            public int? nId_Fuente { get; set; } //en sisges Fuente Búsqueda
            public int? nreferencia { get; set; }
            public int? nid_usuarioupd { get; set; }
            public int? nId_OperadorTelefonico { get; set; } //en sisges Operador Telefónico*
            public int? nId_EstadoAstkProv { get; set; }
            public string? dFec_EstadoAstkProv { get; set; }
            public int? nId_TipoTelefono { get; set; }
            public int? nNoContactados { get; set; }
            public int? nCant_Ivr { get; set; }
            public int? nOrden_Act { get; set; }
            public bool? bReclamo { get; set; }
            public string? c_osiptel { get; set; }
            public string? c_modalidad_osiptel { get; set; }
            public string? c_operadora_osiptel { get; set; }
            public string? f_estado_osiptel { get; set; }
            public string? Nombre { get; set; }
            public string? Contacto { get; set; }
            public string? Parentesco { get; set; }
        }

        public class CreateTelefonoResponseDto
        {
            public int nId_PersTelef { get; set; }
            public int? nId_PersDeudor { get; set; }
            public string? nTelef_Nro { get; set; }
        }

        public class EditTelefonoResponseDto
        {
            public int nId_PersTelef { get; set; }
            public int? nId_PersDeudor { get; set; }
            public string? nTelef_Nro { get; set; }
        }
    }
}