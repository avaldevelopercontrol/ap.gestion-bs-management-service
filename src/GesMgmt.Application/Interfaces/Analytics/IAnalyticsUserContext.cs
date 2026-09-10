namespace GesMgmt.Application.Interfaces.Analytics
{
    /// <summary>
    /// Expone la identidad SISGES y el grupo funcional actual a los casos de uso
    /// de Analytics sin acoplar Application al mecanismo HTTP/autenticación.
    /// </summary>
    public interface IAnalyticsUserContext
    {
        bool TryGetUserId(out int userId);
        bool TryGetGroupId(out int groupId);
    }
}
