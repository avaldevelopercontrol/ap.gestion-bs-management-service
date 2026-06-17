using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_DivisionalRepository
    {
        Task<IQueryable<av_Divisional>> Query();
    }
}