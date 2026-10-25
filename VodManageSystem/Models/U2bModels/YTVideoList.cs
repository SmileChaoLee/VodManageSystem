using System.Collections.Generic;

namespace VodManageSystem.Models.U2bModels
{
    public class YTVideoList
    {
        public string NextPageToken { get; set; } = string.Empty;
        public List<YouTubeVideo> YTVideos { get; set; } = [];
    }
}