using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PersEmailRepository
    {
        Task<IQueryable<av_PersEmail>> Query();
        IQueryable<av_PersEmail?> GetEmailsByIdDeudorAsync(int nId_Cliente, int nId_PersDeudor);
        IQueryable<av_PersEmail> GetEmailsByIdPersEmail(int nId_PersEmail);
        Task<av_PersEmail> AddAsync(av_PersEmail av_PersEmail);
        Task<av_PersEmail> UpdateAsync(av_PersEmail av_PersEmail);
    }
}