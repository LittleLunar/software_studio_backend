using Microsoft.AspNetCore.Mvc;
using software_studio_backend.Models;
using software_studio_backend.Services;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace software_studio_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PantipController : ControllerBase
{
  private readonly MongoDBService _mongoDB;

  public PantipController(MongoDBService mongoDBService)
  {
    _mongoDB = mongoDBService;
  }

  [AllowAnonymous]
  [HttpGet]
  [Route("list")]
  public async Task<IActionResult> GetPantips() // All pantips page
  {
    List<Pantip> pantips = await _mongoDB.PantipCollection.Find(_ => true).ToListAsync();

    List<PantipResponse> pantipResponses = new List<PantipResponse>();

    foreach (Pantip pantip in pantips)
    {
      User author = await _mongoDB.UserCollection.Find(x => x.Id == pantip.UserId).FirstAsync();
      PantipResponse pantipResponse = new PantipResponse(pantip, author);
      pantipResponses.Add(pantipResponse);
    }

    PantipListResponse pantipList = new PantipListResponse { Pantips = pantipResponses.OrderByDescending(x => x.CreatedDate).ToList() };

    return Ok(pantipList);
  }

  [AllowAnonymous]
  [HttpGet]
  [Route("{id:length(24)}")]

  public async Task<IActionResult> GetPantip(string id) // Individual pantip page with its comments.
  {
    Pantip? pantip = await _mongoDB.PantipCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (pantip == null)
      return NotFound(new ErrorMessage("Pantip is not found"));

    User author = await _mongoDB.UserCollection.Find(x => x.Id == pantip.UserId).FirstAsync();

    if (author == null)
      return NotFound(new ErrorMessage("Author is not found"));

    List<Comment> commentsInPantip = await _mongoDB.CommentCollection.Find(x => x.ContentId == id).ToListAsync();

    List<CommentResponse> comments = new List<CommentResponse>();

    foreach (Comment comment in commentsInPantip)
    {
      User authorComment = await _mongoDB.UserCollection.Find(x => x.Id == comment.UserId).FirstAsync();
      CommentResponse commentResponse = new CommentResponse(comment, authorComment);
      comments.Add(commentResponse);
    }

    PantipResponse pantipResponse = new PantipResponse(pantip, author, comments);

    return Ok(pantipResponse);

  }

  [Authorize]
  [HttpPost]
  [Route("create")]
  public async Task<IActionResult> CreatePantip([FromBody] CreatePantipRequest body)
  {
    string? username = Request.HttpContext.User.FindFirstValue(ClaimTypes.Name);

    if (String.IsNullOrEmpty(username))
      return Unauthorized(new ErrorMessage("You are not authorized user."));

    User user = await _mongoDB.UserCollection.Find(x => x.Username == username).FirstAsync();

    Pantip newPantip = new Pantip { Topic = body.Topic, Detail = body.Content, UserId = user.Id };

    await _mongoDB.PantipCollection.InsertOneAsync(newPantip);

    return CreatedAtAction(nameof(CreatePantip), newPantip);

  }

  [Authorize]
  [HttpPatch]
  [Route("update/{id:length(24)}")]
  public async Task<IActionResult> UpdatePantip(string id, [FromBody] EditContentRequest body)
  {
    string? username = Request.HttpContext.User.FindFirstValue(ClaimTypes.Name);

    if (String.IsNullOrEmpty(username))
      return Unauthorized(new ErrorMessage("You are not authorized user."));

    User? user = await _mongoDB.UserCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

    if (user == null)
      return NotFound(new ErrorMessage("User is not found."));

    Pantip? pantip = await _mongoDB.PantipCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (pantip == null)
      return NotFound(new ErrorMessage("Pantip is not found."));

    if (pantip.UserId != user.Id)
      return Unauthorized(new ErrorMessage("User is not found"));

    pantip.Detail = body.Content;
    pantip.UpdatedDate = DateTime.Now;

    await _mongoDB.PantipCollection.ReplaceOneAsync(x => x.Id == id, pantip);

    return Ok(pantip);
  }

  [Authorize]
  [HttpPatch]
  [Route("like/{id:length(24)}")]
  public async Task<IActionResult> LikePantip(string id)
  {
    Pantip? pantip = await _mongoDB.PantipCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (pantip == null)
      return NotFound(new ErrorMessage("Pantip is not found."));

    string? username = Request.HttpContext.User.FindFirstValue(ClaimTypes.Name);

    if (String.IsNullOrEmpty(username))
      return Unauthorized(new ErrorMessage("You are not authorized user."));

    if (pantip.Like.Contains(username))
      pantip.Like.Remove(username);
    else
      pantip.Like.Add(username);

    await _mongoDB.PantipCollection.ReplaceOneAsync(x => x.Id == id, pantip);

    return Ok(pantip);

  }

  [Authorize]
  [HttpDelete]
  [Route("delete/{id:length(24)}")]
  public async Task<IActionResult> DeletePantip(string id)
  {
    Pantip? pantip = await _mongoDB.PantipCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (pantip == null)
      return NotFound(new ErrorMessage("Pantip is not Found."));

    await _mongoDB.PantipCollection.DeleteOneAsync(x => x.Id == id);
    await _mongoDB.CommentCollection.DeleteManyAsync(x => x.ContentId == id);


    return NoContent();
  }

}