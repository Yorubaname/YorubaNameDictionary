namespace Api.Model
{
    public class CommaSeparatedString
    {
        private readonly string value;

        public CommaSeparatedString(string value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value;
        }

        public static implicit operator CommaSeparatedString(List<string> list)
        {
            string commaSeparatedValue = string.Join(", ", list);
            return new CommaSeparatedString(commaSeparatedValue);
        }

        public static implicit operator List<string>(CommaSeparatedString commaSeparatedString)
        {
            return commaSeparatedString.value.Split(',').Select(item => item.Trim()).ToList();
        }
    }
}
