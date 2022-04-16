using Tweetinvi;
using Tweetinvi.Exceptions;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace GuidBot;

public class GuidGenieResponder : BackgroundService
{
    private readonly TwitterClient twitterClient;
    private readonly ILogger<GuidGenieResponder> logger;

    public GuidGenieResponder(TwitterClient twitterClient, ILogger<GuidGenieResponder> logger)
    {
        this.twitterClient = twitterClient;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var stream = twitterClient.Streams.CreateFilteredStream();

            stream.AddTrack("@GuidGenie", Received);
            stream.StreamStarted += (_, _) => logger.LogInformation("Starting Filtered Streaming...");
            stream.StreamStopped += (_, _) => logger.LogInformation("Stream Stopped");
            
            try
            {
                await stream.StartMatchingAnyConditionAsync().WaitAsync(stoppingToken);
            }
            catch (Exception e)
            {
                try
                {
                    stream.Stop();
                }
                catch
                {
                    // ignored
                }

                logger.LogError(e, "Stream stopped due to exception");
            }
        }
    }

    private async void Received(ITweet tweet)
    {
        logger.LogInformation("{tweet}", tweet);

        try
        {
            var client = tweet.Client;
            var parameters = new PublishTweetParameters($"@{tweet.CreatedBy} {Guid.NewGuid()}")
                {InReplyToTweet = tweet};
            await client.Tweets.PublishTweetAsync(parameters);
            logger.LogInformation("Reply sent to {username}", tweet.CreatedBy);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to reply to {tweet}", tweet);
        }
    }
}