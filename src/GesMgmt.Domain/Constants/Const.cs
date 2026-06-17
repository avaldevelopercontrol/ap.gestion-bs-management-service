namespace GesMgmt.Domain.Constants
{
    public class Const
    {
        public const string MESSAGES_CACHE_KEY = "VALIDATION_MESSAGES_CACHE";
        public const string ORIGINS_CACHE_KEY = "ORIGINS_CACHE";
        public const string BUYERS_CACHE_KEY = "BUYERS_CACHE";
        public const string SUBSCRIPTIONSTATUS_CACHE_KEY = "SUBSCRIPTIONSTATUS_CACHE";
        public const string CARD_BRANDS_CACHE_KEY = "CARD_BRANDS_CACHE";

        public const string GESTION_API_MESSAGE = "Gestion";

        public const int DEFAULT_INSTALMENT_PLAN = 12;
        public const int DEFAULT_MAX_INSTALMENT_PLAN = 99;

        public const string LANGUAGE_ESP = "ESP";
        public const string LANGUAGE_ENG = "ENG";

        public const string SUCCESS_CODE = "00";
        public const string SUCCESS_MESSAGE = "OK";
        public const string ERROR_MESSAGE = "ERROR";

        public const int BAD_REQUEST_CODE = 400;
        public const int OK_REQUEST_CODE = 200;
        public const int ERROR_REQUEST_CODE = 500;

        public const string DOCUMENT_TYPE_DNI = "DNI";
        public const string DOCUMENT_TYPE_CE = "CE";
        public const string DOCUMENT_TYPE_PASAPORTE = "PASAPORTE";
        public const string DOCUMENT_TYPE_RUC = "RUC";
        public const string DOCUMENT_TYPE_OTROS = "OTROS";

        // SuscriptionStatus
        public const int SUB_STAT_ACTIVO = 1;
        public const int SUB_STAT_PENDIENTE = 2;
        public const int SUB_STAT_MORA = 3;
        public const int SUB_STAT_FINALIZADO = 4;
        public const int SUB_STAT_CANCELADO = 5;
        public const int SUB_STAT_RECHAZADO = 6;
        public const int SUB_STAT_VENCIDO = 7;
        public const int SUB_LENGTH_CODE = 30;

        public const int SUB_MOVE_REINTENTANDO = 5;

        public const int CHARGE_TYPE_FIXED = 1;
        public const int CHARGE_TYPE_VARIABLE = 2;

        public const string STAT_CONFIRM_APPROVE = "A"; 
        public const string STAT_CONFIRM_DENY = "D";

        public const int MAXIMUM_DAYS_OF_DIFFERENCE = 180;

    }
}