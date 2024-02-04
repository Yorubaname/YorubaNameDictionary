namespace Api.Model
{
    public class CommaSeparatedString : CharacterSeparatedString
    {
        private const string CommaSpace = ", ";
        private const char Comma = ',';

        public CommaSeparatedString(string? value) : base(value)
        {
        }


        public static implicit operator CommaSeparatedString(List<string> list)
        {
            string commaSeparatedValue = string.Join(CommaSpace, list);
            return new CommaSeparatedString(commaSeparatedValue);
        }
        
        public static implicit operator CommaSeparatedString(string commaSeperatedString)
        {
            return new CommaSeparatedString(commaSeperatedString);
        }

        public static implicit operator List<string>(CommaSeparatedString commaSeparatedString)
        {
            return commaSeparatedString.value.Split(Comma).Select(item => item.Trim()).ToList();
        }
    }
}
