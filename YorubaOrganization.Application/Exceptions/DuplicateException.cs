namespace YorubaOrganization.Application.Exceptions
{
    public class DuplicateException : ClientException
    {
        public DuplicateException() : base() { }

        public DuplicateException(string message) : base(message) { }
    }
}
