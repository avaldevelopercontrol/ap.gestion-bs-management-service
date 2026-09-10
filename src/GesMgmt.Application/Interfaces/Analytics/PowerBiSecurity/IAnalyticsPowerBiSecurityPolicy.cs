using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.Interfaces.Analytics;

public interface IAnalyticsPowerBiSecurityPolicy
{
    bool AllowPublishToWeb { get; }
}
