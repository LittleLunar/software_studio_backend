using Microsoft.AspNetCore.Mvc;
using software_studio_backend.Models;
using software_studio_backend.Services;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace software_studio_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnnouncementController : ControllerBase
{
  private readonly MongoDBService _mongoDB;

  public AnnouncementController(MongoDBService mongoDBService)
  {
    _mongoDB = mongoDBService;
  }

  [AllowAnonymous]
  [HttpGet]
  public async Task<IActionResult> GetAnnouncements() // All announcements page
  {
    List<Announcement> announcements = await _mongoDB.AnnouncementCollection.Find(_ => true).ToListAsync();

    List<AnnouncementResponse> announcementResponses = new List<AnnouncementResponse>();

    foreach (Announcement announcement in announcements)
    {
      AnnouncementResponse announcementResponse = new AnnouncementResponse(announcement);
      announcementResponses.Add(announcementResponse);
    }

    AnnouncementListResponse announcementList = new AnnouncementListResponse { Announcements = announcementResponses };

    return Ok(announcementList);
  }

  [AllowAnonymous]
  [HttpGet]
  [Route("{id:length(24)}")]

  public async Task<IActionResult> GetAnnouncement(string id) // Individual announcement page with its comments.
  {
    Announcement? announcement = await _mongoDB.AnnouncementCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (announcement == null)
      return NotFound(new ErrorMessage("Announcement is not found"));

    User author = await _mongoDB.UserCollection.Find(x => x.Id == announcement.UserId).FirstAsync();

    if (author == null)
      return NotFound(new ErrorMessage("Author is not found"));

    AnnouncementResponse announcementResponse = new AnnouncementResponse(announcement);

    return Ok(announcementResponse);

  }

  [Authorize(Roles = "admin")]
  [HttpPost]
  [Route("create")]
  public async Task<IActionResult> CreateAnnouncement([FromBody] CreateAnnounceRequest body)
  {
    string? username = Request.HttpContext.User.FindFirstValue(ClaimTypes.Name);

    if (String.IsNullOrEmpty(username))
      return Unauthorized(new ErrorMessage("You are not authorized user."));

    User user = await _mongoDB.UserCollection.Find(x => x.Username == username).FirstAsync();

    Announcement newAnnouncement = new Announcement { Topic = body.Topic, Detail = body.Content, UserId = user.Id };

    await _mongoDB.AnnouncementCollection.InsertOneAsync(newAnnouncement);

    return CreatedAtAction(nameof(CreateAnnouncement), newAnnouncement);

  }

  [Authorize(Roles = "admin")]
  [HttpPatch]
  [Route("update/{id:length(24)}")]
  public async Task<IActionResult> UpdateAnnouncement(string id, [FromBody] EditContentRequest body)
  {
    Announcement? announcement = await _mongoDB.AnnouncementCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (announcement == null)
      return NotFound("Announcement is not found.");

    announcement.Detail = body.Content;
    announcement.UpdatedDate = DateTime.Now;

    await _mongoDB.AnnouncementCollection.ReplaceOneAsync(x => x.Id == id, announcement);

    return Ok(announcement);
  }

  // [Authorize]
  // [HttpPatch]
  // [Route("{id:length(24)}/like")]
  // public async Task<IActionResult> LikeAnnouncement(string id)
  // {
  //   Announcement? announcement = await _mongoDB.AnnouncementCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

  //   if (announcement == null)
  //     return NotFound("Announcement is not found.");

  //   string? username = Request.HttpContext.User.FindFirstValue(ClaimTypes.Name);

  //   if (String.IsNullOrEmpty(username))
  //     return Unauthorized(new ErrorMessage("You are not authorized user."));

  //   if (announcement.Like.Contains(username))
  //     announcement.Like.Remove(username);
  //   else
  //     announcement.Like.Add(username);

  //   await _mongoDB.AnnouncementCollection.ReplaceOneAsync(x => x.Id == id, announcement);

  //   return Ok(announcement);

  // }

  [Authorize(Roles = "admin")]
  [HttpDelete]
  [Route("delete/{id:length(24)}")]
  public async Task<IActionResult> DeleteAnnouncement(string id)
  {
    Announcement? announcement = await _mongoDB.AnnouncementCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (announcement is null)
      return NotFound(new ErrorMessage("Announcement is not Found."));

    await _mongoDB.AnnouncementCollection.DeleteOneAsync(x => x.Id == id);

    return NoContent();
  }

}