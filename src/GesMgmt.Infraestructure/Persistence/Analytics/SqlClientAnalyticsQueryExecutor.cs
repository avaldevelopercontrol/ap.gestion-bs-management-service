using Dapper;

namespace GesMgmt.Infraestructure.Persistence.Analytics
{
    internal sealed class SqlClientAnalyticsQueryExecutor(
        IAnalyticsDbConnectionFactory connectionFactory,
        AnalyticsDatabaseTelemetry telemetry) : IAnalyticsQueryExecutor
    {
        public async Task<T?> QuerySingleOrDefaultAsync<T>(
            string sql,
            object? parameters,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken)
        {
            await using var connection =
                await connectionFactory.OpenConnectionAsync(cancellationToken);

            return await telemetry.ObserveAsync(
                "analytics",
                "query.single_or_default",
                () => connection.QuerySingleOrDefaultAsync<T>(
                    BuildCommand(
                        sql,
                        parameters,
                        commandTimeoutSeconds,
                        cancellationToken)));
        }

        public async Task<T> QuerySingleAsync<T>(
            string sql,
            object? parameters,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken)
        {
            await using var connection =
                await connectionFactory.OpenConnectionAsync(cancellationToken);

            return await telemetry.ObserveAsync(
                "analytics",
                "query.single",
                () => connection.QuerySingleAsync<T>(
                    BuildCommand(
                        sql,
                        parameters,
                        commandTimeoutSeconds,
                        cancellationToken)));
        }

        public async Task<IReadOnlyList<T>> QueryAsync<T>(
            string sql,
            object? parameters,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken)
        {
            await using var connection =
                await connectionFactory.OpenConnectionAsync(cancellationToken);

            var rows = await telemetry.ObserveAsync(
                "analytics",
                "query",
                () => connection.QueryAsync<T>(
                    BuildCommand(
                        sql,
                        parameters,
                        commandTimeoutSeconds,
                        cancellationToken)));

            return rows.AsList();
        }

        public async Task<AnalyticsTwoResultSets<T1, T2>> QueryTwoAsync<T1, T2>(
            string sql,
            object? parameters,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken)
        {
            await using var connection =
                await connectionFactory.OpenConnectionAsync(cancellationToken);

            return await telemetry.ObserveAsync(
                "analytics",
                "query.multiple.2",
                async () =>
                {
                    using var result = await connection.QueryMultipleAsync(
                        BuildCommand(
                            sql,
                            parameters,
                            commandTimeoutSeconds,
                            cancellationToken));

                    var first = (await result.ReadAsync<T1>()).AsList();
                    var second = (await result.ReadAsync<T2>()).AsList();

                    return new AnalyticsTwoResultSets<T1, T2>(first, second);
                });
        }

        public async Task<AnalyticsFourResultSets<T1, T2, T3, T4>> QueryFourAsync<
            T1,
            T2,
            T3,
            T4>(
            string sql,
            object? parameters,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken)
        {
            await using var connection =
                await connectionFactory.OpenConnectionAsync(cancellationToken);

            return await telemetry.ObserveAsync(
                "analytics",
                "query.multiple.4",
                async () =>
                {
                    using var result = await connection.QueryMultipleAsync(
                        BuildCommand(
                            sql,
                            parameters,
                            commandTimeoutSeconds,
                            cancellationToken));

                    var first = (await result.ReadAsync<T1>()).AsList();
                    var second = (await result.ReadAsync<T2>()).AsList();
                    var third = (await result.ReadAsync<T3>()).AsList();
                    var fourth = (await result.ReadAsync<T4>()).AsList();

                    return new AnalyticsFourResultSets<T1, T2, T3, T4>(
                        first,
                        second,
                        third,
                        fourth);
                });
        }

        public async Task<AnalyticsFiveResultSets<T1, T2, T3, T4, T5>> QueryFiveAsync<
            T1,
            T2,
            T3,
            T4,
            T5>(
            string sql,
            object? parameters,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken)
        {
            await using var connection =
                await connectionFactory.OpenConnectionAsync(cancellationToken);

            return await telemetry.ObserveAsync(
                "analytics",
                "query.multiple.5",
                async () =>
                {
                    using var result = await connection.QueryMultipleAsync(
                        BuildCommand(
                            sql,
                            parameters,
                            commandTimeoutSeconds,
                            cancellationToken));

                    var first = (await result.ReadAsync<T1>()).AsList();
                    var second = (await result.ReadAsync<T2>()).AsList();
                    var third = (await result.ReadAsync<T3>()).AsList();
                    var fourth = (await result.ReadAsync<T4>()).AsList();
                    var fifth = (await result.ReadAsync<T5>()).AsList();

                    return new AnalyticsFiveResultSets<T1, T2, T3, T4, T5>(
                        first,
                        second,
                        third,
                        fourth,
                        fifth);
                });
        }

        public async Task<AnalyticsSixResultSets<T1, T2, T3, T4, T5, T6>> QuerySixAsync<
            T1,
            T2,
            T3,
            T4,
            T5,
            T6>(
            string sql,
            object? parameters,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken)
        {
            await using var connection =
                await connectionFactory.OpenConnectionAsync(cancellationToken);

            return await telemetry.ObserveAsync(
                "analytics",
                "query.multiple.6",
                async () =>
                {
                    using var result = await connection.QueryMultipleAsync(
                        BuildCommand(
                            sql,
                            parameters,
                            commandTimeoutSeconds,
                            cancellationToken));

                    var first = (await result.ReadAsync<T1>()).AsList();
                    var second = (await result.ReadAsync<T2>()).AsList();
                    var third = (await result.ReadAsync<T3>()).AsList();
                    var fourth = (await result.ReadAsync<T4>()).AsList();
                    var fifth = (await result.ReadAsync<T5>()).AsList();
                    var sixth = (await result.ReadAsync<T6>()).AsList();

                    return new AnalyticsSixResultSets<T1, T2, T3, T4, T5, T6>(
                        first,
                        second,
                        third,
                        fourth,
                        fifth,
                        sixth);
                });
        }

        public async Task<int> ExecuteAsync(
            string sql,
            object? parameters,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken)
        {
            await using var connection =
                await connectionFactory.OpenConnectionAsync(cancellationToken);

            return await telemetry.ObserveAsync(
                "analytics",
                "execute",
                () => connection.ExecuteAsync(
                    BuildCommand(
                        sql,
                        parameters,
                        commandTimeoutSeconds,
                        cancellationToken)));
        }

        public async Task ExecuteTransactionAsync(
            IReadOnlyCollection<AnalyticsDbCommand> commands,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken)
        {
            if (commands.Count == 0)
            {
                return;
            }

            await using var connection =
                await connectionFactory.OpenConnectionAsync(cancellationToken);

            await telemetry.ObserveAsync(
                "analytics",
                "transaction",
                async () =>
                {
                    await using var transaction =
                        await connection.BeginTransactionAsync(cancellationToken);

                    try
                    {
                        foreach (var command in commands)
                        {
                            await connection.ExecuteAsync(
                                new CommandDefinition(
                                    command.Sql,
                                    command.Parameters,
                                    transaction,
                                    commandTimeout: commandTimeoutSeconds,
                                    cancellationToken: cancellationToken));
                        }

                        await transaction.CommitAsync(cancellationToken);
                    }
                    catch
                    {
                        try
                        {
                            await transaction.RollbackAsync(CancellationToken.None);
                        }
                        catch
                        {
                            // Se conserva la excepción original de base de datos.
                        }

                        throw;
                    }
                });
        }

        private static CommandDefinition BuildCommand(
            string sql,
            object? parameters,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken) =>
            new(
                sql,
                parameters,
                commandTimeout: commandTimeoutSeconds,
                cancellationToken: cancellationToken);
    }
}
