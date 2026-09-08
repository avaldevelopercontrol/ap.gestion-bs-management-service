namespace GesMgmt.Infraestructure.Persistence.Analytics
{
    public sealed class SisgesDatabaseOptions
    {
        public const string SectionName = "SisgesDatabase";

        public string ExpectedDatabase { get; init; } = "aval_cob";

        public int CommandTimeoutSeconds { get; init; } = 15;
    }
}
