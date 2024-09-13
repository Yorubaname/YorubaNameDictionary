namespace Application.Services
{
    public interface ITwitterService
    {
        Task PostNewNameAsync(string name, string meaning, CancellationToken cancellationToken);
    }
}
