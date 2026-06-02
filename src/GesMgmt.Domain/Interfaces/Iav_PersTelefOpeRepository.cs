using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PersTelefOpeRepository
    {
        Task<IQueryable<av_PersTelefOpe>> Query();
    }
}