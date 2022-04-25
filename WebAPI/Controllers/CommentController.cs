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
[Authorize]
public class CommentController : ControllerBase
{
  private readonly MongoDBService _mongoDB;
  private readonly ILogger<CommentController> _logger;

  public CommentController(ILogger<CommentController> logger, MongoDBService mongoDBService)
  {
    _logger = logger;
    _mongoDB = mongoDBService;
  }

  [HttpPost]
  [Route("create")]
  public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest body)
  {
    string? username = Request.HttpContext.User.FindFirstValue("username");

    if (String.IsNullOrEmpty(username))
      return Unauthorized("You are not authorized user.");

    User? author = await _mongoDB.UserCollection.Find(x => x.Username == username && !x.Banned && !x.Deleted).FirstOrDefaultAsync();

    if (author == null)
      return NotFound("User is not found.");

    Comment newComment = new Comment { Detail = body.Content, ContentId = body.ContentId, UserId = author.Id };

    await _mongoDB.CommentCollection.InsertOneAsync(newComment);

    return CreatedAtAction(nameof(CreateComment), newComment);
  }

  [HttpPatch]
  [Route("update/{id:length(24)}")]
  public async Task<IActionResult> UpdateComment(string id, [FromBody] EditContentRequest body)
  {
    string? username = Request.HttpContext.User.FindFirstValue("username");

    if (String.IsNullOrEmpty(username))
      return Unauthorized("You are not authorized user.");

    User? author = await _mongoDB.UserCollection.Find(x => x.Username == username && !x.Banned && !x.Deleted).FirstOrDefaultAsync();

    if (author == null)
      return NotFound("User is not found");

    Comment? comment = await _mongoDB.CommentCollection.Find(x => x.Id == id && !x.Delete).FirstOrDefaultAsync();

    if (comment == null)
      return NotFound("Comment is not found.");

    if (author.Id != comment.UserId)
      return Unauthorized("You are not the author of this comment.");

    comment.Detail = body.Content ?? comment.Detail;
    comment.UpdatedDate = DateTime.UtcNow;

    await _mongoDB.CommentCollection.ReplaceOneAsync(x => x.Id == id, comment);

    return Ok(comment);
  }

  [HttpPatch]
  [Route("like/{id:length(24)}")]
  public async Task<IActionResult> LikeComment(string id)
  {
    string? username = Request.HttpContext.User.FindFirstValue("username");

    if (String.IsNullOrEmpty(username))
      return Unauthorized("You are not authorized user.");

    Comment? comment = await _mongoDB.CommentCollection.Find(x => x.Id == id && !x.Delete).FirstOrDefaultAsync();

    if (comment == null)
      return NotFound("Comment is not found.");

    if (comment.Like.Contains(username))
      comment.Like.Remove(username);
    else
      comment.Like.Add(username);

    await _mongoDB.CommentCollection.ReplaceOneAsync(x => x.Id == id, comment);

    return Ok(comment);

  }

  [HttpDelete]
  [Route("delete/{id:length(24)}")]
  public async Task<IActionResult> DeleteComment(string id)
  {
    string? username = Request.HttpContext.User.FindFirstValue("username");

    if (String.IsNullOrEmpty(username))
      return Unauthorized("You are not authorized user.");

    User? user = await _mongoDB.UserCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

    if (user == null)
      return NotFound("User is not found.");

    Comment? comment = await _mongoDB.CommentCollection.Find(x => x.Id == id && !x.Delete).FirstOrDefaultAsync();

    if (comment == null)
      return NotFound("Comment is not found.");

    if (comment.UserId != user.Id)
      return Unauthorized("You are not the author of this comment.");

    comment.Delete = true;

    await _mongoDB.CommentCollection.ReplaceOneAsync(x => x.Id == id, comment);

    return NoContent();
  }

}
