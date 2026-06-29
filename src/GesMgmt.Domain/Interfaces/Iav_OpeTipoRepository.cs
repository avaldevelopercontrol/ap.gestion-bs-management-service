using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_OpeTipoRepository
    {
        Task<IQueryable<av_OpeTipo>> Query();
    }
}