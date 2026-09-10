using GesMgmt.Application.Services.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

namespace GesMgmt.UnitTests.Analytics.Portfolio;

public sealed class PortfolioAdvisorPerformanceResponseMapperTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Map_UsesExplicitMissingDataLabel_WhenAdvisorNameIsBlank(string advisorName)
    {
        PortfolioAdvisorPerformanceDbRow[] rows =
        [
            CreateRow(advisorId: 42, advisorName: advisorName)
        ];

        var response = PortfolioAdvisorPerformanceResponseMapper.Map(rows);

        var advisor = Assert.Single(response.Advisors);
        Assert.Equal(42, advisor.AdvisorId);
        Assert.Equal("Sin nombre (ID 42)", advisor.AdvisorName);
    }

    [Fact]
    public void Map_UsesExplicitMissingDataLabel_WhenDatabaseReturnsNullAdvisorName()
    {
        PortfolioAdvisorPerformanceDbRow[] rows =
        [
            CreateRow(advisorId: 73, advisorName: null!)
        ];

        var response = PortfolioAdvisorPerformanceResponseMapper.Map(rows);

        Assert.Equal("Sin nombre (ID 73)", Assert.Single(response.Advisors).AdvisorName);
    }

    [Fact]
    public void Map_TrimsNames_AndNormalizesBlankOptionalSupervisorNameToNull()
    {
        PortfolioAdvisorPerformanceDbRow[] rows =
        [
            CreateRow(
                advisorId: 9,
                advisorName: "  PEREZ JUAN  ",
                periodSupervisorId: 7,
                periodSupervisorName: "  SUPERVISORA DEL PERIODO  ",
                currentSupervisorId: 3,
                currentSupervisorName: "   ")
        ];

        var advisor = Assert.Single(PortfolioAdvisorPerformanceResponseMapper.Map(rows).Advisors);

        Assert.Equal("PEREZ JUAN", advisor.AdvisorName);
        Assert.Equal(7, advisor.PeriodSupervisorId);
        Assert.Equal("SUPERVISORA DEL PERIODO", advisor.PeriodSupervisorName);
        Assert.Equal(3, advisor.CurrentSupervisorId);
        Assert.Null(advisor.CurrentSupervisorName);
    }

    private static PortfolioAdvisorPerformanceDbRow CreateRow(
        int advisorId,
        string advisorName,
        int? periodSupervisorId = null,
        string? periodSupervisorName = null,
        int? currentSupervisorId = null,
        string? currentSupervisorName = null)
    {
        return new PortfolioAdvisorPerformanceDbRow
        {
            AdvisorId = advisorId,
            AdvisorName = advisorName,
            PeriodSupervisorId = periodSupervisorId,
            PeriodSupervisorName = periodSupervisorName,
            CurrentSupervisorId = currentSupervisorId,
            CurrentSupervisorName = currentSupervisorName,
            DateFrom = new DateTime(2026, 9, 1),
            DateTo = new DateTime(2026, 9, 8),
            ManagementCount = 1,
            AttributableRecoveredAmount = 0m
        };
    }
}
