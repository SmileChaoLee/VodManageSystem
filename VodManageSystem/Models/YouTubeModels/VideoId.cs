namespace VodManageSystem.Models.YouTubeModels
{
    public class VideoId
    {
        // The "kind" field will usually be "youtube#video" for search results where type=video
        public string Kind { get; set; } = string.Empty;
        // **This is the ID you need for the android_youtube_player**
        public string Id { get; set; } = string.Empty;
    }    
}