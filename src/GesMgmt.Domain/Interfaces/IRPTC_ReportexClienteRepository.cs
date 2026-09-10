using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface IRPTC_ReportexClienteRepository
    {
        Task<IQueryable<RPTC_ReportexCliente>> Query();
    }
}