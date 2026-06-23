using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_EstadoEnvioEmailErrorRepository
    {
        Task<IQueryable<av_EstadoEnvioEmailError>> Query();
    }
}