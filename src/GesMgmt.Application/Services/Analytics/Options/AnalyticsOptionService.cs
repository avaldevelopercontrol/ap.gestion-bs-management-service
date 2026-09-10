using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public sealed class AnalyticsOptionService(
    IAnalyticsOptionConfigRepository repository) : IAnalyticsOptionService
{
    public async Task<IReadOnlyList<AnalyticsOptionConfigResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var options = await repository.GetAllAsync(cancellationToken);

        return options
            .Select(option => new AnalyticsOptionConfigResponse(
                option.OptionId,
                option.OptionCode,
                option.OptionName))
            .ToArray();
    }

    public Task<bool> ExistsAsync(
        int optionId,
        CancellationToken cancellationToken) =>
        repository.ExistsAsync(optionId, cancellationToken);

    public Task<bool> IsActiveAsync(
        int optionId,
        CancellationToken cancellationToken) =>
        repository.IsActiveAsync(optionId, cancellationToken);

    public Task UpsertAsync(
        int optionId,
        string optionCode,
        string optionName,
        bool isActive,
        int? userId,
        CancellationToken cancellationToken) =>
        repository.UpsertAsync(
            optionId,
            optionCode,
            optionName,
            isActive,
            userId,
            cancellationToken);
}
