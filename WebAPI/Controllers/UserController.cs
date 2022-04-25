using software_studio_backend.Models;
using software_studio_backend.Services;
using Microsoft.AspNetCore.Mvc;
using software_studio_backend.Utils;
using MongoDB.Driver;
using software_studio_backend.Shared;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace software_studio_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
  private readonly MongoDBService _mongoDB;
  private readonly ILogger<UserController> _logger;

  public UserController(ILogger<UserController> logger, MongoDBService mongoDBService)
  {
    _logger = logger;
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
  public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest body)
  {
    string username = Request.HttpContext.User.FindFirstValue("username");

    User? user = await _mongoDB.UserCollection.Find(x => x.Username == username && !x.Banned && !x.Deleted).FirstOrDefaultAsync();

    if (user == null)
      return NotFound("User is not found.");

    user.Name = body.Name ?? user.Name;
    user.ProfileImage = body.ProfileImage ?? user.ProfileImage;
    user.UpdatedDate = DateTime.UtcNow;

    await _mongoDB.UserCollection.ReplaceOneAsync(x => x.Id == user.Id, user);

    return Ok(user);

  }

  [HttpDelete]
  [Route("delete")]
  public async Task<IActionResult> DeleteUser()
  {
    string username = Request.HttpContext.User.FindFirstValue("username");

    User? user = await _mongoDB.UserCollection.Find(x => x.Username == username && !x.Banned && !x.Deleted).FirstOrDefaultAsync();

    if (user == null)
      return NotFound("User is not found.");

    user.Banned = !user.Deleted;
    user.Deleted = !user.Deleted;

    Response.Cookies.Delete(Constant.Name.AccessToken);
    Response.Cookies.Delete(Constant.Name.RefreshToken);

    await _mongoDB.UserCollection.ReplaceOneAsync(x => x.Id == user.Id, user);

    return Ok(user);

  }

}