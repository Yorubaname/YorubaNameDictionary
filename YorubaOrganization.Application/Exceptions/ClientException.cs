namespace YorubaOrganization.Application.Exceptions
{
    /// <summary>
    /// TODO: This should be a 400 Bad Request exception
    /// </summary>
    public class ClientException : Exception
    {
        public ClientException()
        {
        }

        public ClientException(string message)
            : base(message)
        {
        }

        public ClientException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}
