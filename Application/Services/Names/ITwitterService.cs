namespace Application.Services.Names
{
    public interface ITwitterService
    {
        Task PostNewNameAsync(string name, string meaning, CancellationToken cancellationToken);
    }
}
