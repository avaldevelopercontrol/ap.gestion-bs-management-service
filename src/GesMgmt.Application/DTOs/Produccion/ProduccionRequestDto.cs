namespace GesMgmt.Application.DTOs.Produccion
{
    public class ProduccionRequestDto
    {
        public class GetProduccionResumenRequestDto
        {
            public int nId_Cliente { get; set; }
            public int nId_Perfil { get; set; }
            public int nId_Ubigeo { get; set; }
            public int nId_TipoLlamada { get; set; }
        }
    }
}