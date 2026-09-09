namespace GesMgmt.Infraestructure.Persistence.Analytics
{
    public sealed class SisgesDatabaseOptions
    {
        public const string SectionName = "SisgesDatabase";
        public const string ConnectionStringName = "AvalCobConnection";
        public const string DatabaseName = "aval_cob";

        public int CommandTimeoutSeconds { get; init; } = 15;
    }
}
