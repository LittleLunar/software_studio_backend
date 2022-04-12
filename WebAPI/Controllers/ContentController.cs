using Microsoft.AspNetCore.Mvc;
using software_studio_backend.Models;
using software_studio_backend.Services;
using MongoDB.Driver;
using System.Dynamic;

namespace software_studio_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentController : ControllerBase
{
  private readonly MongoDBService _mongoDB;

  public ContentController(MongoDBService mongoDBService)
  {
    _mongoDB = mongoDBService;
  }

  [HttpGet]
  public async Task<IActionResult> Get()
  {
    List<Content> contents = await _mongoDB.ContentCollection.Find(_ => true).ToListAsync();

    ContentListResponse contentListResponse = new ContentListResponse();
    foreach (Content content in contents)
    {
      ContentResponse contentResponse = new ContentResponse(content);
      User ownerContent = await _mongoDB.UserCollection.Find(x => x.Id == content.UserId).FirstAsync();
      UserResponse userResponse = new UserResponse(ownerContent);

      contentResponse.Owner = userResponse;

      contentListResponse.Contents.Add(contentResponse);
    }
    return Ok(contentListResponse);
  }

  [HttpGet]
  [Route("{id:length(24)}")]

  public async Task<IActionResult> Get(string id)
  {
    Content? content = await _mongoDB.ContentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (content is null)
    {
      return NotFound();
    }

    User contentOwner = await _mongoDB.UserCollection.Find(x => x.Id == content.UserId).FirstAsync();

    ContentResponse contentResponse = new ContentResponse(content);
    UserResponse userResponse = new UserResponse(contentOwner);
    contentResponse.Owner = userResponse;

    List<Comment>? comments = await _mongoDB.CommentCollection.Find(x => x.ContentId == content.Id).ToListAsync();
    List<CommentResponse> commentList = new List<CommentResponse>();
    foreach (Comment comment in comments)
    {
      CommentResponse commentResponse = new CommentResponse(comment);
      User owner = await _mongoDB.UserCollection.Find(x => x.Id == comment.UserId).FirstAsync();
      UserResponse ownerResponse = new UserResponse(owner);
      commentResponse.Owner = ownerResponse;

      commentList.Add(commentResponse);
    }

    ContentPageResponse data = new ContentPageResponse { Content = contentResponse, Comments = commentList };

    return Ok(data);
  }

  [HttpPost]
  public async Task<IActionResult> Post([FromBody] Content newContent)
  {
    await _mongoDB.ContentCollection.InsertOneAsync(newContent);

    return CreatedAtAction(nameof(Get), new { id = newContent.Id }, newContent);

  }

  [HttpPatch]
  [Route("{id:length(24)}/like")]
  public async Task<IActionResult> Like(string id, [FromBody] LikeRequest likeUser)
  {
    // Console.WriteLine(username);
    Content? content = await _mongoDB.ContentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (content == null) return NotFound("Content not found");

    if (content.Like.Contains(likeUser.username))
      content.Like.Remove(likeUser.username);
    else
      content.Like.Add(likeUser.username);

    await _mongoDB.ContentCollection.ReplaceOneAsync(x => x.Id == id, content);

    return Ok(content);

  }

  [HttpDelete]
  [Route("{id:length(24)}")]
  public async Task<IActionResult> Delete(string id)
  {
    Content? content = await _mongoDB.ContentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (content is null)
    {
      return NotFound();
    }

    await _mongoDB.ContentCollection.DeleteOneAsync(x => x.Id == id);

    return NoContent();
  }

  [HttpDelete]
  public async Task<IActionResult> Delete()
  {

    await _mongoDB.ContentCollection.DeleteManyAsync(_ => true);

    return NoContent();
  }
}