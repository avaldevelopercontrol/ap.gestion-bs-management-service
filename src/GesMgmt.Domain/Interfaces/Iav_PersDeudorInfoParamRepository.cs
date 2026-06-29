using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PersDeudorInfoParamRepository
    {
        Task<IQueryable<av_PersDeudorInfoParam>> Query();
        Task<av_PersDeudorInfoParam> GetGestionInformacionDeudorParamAsync(int nId_PersDeudor);
    }
}