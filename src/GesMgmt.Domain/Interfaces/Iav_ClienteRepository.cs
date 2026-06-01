using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_ClienteRepository
    {
        Task<IQueryable<av_Cliente>> Query();
    }
}