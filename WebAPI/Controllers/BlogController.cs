using Microsoft.AspNetCore.Mvc;
using software_studio_backend.Models;
using software_studio_backend.Services;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace software_studio_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BlogController : ControllerBase
{
  private readonly MongoDBService _mongoDB;

  public BlogController(MongoDBService mongoDBService)
  {
    _mongoDB = mongoDBService;
  }

  [AllowAnonymous]
  [HttpGet]
  [Route("list")]
  public async Task<IActionResult> GetBlogs() // All blogs page
  {
    List<Blog> blogs = await _mongoDB.BlogCollection.Find(x => !x.Hide && !x.Deleted).ToListAsync();

    List<BlogResponse> blogResponses = new List<BlogResponse>();

    foreach (Blog blog in blogs)
    {
      User? user = await _mongoDB.UserCollection.Find(x => x.Id == blog.UserId && !x.Banned && !x.Deleted).FirstOrDefaultAsync();
      BlogResponse blogResponse = new BlogResponse(blog, user, null);
      blogResponses.Add(blogResponse);
    }

    BlogListResponse blogList = new BlogListResponse { Blogs = blogResponses.OrderByDescending(x => x.CreatedDate).ToList() };

    return Ok(blogList);
  }

  [AllowAnonymous]
  [HttpGet]
  [Route("{id:length(24)}")]
  public async Task<IActionResult> GetBlog(string id) // Individual blog page with its comments.
  {
    Blog? blog = await _mongoDB.BlogCollection.Find(x => x.Id == id && !x.Hide && !x.Deleted).FirstOrDefaultAsync();

    if (blog == null)
      return NotFound("Blog is not found");

    User? author = await _mongoDB.UserCollection.Find(x => x.Id == blog.UserId && !x.Banned && !x.Deleted).FirstOrDefaultAsync();

    List<Comment> comments = await _mongoDB.CommentCollection.Find(x => x.ContentId == blog.Id && !x.Delete).ToListAsync();

    List<CommentResponse> commentResponses = new List<CommentResponse>();

    foreach (Comment comment in comments)
    {
      User? user = await _mongoDB.UserCollection.Find(x => x.Id == comment.UserId && !x.Banned && !x.Deleted).FirstOrDefaultAsync();
      CommentResponse commentResponse = new CommentResponse(comment, user);
      commentResponses.Add(commentResponse);
    }

    BlogResponse blogResponse = new BlogResponse(blog, author, commentResponses);

    return Ok(blogResponse);

  }

  [HttpPost]
  [Route("create")]
  public async Task<IActionResult> CreateBlog([FromBody] CreateBlogRequest body)
  {
    string? username = Request.HttpContext.User.FindFirstValue("username");

    if (String.IsNullOrEmpty(username))
      return Unauthorized("You are not authorized user.");

    User? user = await _mongoDB.UserCollection.Find(x => x.Username == username && !x.Banned && !x.Deleted).FirstOrDefaultAsync();

    if (user == null)
      return NotFound("User is not found.");

    Blog newBlog = new Blog { Topic = body.Topic, Category = body.Category, Detail = body.Content, UserId = user.Id };

    await _mongoDB.BlogCollection.InsertOneAsync(newBlog);

    return CreatedAtAction(nameof(CreateBlog), newBlog);

  }

  [HttpPatch]
  [Route("update/{id:length(24)}")]
  public async Task<IActionResult> UpdateBlog(string id, [FromBody] EditContentRequest body)
  {
    Blog? blog = await _mongoDB.BlogCollection.Find(x => x.Id == id && !x.Hide && !x.Deleted).FirstOrDefaultAsync();

    if (blog == null)
      return NotFound("Blog is not found.");

    blog.Topic = body.Topic ?? blog.Topic;
    blog.Detail = body.Content ?? blog.Detail;
    blog.Category = body.Category ?? blog.Category;
    blog.Hide = body.Hide ?? blog.Hide;
    blog.UpdatedDate = DateTime.UtcNow;

    await _mongoDB.BlogCollection.ReplaceOneAsync(x => x.Id == id, blog);

    return Ok(blog);
  }

  [HttpPatch]
  [Route("like/{id:length(24)}")]
  public async Task<IActionResult> LikeBlog(string id)
  {
    string? username = Request.HttpContext.User.FindFirstValue("username");

    if (String.IsNullOrEmpty(username))
      return Unauthorized("You are not authorized user.");

    User? user = await _mongoDB.UserCollection.Find(x => x.Username == username && !x.Banned && !x.Deleted).FirstOrDefaultAsync();

    if (user == null)
      return NotFound("User is not found.");

    Blog? blog = await _mongoDB.BlogCollection.Find(x => x.Id == id && !x.Hide && !x.Deleted).FirstOrDefaultAsync();

    if (blog == null)
      return NotFound("Blog is not found.");

    if (blog.Like.Contains(username))
      blog.Like.Remove(username);
    else
      blog.Like.Add(username);

    await _mongoDB.BlogCollection.ReplaceOneAsync(x => x.Id == id, blog);

    return Ok(blog);

  }

  [HttpPatch]
  [Route("hide/{id:length(24)}")]
  public async Task<IActionResult> HideBlog(string id)
  {
    string? username = Request.HttpContext.User.FindFirstValue("username");

    if (String.IsNullOrEmpty(username))
      return Unauthorized("You are not authorized user.");

    User? user = await _mongoDB.UserCollection.Find(x => x.Username == username && !x.Banned && !x.Deleted).FirstOrDefaultAsync();

    if (user == null)
      return NotFound("User is not found.");

    Blog? blog = await _mongoDB.BlogCollection.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();

    if (blog == null)
      return NotFound("Blog is not found.");

    if (blog.UserId != user.Id)
      return Unauthorized("You are not the author of this blog.");

    blog.Hide = !blog.Hide;

    await _mongoDB.BlogCollection.ReplaceOneAsync(x => x.Id == id, blog);

    return Ok(blog);

  }

  [HttpDelete]
  [Route("delete/{id:length(24)}")]
  public async Task<IActionResult> DeleteBlog(string id)
  {
    string? username = Request.HttpContext.User.FindFirstValue("username");

    if (String.IsNullOrEmpty(username))
      return Unauthorized("You are not authorized user.");

    User? user = await _mongoDB.UserCollection.Find(x => x.Username == username && !x.Banned && !x.Deleted).FirstOrDefaultAsync();

    if (user == null)
      return NotFound("User is not found.");

    Blog? blog = await _mongoDB.BlogCollection.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();

    if (blog == null)
      return NotFound("Blog is not found.");

    if (blog.UserId != user.Id)
      return Unauthorized("You are not the author of this blog.");

    blog.Hide = true;
    blog.Deleted = true;

    await _mongoDB.BlogCollection.ReplaceOneAsync(x => x.Id == id, blog);

    return NoContent();

  }
}