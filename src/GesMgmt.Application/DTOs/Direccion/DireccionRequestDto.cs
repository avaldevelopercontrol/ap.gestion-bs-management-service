using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Direccion
{
    public class DireccionRequestDto
    {
        public class CreateDireccionRequestDto
        {
            public int nId_PersDirecc { get; set; }
            public int? nId_PersDeudor { get; set; }
            public string? cDirecc_Nomb { get; set; }
            public int? nId_ubigeo { get; set; }
            public int? nId_PersRefUbi { get; set; }
            public string? cDirecc_Coment { get; set; }
            public bool? bEstado { get; set; }
            public bool? bOrigen_Base { get; set; }
            public string? cTipoCoDeudor { get; set; }
            public DateTime? dFec_Actualizacion { get; set; }
            public int? nId_Cliente { get; set; }
            public int? nid_CalifDirecc { get; set; }
            public int? nid_usuarioUpd { get; set; }
        }

        public class EditDireccionRequestDto
        {
            public int nId_PersDirecc { get; set; }
            public int? nId_PersDeudor { get; set; }
            public string? cDirecc_Nomb { get; set; }
            public int? nId_ubigeo { get; set; }
            public int? nId_PersRefUbi { get; set; }
            public string? cDirecc_Coment { get; set; }
            public bool? bEstado { get; set; }
            public bool? bOrigen_Base { get; set; }
            public string? cTipoCoDeudor { get; set; }
            public DateTime? dFec_Actualizacion { get; set; }
            public int? nId_Cliente { get; set; }
            public int? nid_CalifDirecc { get; set; }
            public int? nid_usuarioUpd { get; set; }
        }
    }
}