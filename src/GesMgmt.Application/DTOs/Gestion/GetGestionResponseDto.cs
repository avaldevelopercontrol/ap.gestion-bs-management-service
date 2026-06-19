
namespace GesMgmt.Application.DTOs.Gestion
{
    public class GetGestionResponseDto
    {
        public class GestionCarteraDeudorHistoricaResponseDto()
        {
            public int nId_DocxCobrarOpe { get; set; }
            public int nro { get; set; }
            public string? cliente { get; set; }
            public string? cartera { get; set; }
            public string? campanna { get; set; }
            public string? fecha { get; set; }
            public string? gestor { get; set; }
            public string? documento { get; set; }
            public string? operacion { get; set; }
            public string? resultado { get; set; }
            public string? comentario { get; set; }
        }

        public class GestionCarteraDeudorEstadoHistoricaResponseDto
        {
            public int nId_DocxCobrarOpe { get; set; }
            public int nro { get; set; }
            public string? cliente { get; set; }
            public string? cartera { get; set; }
            public string? campanna { get; set; }
            public string? fecha { get; set; }
            public string? gestor { get; set; }
            public string? documento { get; set; }
            public string? operacion { get; set; }
            public string? resultado { get; set; }
            public string? comentario { get; set; }
        }
    }
}