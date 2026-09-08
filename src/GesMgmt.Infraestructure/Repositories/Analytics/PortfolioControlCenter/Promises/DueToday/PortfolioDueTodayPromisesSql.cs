using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal static class PortfolioDueTodayPromisesSql
{
    public const string Query = """
        WITH /* pcc:promises-due-today:summary */ DueTodayBase AS
        (
            SELECT
                p.promise_due_date AS DueDate,
                CONVERT(DECIMAL(19,4), COALESCE(p.promise_amount, 0)) AS PromiseAmount,
                CONVERT(DECIMAL(19,4), COALESCE(p.paid_amount, 0)) AS PaidAmount,
                CONVERT(
                    DECIMAL(19,4),
                    CASE
                        WHEN COALESCE(p.promise_amount, 0) > COALESCE(p.paid_amount, 0)
                            THEN COALESCE(p.promise_amount, 0) - COALESCE(p.paid_amount, 0)
                        ELSE 0
                    END
                ) AS OutstandingAmount,
                CASE
                    WHEN COALESCE(p.paid_amount, 0) <= 0 THEN 'pending'
                    WHEN COALESCE(p.paid_amount, 0) < COALESCE(p.promise_amount, 0) THEN 'partial'
                    ELSE 'covered'
                END AS StatusKey,
                p.loaded_at AS UpdatedAtUtc
            FROM analytics.v_promise_operational AS p
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
              AND p.is_due_today = 1
        )
        SELECT
            CASE WHEN GROUPING(StatusKey) = 1 THEN NULL ELSE StatusKey END AS StatusKey,
            COUNT_BIG(*) AS PromiseCount,
            CONVERT(DECIMAL(19,4), COALESCE(SUM(PromiseAmount), 0)) AS PromiseAmount,
            CONVERT(DECIMAL(19,4), COALESCE(SUM(PaidAmount), 0)) AS PaidAmount,
            CONVERT(DECIMAL(19,4), COALESCE(SUM(OutstandingAmount), 0)) AS OutstandingAmount,
            MAX(DueDate) AS AsOfDate,
            MAX(UpdatedAtUtc) AS UpdatedAtUtc
        FROM DueTodayBase
        GROUP BY GROUPING SETS
        (
            (),
            (StatusKey)
        )

        UNION ALL

        SELECT
            '__filtered__' AS StatusKey,
            COUNT_BIG(*) AS PromiseCount,
            CONVERT(DECIMAL(19,4), COALESCE(SUM(PromiseAmount), 0)) AS PromiseAmount,
            CONVERT(DECIMAL(19,4), COALESCE(SUM(PaidAmount), 0)) AS PaidAmount,
            CONVERT(DECIMAL(19,4), COALESCE(SUM(OutstandingAmount), 0)) AS OutstandingAmount,
            MAX(DueDate) AS AsOfDate,
            MAX(UpdatedAtUtc) AS UpdatedAtUtc
        FROM DueTodayBase
        WHERE @Status IS NULL OR StatusKey = @Status;

        WITH /* pcc:promises-due-today:items */ DueTodayItems AS
        (
            SELECT
                p.promise_fact_key AS PromiseId,
                p.source_debtor_id AS DebtorId,
                p.promise_due_date AS DueDate,
                CONVERT(DECIMAL(19,4), COALESCE(p.promise_amount, 0)) AS PromiseAmount,
                CONVERT(DECIMAL(19,4), COALESCE(p.paid_amount, 0)) AS PaidAmount,
                CONVERT(
                    DECIMAL(19,4),
                    CASE
                        WHEN COALESCE(p.promise_amount, 0) > COALESCE(p.paid_amount, 0)
                            THEN COALESCE(p.promise_amount, 0) - COALESCE(p.paid_amount, 0)
                        ELSE 0
                    END
                ) AS OutstandingAmount,
                p.last_payment_date AS LastPaymentDate,
                CASE
                    WHEN COALESCE(p.paid_amount, 0) <= 0 THEN 'pending'
                    WHEN COALESCE(p.paid_amount, 0) < COALESCE(p.promise_amount, 0) THEN 'partial'
                    ELSE 'covered'
                END AS StatusKey,
                CASE
                    WHEN COALESCE(p.paid_amount, 0) <= 0 THEN 'Pendiente'
                    WHEN COALESCE(p.paid_amount, 0) < COALESCE(p.promise_amount, 0) THEN 'Pago parcial'
                    ELSE 'Cubierta'
                END AS StatusLabel,
                p.advisor_key AS AdvisorId,
                attribution.AdvisorName,
                attribution.SupervisorId,
                attribution.SupervisorName,
                p.loaded_at AS UpdatedAtUtc
            FROM analytics.v_promise_operational AS p
            OUTER APPLY
            (
                SELECT TOP (1)
                    NULLIF(LTRIM(RTRIM(sp.advisor_name)), '') AS AdvisorName,
                    sp.supervisor_key AS SupervisorId,
                    NULLIF(LTRIM(RTRIM(sp.supervisor_name)), '') AS SupervisorName
                FROM analytics.v_supervisor_promise_operational AS sp
                WHERE sp.client_key = p.client_key
                  AND sp.campaign_key = p.campaign_key
                  AND sp.promise_fact_key = p.promise_fact_key
                ORDER BY
                    CASE WHEN sp.supervisor_key IS NULL THEN 1 ELSE 0 END,
                    sp.supervisor_advisor_key DESC,
                    sp.loaded_at DESC
            ) AS attribution
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
              AND p.is_due_today = 1
        )
        SELECT
            PromiseId,
            DebtorId,
            DueDate,
            PromiseAmount,
            PaidAmount,
            OutstandingAmount,
            LastPaymentDate,
            StatusKey,
            AdvisorId,
            AdvisorName,
            SupervisorId,
            SupervisorName,
            UpdatedAtUtc
        FROM DueTodayItems
        WHERE @Status IS NULL OR StatusKey = @Status
        ORDER BY
            CASE WHEN @SortBy = 'debtorId' AND @SortDirection = 'asc' THEN DebtorId END ASC,
            CASE WHEN @SortBy = 'debtorId' AND @SortDirection = 'desc' THEN DebtorId END DESC,
            CASE WHEN @SortBy = 'promiseAmount' AND @SortDirection = 'asc' THEN PromiseAmount END ASC,
            CASE WHEN @SortBy = 'promiseAmount' AND @SortDirection = 'desc' THEN PromiseAmount END DESC,
            CASE WHEN @SortBy = 'paidAmount' AND @SortDirection = 'asc' THEN PaidAmount END ASC,
            CASE WHEN @SortBy = 'paidAmount' AND @SortDirection = 'desc' THEN PaidAmount END DESC,
            CASE WHEN @SortBy = 'outstandingAmount' AND @SortDirection = 'asc' THEN OutstandingAmount END ASC,
            CASE WHEN @SortBy = 'outstandingAmount' AND @SortDirection = 'desc' THEN OutstandingAmount END DESC,
            CASE WHEN @SortBy = 'statusLabel' AND @SortDirection = 'asc' THEN StatusLabel END ASC,
            CASE WHEN @SortBy = 'statusLabel' AND @SortDirection = 'desc' THEN StatusLabel END DESC,
            CASE WHEN @SortBy = 'lastPaymentDate' AND @SortDirection = 'asc' THEN LastPaymentDate END ASC,
            CASE WHEN @SortBy = 'lastPaymentDate' AND @SortDirection = 'desc' THEN LastPaymentDate END DESC,
            CASE WHEN @SortBy = 'advisorName' AND @SortDirection = 'asc' THEN AdvisorName END ASC,
            CASE WHEN @SortBy = 'advisorName' AND @SortDirection = 'desc' THEN AdvisorName END DESC,
            CASE WHEN @SortBy = 'supervisorName' AND @SortDirection = 'asc' THEN SupervisorName END ASC,
            CASE WHEN @SortBy = 'supervisorName' AND @SortDirection = 'desc' THEN SupervisorName END DESC,
            PromiseId ASC
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY;
        """;
}
