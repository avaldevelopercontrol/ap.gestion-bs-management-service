using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_MaeTablaRepository
    {
        Task<IQueryable<av_MaeTabla>> Query();
    }
}