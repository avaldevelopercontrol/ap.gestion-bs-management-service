using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal static class PortfolioOverduePromisesSql
{
    public const string Query = """
        WITH /* pcc:promises-overdue:summary */ FilteredPromises AS
        (
            SELECT
                p.promise_due_date AS DueDate,
                CONVERT(DECIMAL(19,4), p.promise_amount) AS PromiseAmount,
                CONVERT(
                    DECIMAL(19,4),
                    CASE
                        WHEN p.promise_amount > p.paid_amount
                            THEN p.promise_amount - p.paid_amount
                        ELSE 0
                    END
                ) AS OutstandingAmount,
                p.advisor_key AS AdvisorId,
                NULLIF(LTRIM(RTRIM(p.advisor_name)), '') AS AdvisorName,
                p.supervisor_key AS SupervisorId,
                NULLIF(LTRIM(RTRIM(p.supervisor_name)), '') AS SupervisorName,
                COALESCE(
                    MAX(CONVERT(DATE, p.loaded_at)) OVER (),
                    CONVERT(DATE, SYSUTCDATETIME())
                ) AS AsOfDate,
                p.loaded_at AS UpdatedAtUtc
            FROM analytics.v_supervisor_promise_operational AS p
            WHERE p.client_key = @ClientKey
              AND p.campaign_key = @CampaignKey
              AND (@SubPortfolioId IS NULL OR p.portfolio_key = @SubPortfolioId)
              AND (
                  @BusinessUnit IS NULL
                  OR EXISTS
                  (
                      SELECT 1
                      FROM analytics.dim_portfolio AS business_unit_portfolio
                      WHERE business_unit_portfolio.portfolio_key = p.portfolio_key
                        AND business_unit_portfolio.source_business_unit = @BusinessUnit
                  )
              )
              AND p.is_valid_promise = 1
              AND p.status_code = 'BROKEN'
        ),
        OverdueBase AS
        (
            SELECT
                DueDate,
                PromiseAmount,
                OutstandingAmount,
                AdvisorId,
                AdvisorName,
                SupervisorId,
                SupervisorName,
                CASE
                    WHEN DueDate IS NULL THEN 'unclassified'
                    WHEN DATEDIFF(DAY, DueDate, AsOfDate) BETWEEN 1 AND 3 THEN '1-3'
                    WHEN DATEDIFF(DAY, DueDate, AsOfDate) BETWEEN 4 AND 7 THEN '4-7'
                    ELSE '8-plus'
                END AS AgingKey,
                AsOfDate,
                UpdatedAtUtc
            FROM FilteredPromises
        )
        SELECT
            CASE
                WHEN GROUPING(AgingKey) = 0 THEN 'aging'
                WHEN GROUPING(AdvisorId) = 0 THEN 'advisor'
                WHEN GROUPING(SupervisorId) = 0 THEN 'supervisor'
                ELSE 'summary'
            END AS RowType,
            CASE WHEN GROUPING(AgingKey) = 0 THEN AgingKey END AS AgingKey,
            CASE WHEN GROUPING(AdvisorId) = 0 THEN AdvisorId END AS AdvisorId,
            CASE WHEN GROUPING(AdvisorId) = 0 THEN MAX(AdvisorName) END AS AdvisorName,
            CASE WHEN GROUPING(SupervisorId) = 0 THEN SupervisorId END AS SupervisorId,
            CASE WHEN GROUPING(SupervisorId) = 0 THEN MAX(SupervisorName) END AS SupervisorName,
            COUNT_BIG(*) AS PromiseCount,
            CONVERT(DECIMAL(19,4), COALESCE(SUM(PromiseAmount), 0)) AS PromiseAmount,
            CONVERT(DECIMAL(19,4), COALESCE(SUM(OutstandingAmount), 0)) AS OutstandingAmount,
            MAX(AsOfDate) AS AsOfDate,
            MAX(UpdatedAtUtc) AS UpdatedAtUtc
        FROM OverdueBase
        GROUP BY GROUPING SETS
        (
            (),
            (AgingKey),
            (AdvisorId),
            (SupervisorId)
        )

        UNION ALL

        SELECT
            'filtered' AS RowType,
            NULL AS AgingKey,
            NULL AS AdvisorId,
            NULL AS AdvisorName,
            NULL AS SupervisorId,
            NULL AS SupervisorName,
            COUNT_BIG(*) AS PromiseCount,
            CONVERT(DECIMAL(19,4), COALESCE(SUM(PromiseAmount), 0)) AS PromiseAmount,
            CONVERT(DECIMAL(19,4), COALESCE(SUM(OutstandingAmount), 0)) AS OutstandingAmount,
            MAX(AsOfDate) AS AsOfDate,
            MAX(UpdatedAtUtc) AS UpdatedAtUtc
        FROM OverdueBase
        WHERE @Aging IS NULL OR AgingKey = @Aging;

        WITH /* pcc:promises-overdue:items */ FilteredPromises AS
        (
            SELECT
                p.promise_fact_key AS PromiseId,
                p.source_debtor_id AS DebtorId,
                p.promise_due_date AS DueDate,
                CONVERT(DECIMAL(19,4), p.promise_amount) AS PromiseAmount,
                CONVERT(DECIMAL(19,4), p.paid_amount) AS PaidAmount,
                CONVERT(
                    DECIMAL(19,4),
                    CASE
                        WHEN p.promise_amount > p.paid_amount
                            THEN p.promise_amount - p.paid_amount
                        ELSE 0
                    END
                ) AS OutstandingAmount,
                p.advisor_key AS AdvisorId,
                NULLIF(LTRIM(RTRIM(p.advisor_name)), '') AS AdvisorName,
                p.supervisor_key AS SupervisorId,
                NULLIF(LTRIM(RTRIM(p.supervisor_name)), '') AS SupervisorName,
                COALESCE(
                    MAX(CONVERT(DATE, p.loaded_at)) OVER (),
                    CONVERT(DATE, SYSUTCDATETIME())
                ) AS AsOfDate,
                p.loaded_at AS UpdatedAtUtc
            FROM analytics.v_supervisor_promise_operational AS p
            WHERE p.client_key = @ClientKey
              AND p.campaign_key = @CampaignKey
              AND (@SubPortfolioId IS NULL OR p.portfolio_key = @SubPortfolioId)
              AND (
                  @BusinessUnit IS NULL
                  OR EXISTS
                  (
                      SELECT 1
                      FROM analytics.dim_portfolio AS business_unit_portfolio
                      WHERE business_unit_portfolio.portfolio_key = p.portfolio_key
                        AND business_unit_portfolio.source_business_unit = @BusinessUnit
                  )
              )
              AND p.is_valid_promise = 1
              AND p.status_code = 'BROKEN'
        ),
        OverdueItems AS
        (
            SELECT
                PromiseId,
                DebtorId,
                DueDate,
                CASE
                    WHEN DueDate IS NULL THEN NULL
                    WHEN DATEDIFF(DAY, DueDate, AsOfDate) < 1 THEN 1
                    ELSE DATEDIFF(DAY, DueDate, AsOfDate)
                END AS OverdueDays,
                PromiseAmount,
                PaidAmount,
                OutstandingAmount,
                AdvisorId,
                AdvisorName,
                SupervisorId,
                SupervisorName,
                CASE
                    WHEN DueDate IS NULL THEN 'unclassified'
                    WHEN DATEDIFF(DAY, DueDate, AsOfDate) BETWEEN 1 AND 3 THEN '1-3'
                    WHEN DATEDIFF(DAY, DueDate, AsOfDate) BETWEEN 4 AND 7 THEN '4-7'
                    ELSE '8-plus'
                END AS AgingKey,
                AsOfDate,
                UpdatedAtUtc
            FROM FilteredPromises
        )
        SELECT
            PromiseId,
            DebtorId,
            DueDate,
            OverdueDays,
            PromiseAmount,
            PaidAmount,
            OutstandingAmount,
            AdvisorId,
            AdvisorName,
            SupervisorId,
            SupervisorName,
            AgingKey,
            AsOfDate,
            UpdatedAtUtc
        FROM OverdueItems
        WHERE @Aging IS NULL OR AgingKey = @Aging
        ORDER BY
            CASE WHEN @SortBy = 'debtorId' AND @SortDirection = 'asc' THEN DebtorId END ASC,
            CASE WHEN @SortBy = 'debtorId' AND @SortDirection = 'desc' THEN DebtorId END DESC,
            CASE WHEN @SortBy = 'dueDate' AND @SortDirection = 'asc' THEN DueDate END ASC,
            CASE WHEN @SortBy = 'dueDate' AND @SortDirection = 'desc' THEN DueDate END DESC,
            CASE WHEN @SortBy = 'overdueDays' AND @SortDirection = 'asc' THEN OverdueDays END ASC,
            CASE WHEN @SortBy = 'overdueDays' AND @SortDirection = 'desc' THEN OverdueDays END DESC,
            CASE WHEN @SortBy = 'promiseAmount' AND @SortDirection = 'asc' THEN PromiseAmount END ASC,
            CASE WHEN @SortBy = 'promiseAmount' AND @SortDirection = 'desc' THEN PromiseAmount END DESC,
            CASE WHEN @SortBy = 'paidAmount' AND @SortDirection = 'asc' THEN PaidAmount END ASC,
            CASE WHEN @SortBy = 'paidAmount' AND @SortDirection = 'desc' THEN PaidAmount END DESC,
            CASE WHEN @SortBy = 'outstandingAmount' AND @SortDirection = 'asc' THEN OutstandingAmount END ASC,
            CASE WHEN @SortBy = 'outstandingAmount' AND @SortDirection = 'desc' THEN OutstandingAmount END DESC,
            CASE WHEN @SortBy = 'advisorName' AND @SortDirection = 'asc' THEN AdvisorName END ASC,
            CASE WHEN @SortBy = 'advisorName' AND @SortDirection = 'desc' THEN AdvisorName END DESC,
            CASE WHEN @SortBy = 'supervisorName' AND @SortDirection = 'asc' THEN SupervisorName END ASC,
            CASE WHEN @SortBy = 'supervisorName' AND @SortDirection = 'desc' THEN SupervisorName END DESC,
            PromiseId ASC
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY;
        """;
}
