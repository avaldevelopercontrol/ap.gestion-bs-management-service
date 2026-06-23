using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PersEmailOpeRepository
    {
        Task<IQueryable<av_PersEmailOpe>> Query();
    }
}