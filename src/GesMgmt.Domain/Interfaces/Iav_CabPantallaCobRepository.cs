using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_CabPantallaCobRepository
    {
        Task<IQueryable<av_CabPantallaCob>> Query();
        IQueryable<av_CabPantallaCob> GetCabeceraGestionesAsync(av_CabPantallaCob av_CabPantallaCob);
    }
}