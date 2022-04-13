using software_studio_backend.Models;
using software_studio_backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace software_studio_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
  private readonly MongoDBService _mongoDB;

  public UserController(MongoDBService mongoDBService)
  {
    _mongoDB = mongoDBService;
  }

  [Authorize]
  [HttpGet]
  [Route("profile/{id:length(24)}")]
  public async Task<IActionResult> GetUserProfile(string id)
  {
    string username = Request.HttpContext.User.FindFirstValue(ClaimTypes.Name);

    User? user = await _mongoDB.UserCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

    if (user == null)
      return NotFound("User is not found.");

    UserResponse userResponse = new UserResponse(user);

    return Ok(userResponse);
  }

  [Authorize]
  [HttpPatch]
  [Route("resetpass")]
  public async Task<IActionResult> ResetPassword([FromBody] ResetPassRequest body)
  {
    if (body.Password != body.ConfirmPassword)
      return Unauthorized(new ErrorMessage("Password is not match."));

    string username = Request.HttpContext.User.FindFirstValue(ClaimTypes.Name);

    User? user = await _mongoDB.UserCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

    if (user == null)
      return NotFound(new ErrorMessage("User is not found."));

    user.Password = body.Password;

    await _mongoDB.UserCollection.ReplaceOneAsync(x => x.Username == username, user);

    return NoContent();
  }

  [Authorize]
  [HttpPatch]
  [Route("update")]
  public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest body)
  {
    string username = Request.HttpContext.User.FindFirstValue(ClaimTypes.Name);

    User? user = await _mongoDB.UserCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

    if (user == null)
      return NotFound(new ErrorMessage("User is not found."));

    if (body.Name != null)
      user.Name = body.Name;

    if (body.ProfileImage != null)
      user.ProfileImage = body.ProfileImage;

    return NoContent();

  }
  [Authorize(Roles = "admin")]
  [HttpDelete]
  [Route("delete/{id:length(24)}")]
  public async Task<IActionResult> DeleteUser(string id)
  {
    User? user = await _mongoDB.UserCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (user == null)
      return NotFound(new ErrorMessage("User is not found."));

    await _mongoDB.CommentCollection.DeleteManyAsync(x => x.UserId == id);

    List<Blog> blogs = await _mongoDB.BlogCollection.Find(x => x.Like.Contains(user.Username)).ToListAsync();

    foreach (Blog blog in blogs)
      blog.Like.Remove(user.Username);

    List<Comment> comments = await _mongoDB.CommentCollection.Find(x => x.Like.Contains(user.Username)).ToListAsync();

    foreach (Comment comment in comments)
      comment.Like.Remove(user.Username);

    await _mongoDB.UserCollection.DeleteOneAsync(x => x.Id == id);

    return NoContent();
  }

  [Authorize(Roles = "admin")]
  [HttpDelete]
  [Route("banned/{id:length(24)}")]
  public async Task<IActionResult> BanUser(string id)
  {
    User? user = await _mongoDB.UserCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (user == null)
      return NotFound(new ErrorMessage("User is not found."));

    user.Banned = true;
    user.Active = false;

    await _mongoDB.UserCollection.ReplaceOneAsync(x => x.Id == id, user);

    return NoContent();
  }
}