using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class ContentResponse
{
  public string Id { get; set; } = null!;

  public string Detail { get; set; } = "";

  public List<string> Like { get; set; } = null!;

  public UserResponse Owner { get; set; } = null!;

  [JsonPropertyName("post_date")]
  public DateTime PostDate { get; set; }

  public ContentResponse(Content content)
  {
    this.Id = content.Id;
    this.Detail = content.Detail;
    this.Like = content.Like;
    this.PostDate = content.CreatedDate;
  }
}