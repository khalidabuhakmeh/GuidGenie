using GuidBot;
using Tweetinvi;
using Tweetinvi.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Guid Genie
var credentials = new TwitterCredentials
{
    ConsumerKey = configuration["API_KEY"],
    ConsumerSecret = configuration["API_KEY_SECRET"],
    AccessToken = configuration["ACCESS_TOKEN"],
    AccessTokenSecret = configuration["ACCESS_TOKEN_SECRET"],
};

var twitterClient = new TwitterClient(credentials);
await twitterClient.Auth.InitializeClientBearerTokenAsync();

builder.Services.AddSingleton(twitterClient);
builder.Services.AddHostedService<GuidGenieResponder>();

var app = builder.Build();

app.MapGet("/", () => Results.Content(/* language=html */"<html><h1><a href=\"https://twitter.com/guidgenie\">@GuidGenie</a></h1></html>", "text/html"));

await app.RunAsync();