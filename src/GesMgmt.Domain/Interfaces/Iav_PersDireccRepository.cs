using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PersDireccRepository
    {
        Task<IQueryable<av_PersDirecc>> Query();
        IQueryable<av_PersDirecc> GetGestionesDireccionesAsync(av_PersDirecc av_PersDirecc);
    }
}