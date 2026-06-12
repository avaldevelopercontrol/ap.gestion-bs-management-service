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
        Task<av_DetallePersTelef> GetDetalleTelefonoSearchAsync(int nId_Cliente, int nId_PersTelef);
        Task<av_DetallePersTelef> AddAsync(av_DetallePersTelef av_DetallePersTelef);
    }
}