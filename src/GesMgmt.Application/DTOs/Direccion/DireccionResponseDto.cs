using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Direccion
{
    public class DireccionResponseDto
    {
        public class GetDireccionAsync
        {
            public int nId_PersDirecc { get; set; }
            public string? cNombre_PersRefUbi { get; set; }
            public string? cDirecc_Nomb { get; set; }
            public string? tipoDeudor { get; set; }
            public string? nombreAval { get; set; }
            public string? estado { get; set; }
            public int? nId_PersRefUbi { get; set; }
            public string? cDirecc_Coment { get; set; }
            public bool? bEstado { get; set; }
            public bool? bOrigen_Base { get; set; }
            public int? nId_PersTitDeudor { get; set; }
            public string? cTipoCoDeudor { get; set; }
            public int? nid_CalifDirecc { get; set; }
            public string cDescrip_Fija { get; set; }
            public int? nId_Ubigeo { get; set; }
            public int? nId_Departamento { get; set; } 
            public int? nId_Provincia { get; set; }
            public int? nId_Distrito { get; set; }
        }

        public class GetUbigeoDepartamentos
        {
            public int nId_Departamento { get; set; }
            public string? cNombre_Departamento { get; set; }
        }

        public class GetUbigeoProvincias
        {
            public int nId_Provincia { get; set; }
            public string? cNombre_Provincia { get; set; }
        }

        public class GetUbigeoDistritos
        {
            public int nId_Distrito { get; set; }
            public string? cNombre_Distrito { get; set; }
        }
    }
}