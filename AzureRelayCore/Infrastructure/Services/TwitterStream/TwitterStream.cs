using Microsoft.Extensions.Options;
using AzureRelayCore.Helpers;
using AzureRelayCore.Infrastructure.Services.RelayConnection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Streaming;
using AzureRelayCore.Infrastructure.Services.TwitterStream.Extensions;
using Newtonsoft.Json;

namespace AzureRelayCore.Infrastructure.Services.TwitterStream
{
    public class TwitterStream : ITwitterStream, IStream
    {
        private readonly IRelayConnection _relayConnection;
        private IFilteredStream _stream;
        private const int THROTTLE_MILLISECONDS = 350;
        private System.IO.StreamWriter _streamWriter;
        private TwitterFilterOptions _filterOptions;

        public TwitterStream(IOptions<TwitterConfig> twitterConfig,
            IOptions<TwitterFilterOptions> twitterFiltersOptions,
            IRelayConnection relayConnection)
        {
            _relayConnection = relayConnection;
            _filterOptions = twitterFiltersOptions.Value;
            AuthUser(twitterConfig.Value);
        }
        public async Task StartStreamAsync()
        {
            System.IO.Stream hyConnection = await _relayConnection.CreateRelayStreamAsync();
            await StartTwitterStreamAsync(hyConnection);
        }
        private void AuthUser(TwitterConfig twitterConfig)
        {
            Auth.SetUserCredentials(
               twitterConfig.ConsumerKey,
               twitterConfig.ConsumerSecret,
               twitterConfig.UserAccessToken,
               twitterConfig.UserAccessSecret
           );
        }

        public async Task StartTwitterStreamAsync(System.IO.Stream hyConnection)
        {
            _streamWriter = new System.IO.StreamWriter(hyConnection) { AutoFlush = true };
            var limiter = new Throttle(1, TimeSpan.FromMilliseconds(THROTTLE_MILLISECONDS));
            _stream = Stream.CreateFilteredStream();
            AddFiltersStream(_filterOptions);
            _stream.MatchingTweetReceived += (sender, t) =>
            {
                var ct = new CancellationToken();
                limiter.Enqueue(async () =>
                {
                    await _streamWriter.WriteLineAsync(JsonConvert.SerializeObject(t.Tweet.ToTweetInfo()));

                }, ct);
            };
            await _stream.StartStreamMatchingAnyConditionAsync();
        }

        private void AddFiltersStream(TwitterFilterOptions filteredOptions)
        {
            var user = User.GetUserFromScreenName(filteredOptions.FollowUserName);
            _stream.AddFollow(user);
            _stream.AddTrack(filteredOptions.Track);
        }

        public void StopTwitterStream()
        {
            if (_stream.StreamState == StreamState.Stop)
            {
                return;
            }
            _stream.StopStream();
            _stream.ClearTracks();
        }

        public async Task StopStreamAsync()
        {
            StopTwitterStream();
            _streamWriter.Dispose();
            await _relayConnection.CloseRelayStreamAsync();
        }
    }
}
