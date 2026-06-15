namespace GesMgmt.Domain.Entities
{
    public class av_Ubigeo
    {
        public int nId_Ubigeo { get; set; }
        public int? nNivel_Id { get; set; }
        public int? nId_Pais { get; set; }
        public int? nId_Region { get; set; }
        public int? nId_Departamento { get; set; }
        public int? nId_Provincia { get; set; }
        public int? nId_Distrito { get; set; }
        public int? nId_Locacion { get; set; }
        public int? nId_Area { get; set; }
        public string? cNombre_Ubigeo { get; set; }
        public string? cSigla_Ubigeo { get; set; }
        public string? cCod_Postal { get; set; }
        public bool? bEstado { get; set; }
        public string? cCod_bloque { get; set; }
    }
}