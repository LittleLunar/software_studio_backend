using software_studio_backend.Services;
using software_studio_backend.Models;
using software_studio_backend.Utils;
using software_studio_backend.Shared;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;

namespace software_studio_backend.Controllers;

[ApiController]
[Route("auth/[controller]")]
public class SessionController : ControllerBase
{
  private readonly MongoDBService _mongoDB;
  private readonly ILogger<SessionController> _logger;

  public SessionController(ILogger<SessionController> logger, MongoDBService mongoDBService)
  {
    _logger = logger;
    _mongoDB = mongoDBService;
  }

  [AllowAnonymous]
  [HttpPost]
  [Route("login")]
  public async Task<IActionResult> Login([FromBody] AuthenRequest body)
  {

    User? user = await _mongoDB.UserCollection.Find(x => x.Username == body.username && !x.Banned && !x.Deleted).FirstOrDefaultAsync();

    if (user == null)
      return NotFound("User is not found.");

    bool IsPassCorrect = PasswordEncryption.Validate(body.password, user.Password);

    if (!IsPassCorrect)
      return Unauthorized("Username or Password is incorrect.");

    string accessToken = TokenUtils.GenerateAccessToken(user);
    string refreshToken = TokenUtils.GenerateRefreshToken(user);

    Response.Cookies.Append(Constant.Name.AccessToken, accessToken, new CookieOptions
    {
      HttpOnly = false,
      Expires = DateTime.UtcNow.AddDays(Constant.Number.AccessTokenExpiresInDay)
    });

    Response.Cookies.Append(Constant.Name.RefreshToken, refreshToken, new CookieOptions
    {
      HttpOnly = true,
      Expires = DateTime.UtcNow.AddMonths(Constant.Number.RefreshTokenExpiresInMonths)
    });

    return Ok(new { accessToken = accessToken, refreshToken = refreshToken });
  }

  [AllowAnonymous]
  [HttpPost]
  [Route("register")]
  public async Task<IActionResult> Register([FromBody] RegisterRequest body)
  {
    Console.WriteLine("Someone is registering.");

    if (body.Password != body.ConfirmPassword)
      return Unauthorized("Passwords do not match.");

    User? oldUser = await _mongoDB.UserCollection.Find(x => x.Username == body.Username).FirstOrDefaultAsync();

    if (oldUser != null)
      return Unauthorized("This username has been taken.");

    string encryptedPass = PasswordEncryption.Encrypt(body.Password);

    User newUser = new User { Username = body.Username, Password = encryptedPass, Name = body.Name };

    await _mongoDB.UserCollection.InsertOneAsync(newUser);

    return CreatedAtAction(nameof(Register), newUser);
  }

  [Authorize]
  [HttpGet]
  [Route("user")]
  public IActionResult GetSession()
  {
    Console.WriteLine("Authen User success.");
    return Ok("You are normal authorized user.");
  }

  [Authorize(Roles = "admin")]
  [HttpGet]
  [Route("admin")]
  public IActionResult GetAdminSession()
  {
    Console.WriteLine("Authen admin success.");
    return Ok(new { message = "You are an admin." });
  }

  [Authorize]
  [HttpDelete]
  [Route("logout")]
  public IActionResult Logout()
  {
    Console.WriteLine("Someone logging out.");

    Response.Cookies.Delete(Constant.Name.AccessToken);
    Response.Cookies.Delete(Constant.Name.RefreshToken);

    return NoContent();
  }

}