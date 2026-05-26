using GesMgmt.Domain.Constants;
using System.Text.RegularExpressions;

namespace GesMgmt.Application.Utils
{
    public class RegexUtils
    {
        public static bool ValidateCardBrandLength(string input)
        {
            return input.Length == 2;
        }
        public static bool ValidateMerchantCodeLength(string input)
        {
            return input.Length > 0 && input.Length <= 15;
        }

        public static bool ValidateLanguageLength(string input)
        {
            return input.Length > 0 && input.Length <= 3;
        }

        public static bool ValidateOriginLength(string input)
        {
            return input.Length > 0 && input.Length <= 50;
        }

        public static bool ValidateSubscriptionStatus(string input)
        {
            return input.Length > 0 && input.Length <= 50;
        }

        public static bool ValidateEmail(string email)
        {
            string pattern = @"(^[^@]{4,})+@([^@]{4,})+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        public static bool ValidateName(string name)
        {
            string pattern = @"^[a-zA-Z\u00C0-\u00FF ]+$";
            return Regex.IsMatch(name, pattern);
        }

        public static bool ValidateNameLength(string name)
        {
            string pattern = @"^[a-zA-Z\u00C0-\u00FF ]{2,50}$";
            return Regex.IsMatch(name, pattern);
        }

        public static bool ValidatePhoneNumber(string phoneNumber)
        {
            string pattern = @"^\+?[0-9\s\-]{7,15}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }

        public static bool ValidatePan(string pan)
        {
            string pattern = @"^[0-9]{14,19}$";
            return Regex.IsMatch(pan, pattern);
        }

        public static bool ValidateExpMonth(string expMonth)
        {
            string pattern = @"^(0?[1-9]|1[012])$";
            return Regex.IsMatch(expMonth, pattern);
        }

        public static bool ValidateExpYear(string expYear)
        {
            string pattern = @"^(19|20)\d{2}$";
            return Regex.IsMatch(expYear, pattern);
        }

        public static bool ValidateDocumentType(string documentType)
        {
            string pattern = @"^(DNI|CE|PASAPORTE|RUC|OTROS)$";
            return Regex.IsMatch(documentType, pattern);
        }

        public static bool ValidateDocumentNumber(string documentNumber, string documentType)
        {
            string pattern = @"^[a-zA-Z0-9]{8,12}$";
            switch (documentType.ToUpper())
            {
                case Const.DOCUMENT_TYPE_DNI:
                    return ValidateDNI(documentNumber);
                case Const.DOCUMENT_TYPE_RUC:
                    return ValidateRUC(documentNumber);
                case Const.DOCUMENT_TYPE_CE:
                    return ValidateCE(documentNumber);
                case Const.DOCUMENT_TYPE_PASAPORTE:
                    return ValidatePassport(documentNumber);
                default:
                    return Regex.IsMatch(documentNumber, pattern);
            }
        }

        public static bool ValidateDNI(string dni)
        {
            string pattern = @"^[0-9]{8}$";
            return Regex.IsMatch(dni, pattern);
        }

        public static bool ValidateRUC(string ruc)
        {
            string pattern = @"^[0-9]{11}$";
            return Regex.IsMatch(ruc, pattern);
        }

        public static bool ValidateCE(string carneExtranjeria)
        {
            string pattern = @"^[0-9]{9}$";
            return Regex.IsMatch(carneExtranjeria, pattern);
        }

        public static bool ValidatePassport(string passport)
        {
            string pattern = @"^[a-zA-Z0-9]{6,9}$";
            return Regex.IsMatch(passport, pattern);
        }

        public static bool ValidateDateTimeMillis(string time)
        {
            string pattern = @"^\d\d\d\d-(0?[1-9]|1[0-2])-(0?[1-9]|[12][0-9]|3[01]) (2[0-3]|[01][0-9]):([0-9]|[0-5][0-9]):([0-9]|[0-5][0-9])\.(\d{3})$";
            return Regex.IsMatch(time, pattern);
        }

        public static bool ValidateInteger(string number)
        {
            string pattern = @"^\d+$";
            return Regex.IsMatch(number, pattern);
        }

        public static bool ValidateCurrencyCode(string currencyCode)
        {
            string pattern = @"^(604|840)$";
            return Regex.IsMatch(currencyCode, pattern);
        }

        public static bool ValidateCommentsLength(string comments)
        {
            return comments.Length > 0 && comments.Length <= 150;
        }
        public static bool ValidateDecimal_18_2(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            string pattern = @"^\d{1,16}(\.\d{1,2})?$";
            return Regex.IsMatch(value, pattern);
        }

        public static bool ValidateMerchantBuyerIdLength(string merchantBuyerId)
        {
            return merchantBuyerId.Length > 5 && merchantBuyerId.Length <= 100;
        }

        public static bool ValidateCardTokenLength(string token)
        {
            return token.Length > 0 && token.Length <= 64;
        }

        public static bool ValidateStateConfirm(string state)
        {
            string pattern = @"^(A|D)$";
            return Regex.IsMatch(state, pattern);
        }

        public static bool ValidateRejectionReasonLength(string reason)
        {
            return reason.Length > 0 && reason.Length <= 100;
        }

        public static bool ValidateTransactionIdLength(string input)
        {
            return input.Length > 4 && input.Length <= 40;
        }

    }
}
