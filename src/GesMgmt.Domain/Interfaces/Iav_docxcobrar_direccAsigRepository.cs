using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_docxcobrar_direccAsigRepository
    {
        Task<IQueryable<av_docxcobrar_direccAsig>> Query();
    }
}