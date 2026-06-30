using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Cliente
{
    public class ClienteRequestDto
    {
        public class GetClienteUsuarioGrupoRequestDto
        {
            public int nId_Usuario { get; set; }
            public int nId_Perfil { get; set; }
        }
    }
}