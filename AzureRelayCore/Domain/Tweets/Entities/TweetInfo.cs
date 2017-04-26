
using System;

namespace AzureRelayCore.Domain.Tweets.Entities
{
    public class TweetInfo
    {
        public string UrlUserImage { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedByScreenName { get; set; }
        public int FavoriteCount { get; set; }
        public int RetweetedCount { get; set; }
    }
}
