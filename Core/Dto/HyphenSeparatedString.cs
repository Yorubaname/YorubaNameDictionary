
namespace Core.Dto
{
    public class HyphenSeparatedString : CharacterSeparatedString<HyphenSeparatedString>
    {
        public HyphenSeparatedString(string? value) : base(value)
        {
            SeparatorIn = '-';
            SeparatorOut = "-";
        }

        
    }
}
