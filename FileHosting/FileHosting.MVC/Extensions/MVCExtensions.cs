using System.Text.RegularExpressions;

namespace FileHosting.MVC.Extensions
{
    public static class MvcExtensions
    {
        public static bool IsValidEmail(this string email)
        {
            const string expresion =
                @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

            if (Regex.IsMatch(email, expresion))
            {
                return Regex.Replace(email, expresion, string.Empty).Length == 0;
            }

            return false;
        }
    }
}