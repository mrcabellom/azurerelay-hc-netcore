using AzureRelayCore.Domain.Tweets.Entities;
using Tweetinvi.Models;

namespace AzureRelayCore.Infrastructure.Services.TwitterStream.Extensions
{
    public static class TweetinviExtension
    {
        public static TweetInfo ToTweetInfo(this ITweet tweetinviModel)
        {
            return new TweetInfo()
            {
                UrlUserImage = tweetinviModel.CreatedBy.ProfileImageUrl400x400,
                CreatedByName = tweetinviModel.CreatedBy.Name,
                CreatedByScreenName = tweetinviModel.CreatedBy.ScreenName,
                CreatedAt = tweetinviModel.CreatedAt,
                Text = tweetinviModel.Text,
                RetweetedCount = tweetinviModel?.RetweetedTweet?.RetweetCount ?? 0,
                FavoriteCount = tweetinviModel?.RetweetedTweet?.FavoriteCount ?? 0
            };
        }
    }
}
