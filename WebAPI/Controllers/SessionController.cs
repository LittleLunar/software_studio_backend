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

  public SessionController(MongoDBService mongoDBService)
  {
    _mongoDB = mongoDBService;
  }

  [AllowAnonymous]
  [HttpPost]
  [Route("login")]
  public async Task<IActionResult> Login([FromBody] AuthenRequest request)
  {
    Console.WriteLine("Someone is logging in.");

    User? user = await _mongoDB.UserCollection.Find(x => x.Username == request.username).FirstOrDefaultAsync();

    if (user == null) return NotFound();

    if (request.password != user!.Password) return Unauthorized();

    string accessToken = TokenUtils.GenerateAccessToken(user);
    string refreshToken = TokenUtils.GenerateRefreshToken(user);

    Response.Cookies.Append(Constant.Name.AccessToken, accessToken, new CookieOptions
    {
      HttpOnly = false,
      Expires = DateTime.Now.AddSeconds(Constant.Number.AccessTokenExpiresInSec)
    });

    Response.Cookies.Append(Constant.Name.RefreshToken, refreshToken, new CookieOptions
    {
      HttpOnly = true,
      Expires = DateTime.Now.AddMonths(Constant.Number.RefreshTokenExpiresInMonths)
    });

    return Ok(new { accessToken = accessToken, refreshToken = refreshToken });
  }

  [AllowAnonymous]
  [HttpPost]
  [Route("register")]
  public async Task<IActionResult> Register([FromBody] RegisterRequest request)
  {
    Console.WriteLine("Someone is registering.");

    if (request.Password != request.ConfirmPassword) return Unauthorized("Passwords are not match.");

    User? oldUser = await _mongoDB.UserCollection.Find(x => x.Username == request.Username).FirstOrDefaultAsync();

    if (oldUser != null) return Unauthorized("Username is taken.");

    User newUser = new User { Username = request.Username, Password = request.Password };

    await _mongoDB.UserCollection.InsertOneAsync(newUser);

    List<User> users = await _mongoDB.UserCollection.Find(_ => true).ToListAsync();

    return Ok(users.OrderByDescending(x => x.Created_date).ToList());
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