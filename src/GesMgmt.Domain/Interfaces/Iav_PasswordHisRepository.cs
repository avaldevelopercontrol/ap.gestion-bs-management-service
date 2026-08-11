using GesMgmt.Domain.Entities;

namespace GesMgmt.Domain.Interfaces
{
    public interface Iav_PasswordHisRepository
    {
        Task<IQueryable<av_PasswordHis>> Query();
        Task<av_PasswordHis> ByIdAsync(int nId_PasswordHis);
        Task<av_PasswordHis> ByClavePorFechaAsync(int nId_Usuario, string cUsr_Pass, DateTime dFecRegistro);
        Task<av_PasswordHis> AddAsync(av_PasswordHis av_PasswordHis);
        Task<av_PasswordHis> UpdateAsync(av_PasswordHis av_PasswordHis);
    }
}