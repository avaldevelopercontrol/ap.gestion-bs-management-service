using System.Data.Common;

namespace GesMgmt.Infraestructure.Persistence.Analytics
{
    internal interface IAnalyticsDbConnectionFactory
    {
        Task<DbConnection> OpenConnectionAsync(
            CancellationToken cancellationToken = default);
    }
}
