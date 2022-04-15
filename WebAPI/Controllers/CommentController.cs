using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using software_studio_backend.Models;
using software_studio_backend.Services;
namespace software_studio_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
  private readonly MongoDBService _mongoDB;

  public CommentController(MongoDBService mongoDBService)
  {
    _mongoDB = mongoDBService;
  }

  [Authorize]
  [HttpPost]
  [Route("create")]
  public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest body)
  {
    string username = Request.HttpContext.User.FindFirstValue(ClaimTypes.Name);

    User author = await _mongoDB.UserCollection.Find(x => x.Username == username).FirstAsync();

    Comment newComment = new Comment { Detail = body.Content, UserId = author.Id, ContentId = body.ContentId };

    await _mongoDB.CommentCollection.InsertOneAsync(newComment);

    return CreatedAtAction(nameof(CreateComment), newComment);
  }

  [Authorize]
  [HttpPatch]
  [Route("update/{id:length(24)}")]
  public async Task<IActionResult> UpdateComment(string id, [FromBody] EditContentRequest body)
  {
    Comment? updatedComment = await _mongoDB.CommentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (updatedComment == null)
      return NotFound(new ErrorMessage("Comment is not found."));

    string username = Request.HttpContext.User.FindFirstValue(ClaimTypes.Name);

    User? author = await _mongoDB.UserCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

    if (author == null)
      return NotFound(new ErrorMessage("User is not found."));

    if (author.Id != updatedComment.UserId)
      return Unauthorized(new ErrorMessage("You are not the author of this comment."));

    updatedComment.Detail = body.Content;
    updatedComment.UpdatedDate = DateTime.Now;

    await _mongoDB.CommentCollection.ReplaceOneAsync(x => x.Id == updatedComment.Id, updatedComment);

    return Ok(updatedComment);
  }

  [Authorize]
  [HttpPatch]
  [Route("like/{id:length(24)}")]
  public async Task<IActionResult> LikeComment(string id)
  {
    Comment? comment = await _mongoDB.CommentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (comment == null)
      return NotFound("Blog is not found.");

    string? username = Request.HttpContext.User.FindFirstValue(ClaimTypes.Name);

    if (String.IsNullOrEmpty(username))
      return Unauthorized(new ErrorMessage("You are not authorized user."));

    if (comment.Like.Contains(username))
      comment.Like.Remove(username);
    else
      comment.Like.Add(username);

    await _mongoDB.CommentCollection.ReplaceOneAsync(x => x.Id == id, comment);

    return Ok(comment);

  }

  [Authorize]
  [HttpDelete]
  [Route("delete/{id:length(24)")]
  public async Task<IActionResult> DeleteComment(string id)
  {
    Comment? comment = await _mongoDB.CommentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (comment == null)
      return NotFound(new ErrorMessage("Comment is not found."));

    string username = Request.HttpContext.User.FindFirstValue(ClaimTypes.Name);

    User? author = await _mongoDB.UserCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

    if (author == null)
      return NotFound(new ErrorMessage("User is not found."));

    if (author.Id != comment.Id)
      return Unauthorized(new ErrorMessage("You are not the author of this comment."));

    await _mongoDB.CommentCollection.DeleteOneAsync(x => x.Id == id);

    return NoContent();
  }
}
