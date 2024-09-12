namespace Application.Services
{
    public interface ITwitterService
    {
        ValueTask<string> BuildNameTweet(string name, string meaning);
        Task PostTweet(string text);
    }
}
