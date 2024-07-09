namespace Application.Services
{
    public class YorubaAlphabetService
    {
        public static List<string> YorubaAlphabet
        {
            get
            {
                var alphabets = new List<string>("abdefghijklmnoprstuwy".ToCharArray().Select(c => c.ToString()));
                alphabets.Insert(4, "ẹ");
                alphabets.Insert(7, "gb");
                alphabets.Insert(16, "ọ");
                alphabets.Insert(20, "ṣ");
                return alphabets;
            }
        }
    }
}
