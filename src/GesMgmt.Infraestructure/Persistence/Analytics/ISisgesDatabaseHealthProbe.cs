namespace GesMgmt.Infraestructure.Persistence.Analytics
{
    public interface ISisgesDatabaseHealthProbe
    {
        Task<DatabaseHealthProbeResult> CheckAsync(
            CancellationToken cancellationToken = default);
    }
}
