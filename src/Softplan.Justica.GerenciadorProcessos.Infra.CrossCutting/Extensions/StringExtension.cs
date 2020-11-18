using System;
using System.Globalization;

namespace Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Extensions
{
    public static class StringExtension
    {
        public static bool EqualsIgnoreCase(this string str, string strToCompare)
        {
            if (str == null && strToCompare == null)
            {
                return true;
            }

            return str.Equals(strToCompare, StringComparison.OrdinalIgnoreCase) == true;
        }

        public static bool ContainsIgnoreCase(this string value, string substring)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(substring))
            {
                return true;
            }

            return value.Trim().IndexOf(substring.Trim(), StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string ToTitleCase(this string str)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str.ToLower());
        }
    }
}