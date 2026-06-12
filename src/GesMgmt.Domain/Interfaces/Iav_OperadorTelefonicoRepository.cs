using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_OperadorTelefonicoRepository
    {
        Task<IQueryable<av_OperadorTelefonico>> Query();
    }
}