using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PersTelefRepository
    {
        Task<IQueryable<av_PersTelef>> Query();
        Task<av_PersTelef> GetTelefonoByIdTelefonoAsync(int nId_PersTelef);
        IQueryable<av_PersTelef> GetTelefonosAsync(av_PersTelef av_PersTelef);
        Task<av_PersTelef> GetTelefonoNroTelefonoByIdDeudorAsync(string nTelef_Nro, int nId_PersDeudor);
        Task<av_PersTelef> GetTelefonoNroTelefonoAsync(string nTelef_Nro);
        Task<av_PersTelef> AddAsync(av_PersTelef av_PersTelef);
        Task<av_PersTelef> UpdateAsync(av_PersTelef av_PersTelef)
    }
}