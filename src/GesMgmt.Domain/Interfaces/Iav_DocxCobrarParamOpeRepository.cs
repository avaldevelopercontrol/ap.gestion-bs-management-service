using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_DocxCobrarParamOpeRepository
    {
        Task<IQueryable<av_DocxCobrarParamOpe>> Query();
    }
}