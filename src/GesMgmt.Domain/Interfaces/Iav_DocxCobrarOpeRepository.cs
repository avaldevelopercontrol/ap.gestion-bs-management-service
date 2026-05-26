using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_DocxCobrarOpeRepository
    {
        Task<IQueryable<av_DocxCobrarOpe>> Query();
        Task<av_DocxCobrarOpe?> Get_av_DocxCobrarOpeLastGest(int nId_Cliente, int nId_Cartera, int nId_PersDeudor);
    }
}