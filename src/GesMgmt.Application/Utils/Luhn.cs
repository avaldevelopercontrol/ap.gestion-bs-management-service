using System.Text;

namespace GesMgmt.Application.Utils
{
    public class Luhn
    {
        public static bool ValidarLuhn(string tarjeta)
        {
            StringBuilder digitsOnly = new StringBuilder();

            foreach (char c in tarjeta.Where(c => char.IsDigit(c)))
            {
                digitsOnly.Append(c);
            }

            int sum = 0;
            int digit = 0;
            int addend = 0;
            bool timesTwo = false;

            for (int i = digitsOnly.Length - 1; i >= 0; i--)
            {
                digit = Int32.Parse(digitsOnly.ToString(i, 1));
                if (timesTwo)
                {
                    addend = digit * 2;
                    if (addend > 9)
                        addend -= 9;
                }
                else
                    addend = digit;
                sum += addend;
                timesTwo = !timesTwo;

            }
            return (sum % 10) == 0;
        }
    }
}
