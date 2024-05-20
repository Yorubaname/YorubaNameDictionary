namespace Api.Utilities
{
    public static class ResponseHelper
    {
        public static Dictionary<string, string> GetResponseDict(string theMessage)
        {
            return new Dictionary<string, string>
            {
                { "message", theMessage}
            };
        }
    }
}
