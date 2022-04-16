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
}
