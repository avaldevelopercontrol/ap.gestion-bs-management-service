using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_ContFormularioRptcRepository
    {
        Task<IQueryable<av_ContFormularioRptc>> Query();
    }
}