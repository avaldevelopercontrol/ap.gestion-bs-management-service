using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_AgendaRepository
    {
        Task<IQueryable<av_Agenda>> Query();
    }
}