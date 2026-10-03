// FILE: UiTextHelper.cs
// ROLE: Shared text comparison for components: trims, decodes HTML entities, ignores case.

using System;
using System.Net;

namespace Task1.Support
{
    public static class UiTextHelper
    {
        // Trims and decodes HTML entities, e.g. "&amp;" becomes "&"
        public static string Normalize(string s)
        {
            if (s == null) return string.Empty;
            return WebUtility.HtmlDecode(s).Trim();
        }

        // Case-insensitive, ignores spaces at either end
        public static bool IsSameText(string actual, string expected)
        {
            return string.Equals(Normalize(actual), Normalize(expected),
                StringComparison.OrdinalIgnoreCase);
        }
    }
}