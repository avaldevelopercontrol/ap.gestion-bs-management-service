using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Opcion
{
    public class OpcionResponseDto
    {
        public class EditOpcionResponseDto
        {
            public int nId_Opcion { get; set; }
            public string sCodigoOpcion { get; set; }
            public string sNombreOpcion { get; set; }
            public int? nId_OpcionPadre { get; set; }
        }

        public class CreateOpcionResponseDto
        {
            public int nId_Opcion { get; set; }
            public string sCodigoOpcion { get; set; }
            public string sNombreOpcion { get; set; }
            public int? nId_OpcionPadre { get; set; }
        }

        public class GetOpcionesResponseDto
        {
            public int nId_Opcion { get; set; }
            public string sCodigoOpcion { get; set; }
            public string sNombreOpcion { get; set; }
            public string sDescripcionOpcion { get; set; }
            public string sUrlOpcion { get; set; }
            public string? sUrlBI { get; set; }
            public string? sIcono { get; set; }
            public string? sImagenOpcion { get; set; }
            public string? sEmailOpcion { get; set; }
            public int nTipo { get; set; }
            public int? nId_OpcionPadre { get; set; }
            public string sCodigoOpcionPadre { get; set; }
            public string sNombreOpcionPadre { get; set; }
            public int nOrden { get; set; }
            public bool bVisible { get; set; }
            public bool bEstado { get; set; }
            public int nCrea { get; set; }
            public string dFechaCrea { get; set; }
            public int? nModifica { get; set; }
            public string? dFechaModifica { get; set; }
        }

        public class GetOpcionByIdResponseDto
        {
            public int nId_Opcion { get; set; }
            public string sCodigoOpcion { get; set; }
            public string sNombreOpcion { get; set; }
            public string sDescripcionOpcion { get; set; }
            public string sUrlOpcion { get; set; }
            public string? sUrlBI { get; set; }
            public string? sIcono { get; set; }
            public string? sImagenOpcion { get; set; }
            public string? sEmailOpcion { get; set; }
            public int nTipo { get; set; }
            public int? nId_OpcionPadre { get; set; }
            public int nOrden { get; set; }
            public bool bVisible { get; set; }
            public bool bEstado { get; set; }
            public int nCrea { get; set; }
            public string dFechaCrea { get; set; }
            public int? nModifica { get; set; }
            public string? dFechaModifica { get; set; }
        }

        public class GetOpcionByIdPadreResponseDto
        {
            public int nId_Opcion { get; set; }
            public string sCodigoOpcion { get; set; }
            public string sNombreOpcion { get; set; }
            public string sDescripcionOpcion { get; set; }
            public string sUrlOpcion { get; set; }
            public string? sIcono { get; set; }
            public string? sImagenOpcion { get; set; }
            public string? sEmailOpcion { get; set; }
            public int nTipo { get; set; }
            public int? nId_OpcionPadre { get; set; }
            public int nOrden { get; set; }
            public bool bVisible { get; set; }
            public bool bEstado { get; set; }
            public int nCrea { get; set; }
            public DateTime dFechaCrea { get; set; }
            public int? nModifica { get; set; }
            public DateTime? dFechaModifica { get; set; }
        }
    }
}