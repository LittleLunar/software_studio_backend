using System.Text.Json.Serialization;

namespace software_studio_backend.Models;
public class CommentResponse
{
  public string Id { get; set; } = null!;

  public string Detail { get; set; } = "";

  public List<string> Like { get; set; } = new List<string>();

  public UserResponse Owner { get; set; } = null!;

  [JsonPropertyName("post_date")]
  public DateTime PostDate { get; set; }

  public CommentResponse(Comment comment)
  {
    this.Id = comment.Id;
    this.Detail = comment.Detail;
    this.Like = comment.Like;
    this.PostDate = comment.CreatedDate;
  }
}