namespace Infrastructure.Configuration
{
    public record TwitterConfig(
        string ConsumerKey,
        string ConsumerSecret,
        string AccessToken,
        string AccessTokenSecret,
        string NameUrlPrefix,
        string TweetTemplate,
        int TweetIntervalSeconds)
    {
        public TwitterConfig() : this("", "", "", "", "", "", default) { }
    }
}
