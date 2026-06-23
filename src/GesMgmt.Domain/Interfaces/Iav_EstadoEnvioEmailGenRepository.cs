using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_EstadoEnvioEmailGenRepository
    {
        Task<IQueryable<av_EstadoEnvioEmailGen>> Query();
    }
}