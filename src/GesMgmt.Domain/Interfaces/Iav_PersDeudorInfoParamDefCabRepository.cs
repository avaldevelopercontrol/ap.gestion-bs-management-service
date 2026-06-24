using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PersDeudorInfoParamDefCabRepository
    {
        Task<IQueryable<av_PersDeudorInfoParamDefCab>> Query();
        Task<av_PersDeudorInfoParamDefCab> GetPersDeudorInfoParamDefCabAsync(bool tipoCabecera);
    }
}