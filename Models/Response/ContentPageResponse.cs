
namespace software_studio_backend.Models;

public class ContentPageResponse
{
  public ContentResponse Content { get; set; } = null!;

  public List<CommentResponse>? Comments { get; set; }
}