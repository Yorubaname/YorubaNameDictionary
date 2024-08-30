namespace Infrastructure.Twitter
{
    public interface ITwitterClientV2
    {
        Task<TweetV2PostResponse> PostTweet(string text);
    }
}
