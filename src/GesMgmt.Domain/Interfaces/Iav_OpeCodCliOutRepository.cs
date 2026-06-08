using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_OpeCodCliOutRepository
    {
        Task<IQueryable<av_OpeCodCliOut>> Query();
    }
}