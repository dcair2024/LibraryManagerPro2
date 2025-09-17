using System.Globalization;
using System.Text;

namespace LibraryManagerPro.Utils
{
    public static class StringExtensions
    {
        public static string NormalizeToCompare(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            // Remove acentos e coloca em minúsculo
            var normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().ToLowerInvariant();
        }
    }
}
