namespace YorubaOrganization.Core.Dto
{
    public class CommaSeparatedString : CharacterSeparatedString<CommaSeparatedString>
    {
        public CommaSeparatedString(string? value) : base(value)
        {
            SeparatorIn = ',';
            SeparatorOut = ", ";
        }
    }
}
