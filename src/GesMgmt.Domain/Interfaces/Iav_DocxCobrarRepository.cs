using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{

    public interface Iav_DocxCobrarRepository
    {
        IQueryable<av_DocxCobrar> GetGestionesAsync(av_DocxCobrar av_DocxCobrar);
    }

}