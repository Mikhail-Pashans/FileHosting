using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace FileHosting.MVC.Extensions
{
    public static class MyExtensions
    {
        public static bool IsValidEmail(this string email)
        {
            const string expresion = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

            if (Regex.IsMatch(email, expresion))
            {
                return Regex.Replace(email, expresion, string.Empty).Length == 0;
            }

            return false;
        }

        public static string ToHashedString(this string inputString)
        {
            var sha = SHA256.Create();
            var encodedBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(inputString));

            return BitConverter.ToString(encodedBytes).Replace("-", "").ToLower();
        }        

        //public static Dictionary<int, string> EnumToDictionary<TEnum>(this TEnum enumValue) where TEnum : struct
        //{
        //    var values = Enum.GetValues(typeof (TEnum))
        //        .Cast<TEnum>()
        //        .ToDictionary(item => (Convert.ToInt32(item)), item => item.ToString());

        //    return values;
        //}

        //public static SelectList EnumToSelectList<TEnum>(this TEnum enumValue) where TEnum : struct
        //{
        //    var items = Enum.GetValues(typeof (TEnum))
        //        .Cast<TEnum>()
        //        .Select(item => new SelectListItem
        //        {
        //            Value = (Convert.ToInt32(item)).ToString(CultureInfo.InvariantCulture),
        //            Text = item.ToString()
        //        });

        //    return new SelectList(items, "Value", "Text", Convert.ToInt32(enumValue));
        //}
    }
}