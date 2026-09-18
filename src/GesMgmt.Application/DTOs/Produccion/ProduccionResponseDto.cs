namespace GesMgmt.Application.DTOs.Produccion
{
    public class ProduccionResponseDto
    {

        public class GetProvinciasProduccionResponseDto
        {
            public int nId_Ubigeo { get; set; }
            public string? cNombre_Ubigeo { get; set; }
        }

        public class GetClientesProduccionActivosResponsetDto
        {
            public int nId_Cliente { get; set; }
            public string? cCli_Siglas { get; set; }
        }

        public class GetPerfilesActivosResponsetDto
        {
            public int nid_perfil { get; set; }
            public string? per_Nombre { get; set; }
        }

        public class GetProduccionResumenResponseDto
        {
            public string nombresUsu { get; set; } = string.Empty;
            public string clienteNom { get; set; } = string.Empty;
            public int minutosGes { get; set; }
            public int contactGes { get; set; }
            public int totalesGes { get; set; }
            public int contactGesProm { get; set; }
        }
    }
}