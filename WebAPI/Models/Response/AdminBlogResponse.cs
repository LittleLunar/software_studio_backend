
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class AdminBlogResponse
{
  [JsonPropertyName("blog_id")]
  public string BlogId { get; set; } = null!;

  [JsonPropertyName("author_id")]
  public string AuthorId { get; set; }

  [JsonPropertyName("topic")]
  public string Topic { get; set; } = null!;

  [JsonPropertyName("content")]
  public string Content { get; set; } = "";

  [JsonPropertyName("category")]
  public List<string> Category { get; set; } = new List<string>();

  [JsonPropertyName("like")]
  public int Like { get; set; }

  [JsonPropertyName("like_users")]
  public List<string> LikeUsers { get; set; } = null!;

  [JsonPropertyName("hide")]
  public bool Hide { get; set; }

  [JsonPropertyName("deleted")]
  public bool Deleted { get; set; }

  [JsonPropertyName("created_date")]
  public DateTime CreatedDate { get; set; }

  [JsonPropertyName("updated_date")]
  public DateTime UpdatedDate { get; set; }

  public AdminBlogResponse(Blog blog)
  {
    this.BlogId = blog.Id;
    this.AuthorId = blog.UserId;
    this.Topic = blog.Topic;
    this.Content = blog.Detail;
    this.Category = blog.Category;
    this.Like = blog.Like.Count;
    this.LikeUsers = blog.Like;
    this.Hide = blog.Hide;
    this.Deleted = blog.Deleted;
    this.CreatedDate = blog.CreatedDate;
    this.UpdatedDate = blog.UpdatedDate;
  }
}