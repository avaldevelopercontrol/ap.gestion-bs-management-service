using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_DocxPagoRepository
    {
        Task<IQueryable<av_DocxPago>> Query();
    }
}