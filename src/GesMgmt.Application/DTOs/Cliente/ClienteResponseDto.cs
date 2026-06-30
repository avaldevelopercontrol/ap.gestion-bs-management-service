using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Cliente
{
    public class ClienteResponseDto
    {
        public class GetClienteUsuarioGrupoResponsetDto
        {
            public int nId_Cliente { get; set; }
            public string cCli_Nombre { get; set; }
            public int? swt_estadoGest { get; set; }
            public int? ntip_campanna { get; set; }
        }
    }
}