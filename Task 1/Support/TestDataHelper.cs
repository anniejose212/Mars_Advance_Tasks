// FILE: TestDataHelper.cs
// ROLE: Decodes test input placeholders.
//       Kept minimal for first push — more methods added as features are built.

namespace Task1.Support
{
    public static class TestDataHelper
    {
        /// <summary>
        /// Replaces placeholder tokens in test input strings.
        /// {DQ}   → double-quote character
        /// {EQ:n} → n '=' characters
        /// </summary>
        public static string NormalizeTestInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = input.Replace("{DQ}", "\"");

            if (input.Contains("{EQ:"))
            {
                int start = input.IndexOf("{EQ:", System.StringComparison.Ordinal);
                int end   = input.IndexOf("}", start, System.StringComparison.Ordinal);

                if (start != -1 && end != -1)
                {
                    string token = input.Substring(start, end - start + 1);
                    string num   = token.Replace("{EQ:", "").Replace("}", "");

                    if (int.TryParse(num, out int count))
                        input = input.Replace(token, new string('=', count));
                }
            }

            return input.Trim();
        }
    }
}
