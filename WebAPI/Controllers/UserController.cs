using software_studio_backend.Models;
using software_studio_backend.Services;
using Microsoft.AspNetCore.Mvc;
using software_studio_backend.Utils;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace software_studio_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
  private readonly MongoDBService _mongoDB;

  public UserController(MongoDBService mongoDBService)
  {
    _mongoDB = mongoDBService;
  }

  [HttpGet]
  [Route("profile")]
  public async Task<IActionResult> GetUserProfile()
  {
    string username = Request.HttpContext.User.FindFirstValue("username");

    User? user = await _mongoDB.UserCollection.Find(x => x.Username == username && !x.Banned && !x.Deleted).FirstOrDefaultAsync();

    if (user == null)
      return NotFound("User is not found.");

    UserResponse userResponse = new UserResponse(user);

    return Ok(userResponse);
  }

  [HttpPatch]
  [Route("resetpass")]
  public async Task<IActionResult> ResetPassword([FromBody] ResetPassRequest body)
  {
    if (body.Password != body.ConfirmPassword)
      return Unauthorized("Passwords do not match.");

    string username = Request.HttpContext.User.FindFirstValue("username");

    User? user = await _mongoDB.UserCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

    if (user == null)
      return NotFound("User is not found.");

    string encryptedPass = PasswordEncryption.Encrypt(body.Password);

    if (encryptedPass == user.Password)
      return BadRequest("New password is the same as current password.");

    user.Password = encryptedPass;

    await _mongoDB.UserCollection.ReplaceOneAsync(x => x.Username == username, user);

    return NoContent();
  }

  [HttpPatch]
  [Route("update")]
  public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest body)
  {
    string username = Request.HttpContext.User.FindFirstValue("username");

    User? user = await _mongoDB.UserCollection.Find(x => x.Username == username && !x.Banned && !x.Deleted).FirstOrDefaultAsync();

    if (user == null)
      return NotFound("User is not found.");

    user.Name = body.Name ?? user.Name;
    user.ProfileImage = body.ProfileImage ?? user.ProfileImage;
    user.UpdatedDate = DateTime.Now;

    await _mongoDB.UserCollection.ReplaceOneAsync(x => x.Id == id, user);

    return NoContent();

  }

}