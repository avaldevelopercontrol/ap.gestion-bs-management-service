using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record AnalyticsUserAllowedClient(
    int ClientId,
    string Name);

public sealed record AnalyticsUserAllowedClientsResponse(
    int OptionId,
    IReadOnlyList<int> ClientIds,
    IReadOnlyList<AnalyticsUserAllowedClient> Clients)
{
    public static AnalyticsUserAllowedClientsResponse FromIds(
        int optionId,
        IReadOnlyList<int> clientIds) =>
        new(
            optionId,
            clientIds,
            Array.Empty<AnalyticsUserAllowedClient>());

    public static AnalyticsUserAllowedClientsResponse FromClients(
        int optionId,
        IReadOnlyList<AnalyticsAllowedClient> clients) =>
        new(
            optionId,
            clients.Select(client => client.CrmClientId).ToArray(),
            clients.Select(client => new AnalyticsUserAllowedClient(
                client.CrmClientId,
                client.Name)).ToArray());
}
