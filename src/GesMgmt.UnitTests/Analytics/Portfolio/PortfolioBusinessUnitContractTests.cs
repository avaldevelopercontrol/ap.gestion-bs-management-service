using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

namespace GesMgmt.UnitTests.Analytics.Portfolio;

public sealed class PortfolioBusinessUnitContractTests
{
    [Fact]
    public void TryResolve_UsesCanonicalExplicitBusinessUnit()
    {
        var success = PortfolioBusinessUnitContract.TryResolve(
            " claro gobierno ",
            "CLARO ADMINISTRATIVO",
            ["CLARO ADMINISTRATIVO", "CLARO GOBIERNO"],
            out var selection,
            out var errors);

        Assert.True(success);
        Assert.Empty(errors);
        Assert.Equal("CLARO GOBIERNO", selection.SelectedBusinessUnit);
        Assert.False(selection.WasDefaulted);
        Assert.True(selection.HasMultipleBusinessUnits);
    }

    [Fact]
    public void TryResolve_OmittedSelectionUsesBackwardCompatibleDefault()
    {
        var success = PortfolioBusinessUnitContract.TryResolve(
            null,
            " claro administrativo ",
            ["CLARO GOBIERNO", "CLARO ADMINISTRATIVO"],
            out var selection,
            out var errors);

        Assert.True(success);
        Assert.Empty(errors);
        Assert.Equal("CLARO ADMINISTRATIVO", selection.SelectedBusinessUnit);
        Assert.True(selection.WasDefaulted);
        Assert.Equal(
            new[] { "CLARO ADMINISTRATIVO", "CLARO GOBIERNO" },
            selection.AvailableBusinessUnits.ToArray());
    }

    [Fact]
    public void TryResolve_RejectsBusinessUnitOutsideAvailableData()
    {
        var success = PortfolioBusinessUnitContract.TryResolve(
            "OTRA UNIDAD",
            "CLARO ADMINISTRATIVO",
            ["CLARO ADMINISTRATIVO", "CLARO GOBIERNO"],
            out var selection,
            out var errors);

        Assert.False(success);
        Assert.Null(selection.SelectedBusinessUnit);
        Assert.Contains("businessUnit", errors.Keys);
    }
}
