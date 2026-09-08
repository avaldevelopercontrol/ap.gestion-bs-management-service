namespace GesMgmt.Application.Interfaces.Analytics
{
    /// <summary>
    /// Expone la identidad del usuario actual a los casos de uso de Analytics sin
    /// acoplar la capa Application a un mecanismo concreto de autenticación.
    /// </summary>
    public interface IAnalyticsUserContext
    {
        bool TryGetUserId(out int userId);
    }
}
