using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Configuration;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System.Linq;
using VodManageSystem.Models.U2bModels;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace VodManageSystem.Api.Controllers
{
    [Route("api/[controller]")]
    public class U2bController : Controller
    {

        private readonly string _apiKey;

        public U2bController(IConfiguration configuration)
        {
            // Read your API key securely from appsettings.json or Environment Variables
            _apiKey = configuration["YouTube:ApiKey"] 
                ?? throw new ArgumentNullException("YouTube API Key is missing.");
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
                Console.WriteLine("U2bController.cleanedVideos.Count = " + cleanedVideos.Count);
                
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
            catch (Exception ex)
            {
                // Log exception internally and mask server secrets from client
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}