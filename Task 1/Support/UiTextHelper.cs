// FILE: UiTextHelper.cs
// ROLE: Normalises UI text and performs case-insensitive equality checks.

using System;
using System.Net;

namespace Task1.Support
{
    public static class UiTextHelper
    {
        public static string Normalize(string s)
        {
            if (s == null) return string.Empty;
            return WebUtility.HtmlDecode(s).Trim();
        }

        public static bool EqNorm(string a, string b)
        {
            return string.Equals(Normalize(a), Normalize(b),
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
