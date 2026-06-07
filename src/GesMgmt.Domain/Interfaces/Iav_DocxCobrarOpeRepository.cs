using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_DocxCobrarOpeRepository
    {
        Task<IQueryable<av_DocxCobrarOpe>> Query();
        IQueryable<av_DocxCobrarOpe?> GetGestionesCarteraDeudor(int nId_Cliente, int nId_Cartera, int nId_PersDeudor, int? nId_PerfilUsuario);
    }
}