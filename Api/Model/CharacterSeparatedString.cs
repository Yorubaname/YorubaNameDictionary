namespace Api.Model
{
    public abstract class CharacterSeparatedString
    {
        protected readonly string value;

        public CharacterSeparatedString(string? value)
        {
            this.value = value ?? string.Empty;
        }

        public override string ToString()
        {
            return value;
        }
    }
}
