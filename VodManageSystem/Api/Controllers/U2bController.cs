using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Configuration;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System.Linq;
using VodManageSystem.Models.U2bModels;
using Newtonsoft.Json.Linq;

namespace VodManageSystem.Api.Controllers
{
    [Route("api/[controller]")]
    public class U2bController : Controller
    {

        private readonly string _apiKey;
        private readonly string? _apiKey2;

        public U2bController(IConfiguration configuration)
        {
            // Read your API key securely from appsettings.json or Environment Variables
            _apiKey = configuration["YouTube:ApiKey"] 
                ?? throw new ArgumentNullException("YouTube API Key is missing.");

            _apiKey2 = configuration["YouTube:ApiKey2"]
                ?? throw new ArgumentNullException("YouTube ApiKey2 is missing.");
        }

        // Changed route to handle space characters and symbols safely via query strings
        // URL path will be: GET api/u2b/search?query=your+search+term
        // [HttpGet("search")]
        // public async Task<IActionResult> SearchVideos([FromQuery] string query)

        // URL path will be: GET api/u2b/query
        [HttpGet("{queryString}")]
        public async Task<IActionResult> SearchVideos(string queryString)
        {
            Console.WriteLine("U2bController.SearchVideos.queryString = " + queryString);
            Console.WriteLine("U2bController.SearchVideos.No pageToken provided.");
            return await SearchVideos(queryString, "", true); // for old version of U2bPlayer
        }

        // URL path will be: GET api/u2b/query
        [HttpGet("{queryString}/{pageToken}")]
        public async Task<IActionResult> SearchVideos(string queryString, string pageToken, bool isOldVersion = false)
        {
            Console.WriteLine("U2bController.SearchVideos.queryString = " + queryString);
            Console.WriteLine("U2bController.SearchVideos.pageToken = " + pageToken);
            if (string.IsNullOrWhiteSpace(queryString))
            {
                return BadRequest("Search query cannot be empty.");
            }

            try
            {
                // 1. Initialize the official YouTube service
                using var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = _apiKey,
                    ApplicationName = "YourAndroidAppBackend"
                });
                if (isOldVersion)
                {
                    return await GetVideosFromYouTubeOld(youtubeService, queryString);
                }
                return await GetVideosFromYouTube(youtubeService, queryString, pageToken);
            }
            catch (Exception ex)
            {
                // Log exception internally and mask server secrets from client
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // URL path will be: GET api/u2b/2/query        
        [HttpGet("{secondApiKey}/{queryString}/{pageToken}")]
        public async Task<IActionResult> SearchVideos2(string secondApiKey, string queryString, string? pageToken = null)
        {
            Console.WriteLine("U2bController.SearchVideos2.secondApiKey = " + secondApiKey);
            Console.WriteLine("U2bController.SearchVideos2.queryString = " + queryString);
            if (string.IsNullOrWhiteSpace(queryString))
            {
                return BadRequest("Search query cannot be empty.");
            }

            try
            {
                // 1. Initialize the official YouTube service
                using var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = _apiKey2,
                    ApplicationName = "YourAndroidAppBackend"
                });

                return await GetVideosFromYouTube(youtubeService, queryString, pageToken);
            }
            catch (Exception ex)
            {
                // Log exception internally and mask server secrets from client
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task<IActionResult> GetVideosFromYouTubeOld(YouTubeService youtubeService, string queryString)
        {
            Console.WriteLine("U2bController.GetVideosFromYouTubeOld");            
            // 2. Configure the search request parameters
            var searchRequest = youtubeService.Search.List("snippet");
            searchRequest.Q = queryString;
            searchRequest.Type = "video"; // Excludes channels/playlists
            searchRequest.MaxResults = 50; // Limits data payload size

            // 3. Execute request to Google servers securely
            var searchResponse = await searchRequest.ExecuteAsync();

            // 4. Map the complex Google API data model to your clean Android model
            var cleanedVideos = searchResponse.Items.Select(item => new YouTubeVideo
            {
                Id = item.Id.VideoId,
                Title = item.Snippet.Title,
                Thumbnail = item.Snippet.Thumbnails.High?.Url ?? item.Snippet.Thumbnails.Medium?.Url ?? "",
                ChannelTitle = item.Snippet.ChannelTitle
            }).ToList();
            
            JArray jArray = [];
            if (cleanedVideos == null)
            {
                return Ok(jArray.ToString());
                // return Ok(new List<YouTubeVideo>());
            }
            Console.WriteLine("U2bController.GetVideosFromYouTubeOld.cleanedVideos.Count = " + cleanedVideos.Count);
            
            // var responsePayload = new VideoList { Videos = cleanedVideos };
            
            // 5. Send JSON array payload back to the Android client
            foreach (var video in cleanedVideos)
            {
                jArray.Add(new JObject
                {
                    {"id", video.Id},
                    {"title", video.Title},
                    {"thumbnail", video.Thumbnail},
                    {"channelTitle", video.ChannelTitle}
                });
            }
            return Ok(jArray.ToString());   // more readable
            // 5. Send optimized JSON payload back to the Android client
            // return Ok(cleanedVideos);
        }

        private async Task<IActionResult> GetVideosFromYouTube(YouTubeService youtubeService, string queryString, string? pageToken)
        {            
            // 2. Configure the search request parameters
            var searchRequest = youtubeService.Search.List("snippet");
            searchRequest.Q = queryString;
            searchRequest.Type = "video"; // Excludes channels/playlists
            searchRequest.MaxResults = 50; // Limits data payload size
            if (!string.IsNullOrEmpty(pageToken) && pageToken.Trim().ToLower() != "first")
            {
                Console.WriteLine("U2bController.GetVideosFromYouTube.pageToken = " + pageToken);
                searchRequest.PageToken = pageToken;
            }

            // 3. Execute request to Google servers securely
            var searchResponse = await searchRequest.ExecuteAsync();

            // 4. Map the complex Google API data model to your clean Android model
            var cleanedVideos = searchResponse.Items.Select(item => new YouTubeVideo
            {
                Id = item.Id.VideoId,
                Title = item.Snippet.Title,
                Thumbnail = item.Snippet.Thumbnails.High?.Url ?? item.Snippet.Thumbnails.Medium?.Url ?? "",
                ChannelTitle = item.Snippet.ChannelTitle
            }).ToList();
            
            var responseJson = new JObject
            {
                ["nextPageToken"] = searchResponse.NextPageToken, // Critical for Android
                ["ytVideos"] = new JArray()
            };
            if (cleanedVideos != null)
            {
                Console.WriteLine("U2bController.GetVideosFromYouTube.cleanedVideos.Count = " + cleanedVideos.Count);                        
                // 5. Send JSON Object payload back to the Android client
                responseJson = new JObject
                {
                    ["nextPageToken"] = searchResponse.NextPageToken, // Critical for Android                
                    ["ytVideos"] = new JArray(cleanedVideos.Select(video => new JObject
                    {
                        {"id", video.Id},
                        {"title", video.Title},
                        {"thumbnail", video.Thumbnail},
                        {"channelTitle", video.ChannelTitle}
                    }))                   
                };
            }            
        
            return Ok(responseJson.ToString());   // more readable                        
        }
    }
}