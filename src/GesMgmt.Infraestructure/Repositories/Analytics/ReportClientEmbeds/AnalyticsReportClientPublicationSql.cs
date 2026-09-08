using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

public static class AnalyticsReportClientPublicationSql
{
    public const string ApplyPatch = """
        DECLARE @Publications TABLE
        (
            crm_client_id       INT NOT NULL,
            report_client_value VARCHAR(150) NOT NULL,
            group_ids_json      NVARCHAR(MAX) NULL,
            embed_url           VARCHAR(2048) NULL,
            update_groups       BIT NOT NULL,

            PRIMARY KEY
            (
                crm_client_id,
                report_client_value
            )
        );

        INSERT INTO @Publications
        (
            crm_client_id,
            report_client_value,
            group_ids_json,
            embed_url,
            update_groups
        )
        SELECT
            source.ClientId,
            CONVERT(VARCHAR(150), LTRIM(RTRIM(source.Name))),
            source.GroupIds,
            CASE
                WHEN source.EmbedUrl IS NULL THEN NULL
                ELSE CONVERT(VARCHAR(2048), LTRIM(RTRIM(source.EmbedUrl)))
            END,
            CASE WHEN source.GroupIds IS NULL THEN 0 ELSE 1 END
        FROM OPENJSON(@PublicationsJson)
        WITH
        (
            ClientId INT '$.ClientId',
            Name NVARCHAR(150) '$.Name',
            GroupIds NVARCHAR(MAX) '$.GroupIds' AS JSON,
            EmbedUrl NVARCHAR(2048) '$.EmbedUrl'
        ) AS source;

        IF @HasGroupUpdates = 1
        BEGIN
            DECLARE @Groups TABLE
            (
                crm_client_id       INT NOT NULL,
                report_client_value VARCHAR(150) NOT NULL,
                sisges_group_id     INT NOT NULL,

                PRIMARY KEY
                (
                    crm_client_id,
                    report_client_value,
                    sisges_group_id
                )
            );

            INSERT INTO @Groups
            (
                crm_client_id,
                report_client_value,
                sisges_group_id
            )
            SELECT DISTINCT
                publication.crm_client_id,
                publication.report_client_value,
                TRY_CONVERT(INT, group_source.[value])
            FROM @Publications AS publication
            CROSS APPLY OPENJSON(COALESCE(publication.group_ids_json, N'[]')) AS group_source
            WHERE publication.update_groups = 1
              AND TRY_CONVERT(INT, group_source.[value]) > 0;

            UPDATE target
            SET
                target.is_active = 0,
                target.updated_by = @UpdatedBy,
                target.updated_at = SYSUTCDATETIME()
            FROM analytics_access.power_bi_report_client_scope AS target
            INNER JOIN @Publications AS source
                ON source.crm_client_id = target.crm_client_id
               AND source.report_client_value = target.report_client_value
            WHERE target.option_id = @OptionId
              AND source.update_groups = 1
              AND target.is_active = 1;

            UPDATE target
            SET
                target.is_active = 1,
                target.updated_by = @UpdatedBy,
                target.updated_at = SYSUTCDATETIME()
            FROM analytics_access.power_bi_report_client_scope AS target
            INNER JOIN @Groups AS source
                ON source.crm_client_id = target.crm_client_id
               AND source.report_client_value = target.report_client_value
               AND source.sisges_group_id = target.sisges_group_id
            WHERE target.option_id = @OptionId;

            INSERT INTO analytics_access.power_bi_report_client_scope
            (
                option_id,
                crm_client_id,
                report_client_value,
                sisges_group_id,
                is_active,
                created_by,
                updated_by
            )
            SELECT
                @OptionId,
                source.crm_client_id,
                source.report_client_value,
                source.sisges_group_id,
                1,
                @UpdatedBy,
                @UpdatedBy
            FROM @Groups AS source
            WHERE NOT EXISTS
            (
                SELECT 1
                FROM analytics_access.power_bi_report_client_scope AS target
                    WITH (UPDLOCK, HOLDLOCK)
                WHERE target.option_id = @OptionId
                  AND target.crm_client_id = source.crm_client_id
                  AND target.report_client_value = source.report_client_value
                  AND target.sisges_group_id = source.sisges_group_id
            );
        END;

        UPDATE target
        SET
            target.is_active = 0,
            target.updated_by = @UpdatedBy,
            target.updated_at = SYSUTCDATETIME()
        FROM analytics_access.power_bi_report_client_embed AS target
        INNER JOIN @Publications AS source
            ON source.crm_client_id = target.crm_client_id
           AND source.report_client_value = target.report_client_value
        WHERE target.option_id = @OptionId
          AND source.embed_url IS NULL
          AND target.is_active = 1;

        UPDATE target
        SET
            target.embed_url = source.embed_url,
            target.is_active = 1,
            target.updated_by = @UpdatedBy,
            target.updated_at = SYSUTCDATETIME()
        FROM analytics_access.power_bi_report_client_embed AS target
        INNER JOIN @Publications AS source
            ON source.crm_client_id = target.crm_client_id
           AND source.report_client_value = target.report_client_value
        WHERE target.option_id = @OptionId
          AND source.embed_url IS NOT NULL;

        INSERT INTO analytics_access.power_bi_report_client_embed
        (
            option_id,
            crm_client_id,
            report_client_value,
            embed_url,
            is_active,
            created_by,
            updated_by
        )
        SELECT
            @OptionId,
            source.crm_client_id,
            source.report_client_value,
            source.embed_url,
            1,
            @UpdatedBy,
            @UpdatedBy
        FROM @Publications AS source
        WHERE source.embed_url IS NOT NULL
          AND NOT EXISTS
          (
              SELECT 1
              FROM analytics_access.power_bi_report_client_embed AS target
                  WITH (UPDLOCK, HOLDLOCK)
              WHERE target.option_id = @OptionId
                AND target.crm_client_id = source.crm_client_id
                AND target.report_client_value = source.report_client_value
          );
        """;
}
