using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

public sealed record PortfolioPromisesPagination(
    int Page,
    int PageSize,
    long TotalItems,
    long TotalPages,
    bool HasPreviousPage,
    bool HasNextPage)
{
    public static PortfolioPromisesPagination Create(
        int page,
        int pageSize,
        long totalItems)
    {
        var totalPages = totalItems == 0
            ? 0
            : (totalItems / pageSize) + (totalItems % pageSize == 0 ? 0 : 1);

        return new PortfolioPromisesPagination(
            page,
            pageSize,
            totalItems,
            totalPages,
            totalPages > 0 && page > 1,
            page < totalPages);
    }
}
public static class PortfolioPromisesPaging
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 100;
    public const int MaxPageSize = 200;

    public static void Parse(
        string? page,
        string? pageSize,
        IDictionary<string, string[]> errors,
        out int parsedPage,
        out int parsedPageSize)
    {
        parsedPage = ParsePositiveInt(
            page,
            DefaultPage,
            "page",
            "page debe ser un entero positivo.",
            errors);

        parsedPageSize = ParsePositiveInt(
            pageSize,
            DefaultPageSize,
            "pageSize",
            $"pageSize debe ser un entero entre 1 y {MaxPageSize}.",
            errors);

        if (parsedPageSize > MaxPageSize)
        {
            errors["pageSize"] =
            [$"pageSize debe ser un entero entre 1 y {MaxPageSize}."];
        }
    }

    private static int ParsePositiveInt(
        string? value,
        int defaultValue,
        string field,
        string errorMessage,
        IDictionary<string, string[]> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        if (int.TryParse(
                value.Trim(),
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var parsed)
            && parsed > 0)
        {
            return parsed;
        }

        errors[field] = [errorMessage];
        return defaultValue;
    }
}
