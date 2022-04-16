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
  [HttpPost]
  [Route("create")]
  public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest body)
  {
    string username = Request.HttpContext.User.FindFirstValue(ClaimTypes.Name);

    User? author = await _mongoDB.UserCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

    if (author == null)
      return NotFound(new ErrorMessage("User is not found."));

    Comment newComment = new Comment { Detail = body.Content, ContentId = body.ContentId, UserId = author.Id };

    return CreatedAtAction(nameof(CreateComment), newComment);
  }

  [Authorize]
  [HttpPatch]
  [Route("update/{id:length(24)}")]
  public async Task<IActionResult> UpdateComment(string id, [FromBody] EditContentRequest body)
  {
    string username = Request.HttpContext.User.FindFirstValue(ClaimTypes.Name);

    User? author = await _mongoDB.UserCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

    if (author == null)
      return NotFound(new ErrorMessage("User is not found"));

    Comment? comment = await _mongoDB.CommentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (comment == null)
      return NotFound(new ErrorMessage("Comment is not found."));

    if (author.Id != comment.UserId)
      return Unauthorized(new ErrorMessage("You are not the author of this comment."));

    comment.Detail = body.Content;
    comment.UpdatedDate = DateTime.Now;

    return Ok(comment);
  }

  [Authorize]
  [HttpPatch]
  [Route("like/{id:length(24)}")]
  public async Task<IActionResult> LikeComment(string id)
  {
    string username = Request.HttpContext.User.FindFirstValue(ClaimTypes.Name);

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
    string username = Request.HttpContext.User.FindFirstValue(ClaimTypes.Name);

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
