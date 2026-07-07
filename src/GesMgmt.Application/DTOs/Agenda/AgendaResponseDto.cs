using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Agenda
{
    public class AgendaResponseDto
    {
        public class CreateAgendaResponseDto
        {
            public int? nid_Cliente { get; set; }
            public int? nid_Cartera { get; set; }
            public int? nid_UsuOpe { get; set; }
            public int nid_agenda { get; set; }
            public int? nid_PersDeudor { get; set; }
        }
    }
}