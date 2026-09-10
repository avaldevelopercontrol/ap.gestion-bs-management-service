namespace GesMgmt.Domain.Entities.Analytics;

public sealed record SisgesClientGroup
{
    public int GroupId { get; init; }
    public int ClientId { get; init; }
    public required string GroupName { get; init; }
}
