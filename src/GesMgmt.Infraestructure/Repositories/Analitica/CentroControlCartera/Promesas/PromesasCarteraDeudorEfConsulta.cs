using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analitica.CentroControlCartera;

internal static class PromesasCarteraDeudorEfConsulta
{
    public static async Task<IReadOnlyDictionary<long, string?>> ObtenerNombresAsync(
        AvalDbContext context,
        IEnumerable<long> idsDeudor,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(idsDeudor);

        var ids = idsDeudor
            .Where(id => id is > 0 and <= int.MaxValue)
            .Select(id => (int)id)
            .Distinct()
            .ToArray();

        if (ids.Length == 0)
        {
            return new Dictionary<long, string?>();
        }

        var rows = await context.av_PersDeudors
            .AsNoTracking()
            .Where(row => ids.Contains(row.nId_PersDeudor))
            .Select(row => new
            {
                row.nId_PersDeudor,
                row.cNomCompleto,
                row.cPers_Nombres,
                row.cPers_ApePat,
                row.cPers_ApeMat
            })
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(row => row.nId_PersDeudor)
            .ToDictionary(
                group => (long)group.Key,
                group => NormalizarNombre(
                    group.Select(row => row.cNomCompleto).FirstOrDefault(),
                    group.Select(row => row.cPers_Nombres).FirstOrDefault(),
                    group.Select(row => row.cPers_ApePat).FirstOrDefault(),
                    group.Select(row => row.cPers_ApeMat).FirstOrDefault()));
    }

    internal static string? NormalizarNombre(
        string? nombreCompleto,
        string? nombres,
        string? apellidoPaterno,
        string? apellidoMaterno)
    {
        if (!string.IsNullOrWhiteSpace(nombreCompleto))
        {
            return nombreCompleto.Trim();
        }

        var compuesto = string.Join(
            ' ',
            new[] { nombres, apellidoPaterno, apellidoMaterno }
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value!.Trim()));

        return string.IsNullOrWhiteSpace(compuesto)
            ? null
            : compuesto;
    }
}
