using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_DetallePersTelefRepository
    {
        Task<IQueryable<av_DetallePersTelef>> Query();
        IQueryable<av_DetallePersTelef> GetDetalleTelefonosAsync(av_DetallePersTelef av_DetallePersTelef);
    }
}