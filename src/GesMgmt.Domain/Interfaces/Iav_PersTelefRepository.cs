using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PersTelefRepository
    {
        Task<IQueryable<av_PersTelef>> Query();
        IQueryable<av_PersTelef> GetTelefonosAsync(av_PersTelef av_PersTelef);
    }
}