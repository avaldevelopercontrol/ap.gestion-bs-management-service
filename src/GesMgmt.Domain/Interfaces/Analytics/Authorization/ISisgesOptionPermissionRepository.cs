using GesMgmt.Domain.Constants;

namespace GesMgmt.Domain.Interfaces.Analytics;

public interface ISisgesOptionPermissionRepository
{
    Task<bool> HasPermissionAsync(
        int userId,
        int? groupId,
        string optionCode,
        SisgesOptionPermission permission,
        CancellationToken cancellationToken);
}
