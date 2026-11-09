using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using VodManageSystem.Models.DataModels;

namespace VodManageSystem.Services
{
    public class InvalidVideoIdService
    {
        private const string CacheKey = nameof(InvalidVideoIdService);
        private readonly KtvSystemDBContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        public InvalidVideoIdService(KtvSystemDBContext context, IConfiguration configuration, IMemoryCache cache)
        {
            _context = context;
            _configuration = configuration;
            _cache = cache;
        }

        public async Task<HashSet<string>> GetInvalidVideoIdsAsync(CancellationToken cancellationToken = default, bool forceRefresh = false)
        {
            if (!forceRefresh && _cache.TryGetValue(CacheKey, out HashSet<string>? cachedIds) && cachedIds != null)
            {
                return cachedIds;
            }

            List<string> videoIds = await _context.Song
                .AsNoTracking()
                .Where(song => song.NMpeg == "00" && song.MMpeg == "00" && song.VodNo != null && song.VodNo != "")
                .Select(song => song.VodNo!)
                .Distinct()
                .ToListAsync(cancellationToken);

            var invalidVideoIds = new HashSet<string>(StringComparer.Ordinal);
            if (videoIds.Count > 0)
            {
                string? apiKey = _configuration["YouTube:ApiKey"];
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    throw new InvalidOperationException("The YouTube API key is not configured.");
                }

                using var youtubeService = new YouTubeService(new BaseClientService.Initializer
                {
                    ApiKey = apiKey,
                    ApplicationName = "VodManageSystem"
                });

                foreach (string[] batch in videoIds.Chunk(50))
                {
                    var request = youtubeService.Videos.List("snippet");
                    request.Id = string.Join(",", batch);
                    var response = await request.ExecuteAsync(cancellationToken);
                    var availableVideoIds = response.Items?
                        .Select(video => video.Id)
                        .ToHashSet(StringComparer.Ordinal) ?? new HashSet<string>(StringComparer.Ordinal);

                    invalidVideoIds.UnionWith(batch.Where(videoId => !availableVideoIds.Contains(videoId)));
                }
            }

            _cache.Set(CacheKey, invalidVideoIds, TimeSpan.FromMinutes(30));
            return invalidVideoIds;
        }
    }
}