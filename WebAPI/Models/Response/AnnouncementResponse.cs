
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class AnnouncementResponse
{
  [JsonPropertyName("announce_id")]
  public string AnnouncementId { get; set; } = null!;

  [JsonPropertyName("topic")]
  public string Topic { get; set; } = null!;

  [JsonPropertyName("content")]
  public string Content { get; set; } = null!;

  [JsonPropertyName("created_date")]
  public DateTime CreatedDate { get; set; }

  [JsonPropertyName("updated_date")]
  public DateTime UpdatedDate { get; set; }


  public AnnouncementResponse(Announcement announcement)
  {
    this.AnnouncementId = announcement.Id;
    this.Topic = announcement.Topic;
    this.Content = announcement.Detail;
    this.CreatedDate = announcement.CreatedDate;
    this.UpdatedDate = announcement.UpdatedDate;

  }
}