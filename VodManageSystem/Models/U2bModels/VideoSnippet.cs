namespace VodManageSystem.Models.U2bModels
{
    public class VideoSnippet
    {
        public string PublishedAt { get; set; } = string.Empty;
        public string ChannelId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Thumbnails? Thumbnails { get; set; } = null;
        public string ChannelTitle { get; set; } = string.Empty;
    }
}