namespace GesMgmt.Infraestructure.Persistence.Analytics
{
    internal interface ISisgesQueryExecutor
    {
        Task<IReadOnlyList<T>> QueryAsync<T>(
            string sql,
            object? parameters,
            CancellationToken cancellationToken);
    }
}
