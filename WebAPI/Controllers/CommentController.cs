using software_studio_backend.Models;
using software_studio_backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

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

  [HttpGet]
  public async Task<List<Comment>> Get() => await _mongoDB.CommentCollection.Find(_ => true).ToListAsync();

  [HttpGet("{id:length(24)}")]
  public async Task<ActionResult<Comment>> Get(string id)
  {
    Comment? comment = await _mongoDB.CommentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (comment is null)
    {
      return NotFound();
    }

    return comment;
  }

  [HttpPost]
  public async Task<IActionResult> Post(Comment newComment)
  {
    await _mongoDB.CommentCollection.InsertOneAsync(newComment);

    return CreatedAtAction(nameof(Get), new { id = newComment.Id }, newComment);

  }

  [HttpPatch]
  [Route("{id:length(24)}/like")]
  public async Task<IActionResult> Like(string id, [FromBody] LikeRequest likeUser)
  {
    // Console.WriteLine(username);
    Comment? comment = await _mongoDB.CommentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (comment == null) return NotFound("Content not found");

    if (comment.Like.Contains(likeUser.username))
      comment.Like.Remove(likeUser.username);
    else
      comment.Like.Add(likeUser.username);

    await _mongoDB.CommentCollection.ReplaceOneAsync(x => x.Id == id, comment);

    return Ok(comment);

  }

  [HttpDelete("{id:length(24)}")]
  public async Task<IActionResult> Delete(string id)
  {
    Comment? comment = await _mongoDB.CommentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (comment is null)
    {
      return NotFound();
    }

    await _mongoDB.CommentCollection.DeleteOneAsync(x => x.Id == id);

    return NoContent();
  }
  [HttpDelete]
  public async Task<IActionResult> Delete()
  {

    await _mongoDB.CommentCollection.DeleteManyAsync(_ => true);

    return NoContent();
  }
}