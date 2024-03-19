namespace Core.Dto
{
    public class HyphenSeparatedString : CharacterSeparatedString
    {
        private const char Hyphen = '-';
        public HyphenSeparatedString(string? value) : base(value)
        {
        }

        // TODO: Find a way to use generics here to avoid repeating these methods in both classes
        public static implicit operator HyphenSeparatedString(List<string> list)
        {
            return new HyphenSeparatedString(string.Join(Hyphen, list));
        }

        public static implicit operator List<string>(HyphenSeparatedString commaSeparatedString)
        {
            return commaSeparatedString.value.Split(Hyphen).Select(item => item.Trim()).ToList();
        }
    }
}
