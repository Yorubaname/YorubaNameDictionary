﻿using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Utilities
{
    public static class DiacriticsNormalizer
    {
        private static string RemoveDiacriticsAndSimplify(this string text)
        {
            const char a = 'a', e = 'e', i = 'i', o = 'o', u = 'u';
            var vowelMap = new Dictionary<char, char>
            {
                { 'á', a },
                { 'à', a },
                { 'a', a },
                { 'é', e },
                { 'è', e },
                { 'e', e },
                { 'ẹ', e },
                { 'í', i },
                { 'ì', i },
                { 'i', i },
                { 'ó', o },
                { 'ò', o },
                { 'o', o },
                { 'ọ', o },
                { 'ú', u },
                { 'ù', u },
                { 'u', u }
            };

            var decomposed = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    if (vowelMap.TryGetValue(c, out var simpleVowel))
                    {
                        stringBuilder.Append(simpleVowel);
                    }
                    else
                    {
                        stringBuilder.Append(c);
                    }
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string ReplaceYorubaVowelsWithPattern(this string term)
        {
            var vowelPatterns = new Dictionary<char, string>
            {
                { 'a', "[aáà]\\p{Mn}?" },
                { 'e', "[eéèẹ]\\p{Mn}?" },
                { 'i', "[iíì]\\p{Mn}?" },
                { 'o', "[oóòọ]\\p{Mn}?" },
                { 'u', "[uúù]\\p{Mn}?" }
            };

            term = RemoveDiacriticsAndSimplify(term);

            return string.Concat(term.Select(c =>
                vowelPatterns.TryGetValue(c, out var pattern) ? pattern : Regex.Escape(c.ToString())
            ));
        }
    }
}
