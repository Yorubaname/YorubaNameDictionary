using System.Text;

namespace Words.Core.Utilities
{
    public static class WordTitleNormalizer
    {
        /// <summary>
        /// Trims a title and lowercases its first letter, preserving the remaining characters.
        /// </summary>
        /// <param name="title">The title to normalize.</param>
        /// <returns>The trimmed title with its first letter lowercased.</returns>
        public static string Normalize(string title)
        {
            var trimmedTitle = title.Trim();

            for (var index = 0; index < trimmedTitle.Length;)
            {
                var rune = Rune.GetRuneAt(trimmedTitle, index);
                if (Rune.IsLetter(rune))
                {
                    return trimmedTitle[..index]
                        + Rune.ToLowerInvariant(rune).ToString()
                        + trimmedTitle[(index + rune.Utf16SequenceLength)..];
                }

                index += rune.Utf16SequenceLength;
            }

            return trimmedTitle;
        }
    }
}
