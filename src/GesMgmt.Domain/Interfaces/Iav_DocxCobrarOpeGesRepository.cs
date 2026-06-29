using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_DocxCobrarOpeGesRepository
    {
        Task<IQueryable<av_DocxCobrarOpeGes>> Query();
        Task<av_DocxCobrarOpeGes> AddAsync(av_DocxCobrarOpeGes av_DocxCobrarOpeGes);
        Task<av_DocxCobrarOpeGes> UpdateAsync(av_DocxCobrarOpeGes av_DocxCobrarOpeGes);
        Task<IEnumerable<av_DocxCobrarOpeGes>> AddRangeAsync(IEnumerable<av_DocxCobrarOpeGes> entities);
    }
}