using GesMgmt.Application.Services.Analytics;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.UnitTests.Analytics.Authorization;

public sealed class AnalyticsAuthorizationServiceTests
{
    [Fact]
    public async Task CanAccessAdministrationAsync_UsesMaintainModuleAndCurrentGroup()
    {
        var repository = new RecordingPermissionRepository(result: true);
        var service = new AnalyticsAuthorizationService(repository);

        var result = await service.CanAccessAdministrationAsync(
            16068,
            156,
            SisgesOptionPermission.Edit,
            CancellationToken.None);

        Assert.True(result.Allowed);
        Assert.Equal(16068, repository.UserId);
        Assert.Equal(156, repository.GroupId);
        Assert.Equal(SisgesOptionCodes.MaintainModule, repository.OptionCode);
        Assert.Equal(SisgesOptionPermission.Edit, repository.Permission);
    }

    [Fact]
    public async Task CanAccessAdministrationAsync_DeniesWhenSisgesPermissionIsMissing()
    {
        var repository = new RecordingPermissionRepository(result: false);
        var service = new AnalyticsAuthorizationService(repository);

        var result = await service.CanAccessAdministrationAsync(
            16068,
            null,
            SisgesOptionPermission.Consult,
            CancellationToken.None);

        Assert.False(result.Allowed);
        Assert.Contains("Mantener módulo", result.Reason);
    }

    private sealed class RecordingPermissionRepository(bool result)
        : ISisgesOptionPermissionRepository
    {
        public int UserId { get; private set; }
        public int? GroupId { get; private set; }
        public string? OptionCode { get; private set; }
        public SisgesOptionPermission Permission { get; private set; }

        public Task<bool> HasPermissionAsync(
            int userId,
            int? groupId,
            string optionCode,
            SisgesOptionPermission permission,
            CancellationToken cancellationToken)
        {
            UserId = userId;
            GroupId = groupId;
            OptionCode = optionCode;
            Permission = permission;
            return Task.FromResult(result);
        }
    }
}
