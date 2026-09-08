namespace GesMgmt.Infraestructure.Persistence.Analytics
{
    internal interface IAnalyticsQueryExecutor
    {
        Task<T?> QuerySingleOrDefaultAsync<T>(
            string sql,
            object? parameters,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken);

        Task<T> QuerySingleAsync<T>(
            string sql,
            object? parameters,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<T>> QueryAsync<T>(
            string sql,
            object? parameters,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken);

        Task<AnalyticsTwoResultSets<T1, T2>> QueryTwoAsync<T1, T2>(
            string sql,
            object? parameters,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken);

        Task<AnalyticsFourResultSets<T1, T2, T3, T4>> QueryFourAsync<
            T1,
            T2,
            T3,
            T4>(
            string sql,
            object? parameters,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken);

        Task<AnalyticsFiveResultSets<T1, T2, T3, T4, T5>> QueryFiveAsync<
            T1,
            T2,
            T3,
            T4,
            T5>(
            string sql,
            object? parameters,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken);

        Task<AnalyticsSixResultSets<T1, T2, T3, T4, T5, T6>> QuerySixAsync<
            T1,
            T2,
            T3,
            T4,
            T5,
            T6>(
            string sql,
            object? parameters,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken);

        Task<int> ExecuteAsync(
            string sql,
            object? parameters,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken);

        Task ExecuteTransactionAsync(
            IReadOnlyCollection<AnalyticsDbCommand> commands,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken);
    }

    internal sealed record AnalyticsTwoResultSets<T1, T2>(
        IReadOnlyList<T1> First,
        IReadOnlyList<T2> Second);

    internal sealed record AnalyticsFourResultSets<T1, T2, T3, T4>(
        IReadOnlyList<T1> First,
        IReadOnlyList<T2> Second,
        IReadOnlyList<T3> Third,
        IReadOnlyList<T4> Fourth);

    internal sealed record AnalyticsFiveResultSets<T1, T2, T3, T4, T5>(
        IReadOnlyList<T1> First,
        IReadOnlyList<T2> Second,
        IReadOnlyList<T3> Third,
        IReadOnlyList<T4> Fourth,
        IReadOnlyList<T5> Fifth);

    internal sealed record AnalyticsSixResultSets<T1, T2, T3, T4, T5, T6>(
        IReadOnlyList<T1> First,
        IReadOnlyList<T2> Second,
        IReadOnlyList<T3> Third,
        IReadOnlyList<T4> Fourth,
        IReadOnlyList<T5> Fifth,
        IReadOnlyList<T6> Sixth);
}
