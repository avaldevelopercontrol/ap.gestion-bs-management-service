using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PersTelefRepository
    {
        Task<IQueryable<av_PersTelef>> Query();
    }
}