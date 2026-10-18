using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace QLNS.Core.Common
{
    public static class StringHelper
    {
        public static string GenerateCode(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (char c in normalized)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            string code = sb.ToString().Normalize(NormalizationForm.FormC);
            code = code.Replace('đ', 'd').Replace('Đ', 'D');

            code = new string(code.Where(char.IsLetterOrDigit).ToArray());

            return code.ToUpperInvariant();
        }
    }
}
