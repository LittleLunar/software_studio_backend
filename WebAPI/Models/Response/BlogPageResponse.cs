
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class BlogPageResponse
{
  [JsonPropertyName("blog_id")]
  public string BlogId { get; set; } = null!;

  [JsonPropertyName("author")]
  public UserResponse Author { get; set; } = null!;

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

  [JsonPropertyName("comments")]
  public List<CommentResponse> Comments { get; set; } = new List<CommentResponse>();

  [JsonPropertyName("created_date")]
  public DateTime CreatedDate { get; set; }

  [JsonPropertyName("updated_date")]
  public DateTime UpdatedDate { get; set; }

  public BlogPageResponse(Blog blog, User user, List<CommentResponse> comments)
  {
    this.BlogId = blog.Id;
    this.Author = new UserResponse(user);
    this.Topic = blog.Topic;
    this.Content = blog.Detail;
    this.Category = blog.Category;
    this.Like = blog.Like.Count;
    this.LikeUsers = blog.Like;
    this.Comments = comments;
    this.CreatedDate = blog.CreatedDate;
    this.UpdatedDate = blog.UpdatedDate;
  }
}