using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_EstadoAsteriskAval
    {
        public int nId_EstadoAsteriskAval { get; set; }
        public int? nId_PersDeudor { get; set; }
        public int? nId_Cartera { get; set; }
        public int? nId_Usuario { get; set; }
        public DateTime? dFec_Inicio { get; set; }
        public DateTime? dFec_Conexion { get; set; }
        public DateTime? dFec_Fin { get; set; }
        public DateTime? dFec_Registro { get; set; }
        public string? cNumeroDestino { get; set; }
        public string? cChannelOrigen { get; set; }
        public string? cMessage { get; set; }
        public string? cEvent { get; set; }
        public string? cPrivilege { get; set; }
        public string? cChannel { get; set; }
        public string? cCallerIDNum { get; set; }
        public string? cCallerIDName { get; set; }
        public string? cAccountcode { get; set; }
        public string? cChannelState { get; set; }
        public string? cChannelStateDesc { get; set; }
        public string? cContext { get; set; }
        public string? cExtension { get; set; }
        public string? cPriority { get; set; }
        public string? cSeconds { get; set; }
        public string? cBridgedChannel { get; set; }
        public string? cBridgedUniqueid { get; set; }
        public string? cUniqueid { get; set; }
        public string? cTipoDiscador { get; set; }
        public string? cProveedor { get; set; }
        public string? RUTA_GRABACION { get; set; }
        public string? ARCHIVO_GRABACION { get; set; }
        public string? ID_MAILING { get; set; }
        public string? CAMPANIA { get; set; }
        public int? THREAD { get; set; }
    }
}