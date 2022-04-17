using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using software_studio_backend.Shared;
using software_studio_backend.Services;
using software_studio_backend.Models;
using software_studio_backend.Utils;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
  [HttpGet]
  [Route("list")]
  public async Task<IActionResult> GetComments()
  {
    List<Comment> comments = await _mongoDB.CommentCollection.Find(_ => true).ToListAsync();

    List<CommentResponse> commentResponses = new List<CommentResponse>();

    foreach (Comment comment in comments)
    {
      User user = await _mongoDB.UserCollection.Find(x => x.Id == comment.UserId).FirstAsync();
      CommentResponse commentResponse = new CommentResponse(comment, user);
      commentResponses.Add(commentResponse);
    }

    CommentListResponse commentList = new CommentListResponse { Comments = commentResponses.OrderByDescending(x => x.CreatedDate).ToList() };

    return Ok(commentList);
  }

  [Authorize]
  [HttpPost]
  [Route("create")]
  public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest body)
  {
    string? username = Request.HttpContext.User.FindFirstValue("username");

    if (String.IsNullOrEmpty(username))
      return Unauthorized(new ErrorMessage("You are not authorized user."));

    User? author = await _mongoDB.UserCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

    if (author == null)
      return NotFound(new ErrorMessage("User is not found."));

    Comment newComment = new Comment { Detail = body.Content, ContentId = body.ContentId, UserId = author.Id };

    await _mongoDB.CommentCollection.InsertOneAsync(newComment);

    return CreatedAtAction(nameof(CreateComment), newComment);
  }

  [Authorize]
  [HttpPatch]
  [Route("update/{id:length(24)}")]
  public async Task<IActionResult> UpdateComment(string id, [FromBody] EditContentRequest body)
  {
    string? username = Request.HttpContext.User.FindFirstValue("username");

    if (String.IsNullOrEmpty(username))
      return Unauthorized(new ErrorMessage("You are not authorized user."));

    User? author = await _mongoDB.UserCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

    if (author == null)
      return NotFound(new ErrorMessage("User is not found"));

    Comment? updatedComment = await _mongoDB.CommentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (updatedComment == null)
      return NotFound(new ErrorMessage("Comment is not found."));

    if (author.Id != updatedComment.UserId)
      return Unauthorized(new ErrorMessage("You are not the author of this comment."));

    updatedComment.Detail = body.Content;
    updatedComment.UpdatedDate = DateTime.Now;

    await _mongoDB.CommentCollection.ReplaceOneAsync(x => x.Id == id, updatedComment);

    return Ok(updatedComment);
  }

  [Authorize]
  [HttpPatch]
  [Route("like/{id:length(24)}")]
  public async Task<IActionResult> LikeComment(string id)
  {
    string? username = Request.HttpContext.User.FindFirstValue("username");

    if (String.IsNullOrEmpty(username))
      return Unauthorized(new ErrorMessage("You are not authorized user."));

    Comment? comment = await _mongoDB.CommentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (comment == null)
      return NotFound(new ErrorMessage("Comment is not found."));

    if (comment.Like.Contains(username))
      comment.Like.Remove(username);
    else
      comment.Like.Add(username);

    await _mongoDB.CommentCollection.ReplaceOneAsync(x => x.Id == id, comment);

    return Ok(comment);

  }

  [Authorize]
  [HttpDelete]
  [Route("delete/{id:length(24)}")]
  public async Task<IActionResult> DeleteComment(string id)
  {
    string? username = Request.HttpContext.User.FindFirstValue("username");

    if (String.IsNullOrEmpty(username))
      return Unauthorized(new ErrorMessage("You are not authorized user."));

    User? user = await _mongoDB.UserCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

    if (user == null)
      return NotFound(new ErrorMessage("User is not found."));

    Comment? comment = await _mongoDB.CommentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (comment == null)
      return NotFound(new ErrorMessage("Comment is not found."));

    if (comment.UserId != user.Id)
      return Unauthorized(new ErrorMessage("You are not the author of this comment."));

    await _mongoDB.CommentCollection.DeleteOneAsync(x => x.Id == id);

    return NoContent();
  }

  [Authorize(Roles = "admin")]
  [HttpDelete]
  [Route("admin/delete/{id:length(24)}")]
  public async Task<IActionResult> AdminDeleteComment(string id)
  {
    Comment? comment = await _mongoDB.CommentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (comment == null)
      return NotFound(new ErrorMessage("Comment is not found."));

    await _mongoDB.CommentCollection.DeleteOneAsync(x => x.Id == id);

    return NoContent();

  }

}
