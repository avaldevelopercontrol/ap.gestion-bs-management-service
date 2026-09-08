using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public sealed class AnalyticsUserOptionQueryService(
    IAnalyticsUserOptionRepository repository) : IAnalyticsUserOptionQueryService
{
    public async Task<IReadOnlyList<AnalyticsUserOptionResponse>> GetAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        var options = await repository.GetUserOptionsAsync(
            userId,
            cancellationToken);

        return options
            .Select(option => new AnalyticsUserOptionResponse(
                option.OptionId,
                option.OptionCode,
                option.OptionName))
            .ToArray();
    }
}
