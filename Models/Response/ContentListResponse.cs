namespace software_studio_backend.Models;

public class ContentListResponse
{
  public List<ContentResponse> Contents { get; set; } = new List<ContentResponse>();

  // public List<CommentResponse>? Comments { get; set; }

}