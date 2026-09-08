namespace GesMgmt.Infraestructure.Persistence.Analytics
{
    public interface IAnalyticsDatabaseHealthProbe
    {
        Task<DatabaseHealthProbeResult> CheckAsync(
            CancellationToken cancellationToken = default);
    }
}
