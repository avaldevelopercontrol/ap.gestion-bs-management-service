using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_ContFormTipoParamOpeRepository
    {
        Task<IQueryable<av_ContFormTipoParamOpe>> Query();
    }
}