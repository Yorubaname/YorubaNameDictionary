namespace Application.Exceptions
{
    public class UnpublishedNameUpdateException : ClientException
    {
        public UnpublishedNameUpdateException() : base("There is an unpublished update on this name already") { }
    }
}
