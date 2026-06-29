
namespace GesMgmt.Application.DTOs.Direccion
{
    public class DireccionRequestDto
    {
        public class GetDireccionesRequestDto
        {
            public int nId_Cliente { get; set; } //ID_CLIENTE
            public int nId_Persdeudor { get; set; } //ID_DEUDOR

            // 🔹 PAGINACIÓN
            public int PageNumber { get; set; } = 1;
            private int _pageSize = 10;

            public int PageSize
            {
                get => _pageSize;
                set => _pageSize = value > 1000 ? 1000 : value; // Máximo 50
            }
        }

        public class CreateDireccionRequestDto
        {
            public int nId_PersDirecc { get; set; }
            public int? nId_PersDeudor { get; set; }
            public string? cDirecc_Nomb { get; set; }
            public int? nId_PersRefUbi { get; set; }
            public string? cDirecc_Coment { get; set; }
            public bool? bEstado { get; set; }
            public bool? bOrigen_Base { get; set; }
            public string? cTipoCoDeudor { get; set; }
            public DateTime? dFec_Actualizacion { get; set; }
            public int? nId_Cliente { get; set; }
            public int? nid_CalifDirecc { get; set; }
            public int? nid_usuarioUpd { get; set; }
            public int? nId_Departamento { get; set; }
            public int? nId_Provincia { get; set; }
            public int? nId_Distrito { get; set; }
        }

        public class EditDireccionRequestDto
        {
            public int nId_PersDirecc { get; set; }
            public int? nId_PersDeudor { get; set; }
            public string? cDirecc_Nomb { get; set; }
            public int? nId_PersRefUbi { get; set; }
            public string? cDirecc_Coment { get; set; }
            public bool? bEstado { get; set; }
            public bool? bOrigen_Base { get; set; }
            public string? cTipoCoDeudor { get; set; }
            public DateTime? dFec_Actualizacion { get; set; }
            public int? nId_Cliente { get; set; }
            public int? nid_CalifDirecc { get; set; }
            public int? nid_usuarioUpd { get; set; }
            public int? nId_Departamento { get; set; }
            public int? nId_Provincia { get; set; }
            public int? nId_Distrito { get; set; }
        }
    }
}