using software_studio_backend.Models;
using software_studio_backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;

namespace software_studio_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnnounceController : ControllerBase
{
  private readonly MongoDBService _mongoDB;
  private readonly ILogger<AnnounceController> _logger;


  public AnnounceController(ILogger<AnnounceController> logger,MongoDBService mongoDBService)
  {
    _logger = logger;
    _mongoDB = mongoDBService;
  }

  [HttpGet]
  public async Task<IActionResult> GetAnnounce()
  {
    Announcement? announcement = await _mongoDB.AnnounceCollection.Find(_ => true).FirstOrDefaultAsync();

    if (announcement == null)
      return NotFound("No announcement.");

    return Ok(announcement);
  }


}