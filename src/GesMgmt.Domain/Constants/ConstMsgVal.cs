using System.Threading;

namespace GesMgmt.Domain.Constants
{
    // Constantes de Mensajes de Validacion
    public class ConstMsgVal
    {
        public const string TELEFONO_REQUERIDO = "001";
        public const string TELEFONO_MENOR_LONGITUD = "007";
        public const string TELEFONO_MAYOR_LONGITUD = "008";

        public const string TELEFONO_DEUDOR_EXISTE = "002";
        public const string TELEFONO_EXISTE = "003";
        public const string UBICACION_REQUERIDO = "004";
        public const string RESULTADO_REQUERIDO = "005";
        public const string OPERADOR_TELEFONICO_REQUERIDO = "006";
        public const string MESSAGE_CODE_NOT_FOUND = "001";

        public const string DIRECCION_LENGTH_ZERO = "009";
        public const string DIRECCION_LENGTH_LARGE = "010";
        public const string DIRECCION_DEPARTAMENTO_REQUERIDO = "011";
        public const string DIRECCION_PROVINCIA_REQUERIDO = "012";
        public const string DIRECCION_DISTRITO_REQUERIDO = "013";
        public const string DIRECCION_UBICACION_REQUERIDO = "014";

    }
}