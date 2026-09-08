namespace GesMgmt.Infraestructure.Persistence.Analytics
{
    public sealed record DatabaseHealthProbeResult(
        bool IsHealthy,
        string FailureCategory,
        string Description);
}
