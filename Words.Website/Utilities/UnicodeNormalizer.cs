using System.Text;

namespace Words.Website.Utilities
{
    public static class UnicodeNormalizer
    {
        public static string? NormalizeLetter(this string word)
        {
            if (word == null)
                return null;

            // Normalize to Form D (NFD)
            string normalized = word.Normalize(NormalizationForm.FormD);

            return normalized;
        }
    }
}