using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_DocxCobrarAdicionalRepository
    {
        Task<IQueryable<av_DocxCobrarAdicional>> Query();
        IQueryable<av_DocxCobrarAdicional> GetGestionesAdicionalesAsync(av_DocxCobrarAdicional av_DocxCobrarAdicional);
    }
}