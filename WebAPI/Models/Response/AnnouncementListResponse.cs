
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class AnnouncementListResponse
{
  [JsonPropertyName("announcements")]
  public List<AnnouncementResponse> Announcements = new List<AnnouncementResponse>();
}