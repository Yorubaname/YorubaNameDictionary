namespace YorubaOrganization.Core.Dto
{
    public abstract class CharacterSeparatedString<T> where T : CharacterSeparatedString<T>
    {
        protected string SeparatorOut { get; init; }
        protected char SeparatorIn { get; init; }
        protected readonly string value;

        public CharacterSeparatedString(string? value)
        {
            this.value = value?.Trim() ?? string.Empty;
        }

        public override string ToString()
        {
            return value;
        }

        public static implicit operator CharacterSeparatedString<T>(List<string> list)
        {
            var anObject = (CharacterSeparatedString<T>)Activator.CreateInstance(typeof(T), string.Empty);
            return (CharacterSeparatedString<T>)Activator.CreateInstance(typeof(T), string.Join(anObject.SeparatorOut, list));

        }

        public static implicit operator List<string>(CharacterSeparatedString<T> charSeparatedString)
        {
            return charSeparatedString.value
                .Split(charSeparatedString.SeparatorIn)
                .Select(item => item.Trim())
                .Where(item => !string.IsNullOrEmpty(item))
                .ToList();
        }
    }
}
