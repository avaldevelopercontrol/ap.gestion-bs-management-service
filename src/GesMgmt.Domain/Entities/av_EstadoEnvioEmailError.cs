using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_EstadoEnvioEmailError
    {
        public int nId_EstadoEnvioEmail { get; set; }
        public string? cCodEstadoEnvio { get; set; }
        public string? cDesEstadoEnvio_Orig { get; set; }
        public string? cDesEstadoEnvio_Esp { get; set; }
    }
}