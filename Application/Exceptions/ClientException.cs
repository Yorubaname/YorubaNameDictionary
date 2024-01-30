namespace Application.Exceptions
{
    /// <summary>
    /// TODO: This should be a 400 Bad Request exception
    /// </summary>
    public abstract class ClientException : Exception
    {
        protected virtual string ClientMessage { get; init; }
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
