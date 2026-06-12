using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PersRefUbiRepository
    {
        Task<IQueryable<av_PersRefUbi>> Query();
        IQueryable<av_PersRefUbi> GetUbicacionesTelefono();
    }
}