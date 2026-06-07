using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_TipoGestionRepository
    {
        Task<IQueryable<av_TipoGestion>> Query();
    }
}