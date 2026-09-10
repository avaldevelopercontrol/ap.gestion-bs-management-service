namespace GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

public sealed record PortfolioOperationResult<T>(
    T? Value,
    int? ErrorStatusCode,
    string? ErrorTitle,
    string? ErrorDetail,
    IReadOnlyDictionary<string, string[]>? ValidationErrors)
{
    public bool IsSuccess => ErrorStatusCode is null && ValidationErrors is null;

    public static PortfolioOperationResult<T> Success(T value) =>
        new(value, null, null, null, null);

    public static PortfolioOperationResult<T> Problem(
        int statusCode,
        string? title,
        string? detail) =>
        new(default, statusCode, title, detail, null);

    public static PortfolioOperationResult<T> Validation(
        IReadOnlyDictionary<string, string[]> errors) =>
        new(default, 400, "Solicitud inválida", "Uno o más parámetros no son válidos.", errors);

    public static PortfolioOperationResult<T> FromAccess(
        PortfolioControlCenterClientAccess access) =>
        Problem(
            access.ErrorStatusCode ?? 403,
            access.ErrorTitle ?? "Acceso Analytics denegado",
            access.ErrorDetail ?? "No se pudo validar el acceso a Portfolio Control Center.");
}
