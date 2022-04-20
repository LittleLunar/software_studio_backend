using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using software_studio_backend.Utils;
using MongoDB.Driver;
using software_studio_backend.Services;
using software_studio_backend.Models;
namespace software_studio_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class AdminController : ControllerBase
{
  private readonly MongoDBService _mongoDB;
  public AdminController(MongoDBService mongoDBService)
  {
    _mongoDB = mongoDBService;
  }

  [HttpGet]
  [Route("manage/user/list")]
  public async Task<IActionResult> GetUserList()
  {
    List<User> users = await _mongoDB.UserCollection.Find(_ => true).ToListAsync();

    return Ok(users.OrderByDescending(x => x.UpdatedDate).ToList());
  }

  [HttpGet]
  [Route("manage/user/get/{id:length(24)}")]
  public async Task<IActionResult> GetUserById(string id)
  {
    User? user = await _mongoDB.UserCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (user == null)
      return NotFound("User is not found.");

    return Ok(user);
  }

  [HttpPatch]
  [Route("manage/user/update/{id:length(24)}")]
  public async Task<IActionResult> UpdateUserById(string id, [FromBody] AdminUserUpdateRequest body)
  {
    User? user = await _mongoDB.UserCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (user == null)
      return NotFound("User is not found.");

    user.Username = body.Username ?? user.Username;

    user.Password = body.Password ?? user.Password;

    user.Role = body.Role ?? user.Role;

    user.Name = body.Name ?? user.Name;

    user.Banned = body.Banned ?? user.Banned;

    user.Deleted = body.Deleted ?? user.Deleted;

    user.ProfileImage = body.ProfileImage ?? user.ProfileImage;

    user.UpdatedDate = DateTime.UtcNow;

    await _mongoDB.UserCollection.ReplaceOneAsync(x => x.Id == id, user);

    return Ok(user);
  }

  [HttpPatch]
  [Route("manage/user/banned/{id:length(24)}")]
  public async Task<IActionResult> BannedUserById(string id)
  {
    User? user = await _mongoDB.UserCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (user == null)
      return NotFound("User is not found.");

    user.Banned = !user.Banned;

    await _mongoDB.UserCollection.ReplaceOneAsync(x => x.Id == id, user);

    return Ok(user);
  }

  [HttpPatch]
  [Route("manage/user/delete/{id:length(24)}")]
  public async Task<IActionResult> DeleteUserById(string id)
  {
    User? user = await _mongoDB.UserCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (user == null)
      return NotFound("User is not found.");

    user.Deleted = !user.Deleted;

    await _mongoDB.UserCollection.ReplaceOneAsync(x => x.Id == id, user);

    return Ok(user);
  }

  [HttpGet]
  [Route("manage/blog/list")]
  public async Task<IActionResult> GetBlogs() // All blogs page
  {
    List<Blog> blogs = await _mongoDB.BlogCollection.Find(_ => true).ToListAsync();

    List<AdminBlogResponse> blogResponses = new List<AdminBlogResponse>();

    foreach (Blog blog in blogs)
    {
      User user = await _mongoDB.UserCollection.Find(x => x.Id == blog.UserId).FirstAsync();
      AdminBlogResponse blogResponse = new AdminBlogResponse(blog);
      blogResponses.Add(blogResponse);
    }

    AdminBlogListResponse blogList = new AdminBlogListResponse { Blogs = blogResponses.OrderByDescending(x => x.CreatedDate).ToList() };

    return Ok(blogList);
  }

  [HttpPatch]
  [Route("manage/blog/update/{id:length(24)}")]
  public async Task<IActionResult> UpdateBlog(string id, [FromBody] AdminBlogUpdateRequest body)
  {
    Blog? blog = await _mongoDB.BlogCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (blog == null)
      return NotFound("Blog is not found.");

    blog.Topic = body.Topic ?? blog.Topic;
    blog.Detail = body.Content ?? blog.Detail;
    blog.Hide = body.Hide ?? blog.Hide;
    blog.Deleted = body.Delete ?? blog.Deleted;
    blog.UpdatedDate = DateTime.UtcNow;

    await _mongoDB.BlogCollection.ReplaceOneAsync(x => x.Id == id, blog);

    return Ok(blog);
  }

  [HttpPatch]
  [Route("manage/blog/hide/{id:length(24)}")]
  public async Task<IActionResult> HideBlogById(string id)
  {
    Blog? blog = await _mongoDB.BlogCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (blog == null)
      return NotFound("Blog is not found.");

    blog.Hide = !blog.Hide;

    await _mongoDB.BlogCollection.ReplaceOneAsync(x => x.Id == id, blog);

    return Ok(blog);
  }

  [HttpPatch]
  [Route("manage/blog/delete/{id:length(24)}")]
  public async Task<IActionResult> DeleteBlogById(string id)
  {
    Blog? blog = await _mongoDB.BlogCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (blog == null)
      return NotFound("Blog is not found.");

    blog.Hide = !blog.Deleted;
    blog.Deleted = !blog.Deleted;

    await _mongoDB.BlogCollection.ReplaceOneAsync(x => x.Id == id, blog);

    return Ok(blog);
  }

  [HttpGet]
  [Route("manage/comment/list")]
  public async Task<IActionResult> GetComments()
  {
    List<Comment> comments = await _mongoDB.CommentCollection.Find(_ => true).ToListAsync();

    List<AdminCommentResponse> adminCommentResponses = new List<AdminCommentResponse>();
    foreach (Comment comment in comments)
    {
      User user = await _mongoDB.UserCollection.Find(x => x.Id == comment.UserId).FirstAsync();
      AdminCommentResponse commentResponse = new AdminCommentResponse(comment, user);
      adminCommentResponses.Add(commentResponse);
    }

    AdminCommentListResponse adminCommentList = new AdminCommentListResponse { Comments = adminCommentResponses.OrderByDescending(x => x.UpdatedDate).ToList() };


    return Ok(adminCommentList);
  }

  [HttpGet]
  [Route("manage/comment/delete/{id:length(24)}")]
  public async Task<IActionResult> UpdateComment(string id)
  {
    Comment? comment = await _mongoDB.CommentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (comment == null)
      return NotFound("comment is not found.");

    comment.Delete = !comment.Delete;

    await _mongoDB.CommentCollection.ReplaceOneAsync(x => x.Id == id, comment);

    return Ok(comment);
  }

  [HttpPost]
  [Route("manage/announce/create")]
  public async Task<IActionResult> CreateAnnounce([FromBody] CreateAnnounceRequest body)
  {
    await _mongoDB.AnnounceCollection.DeleteManyAsync(_ => true);

    Announcement announce = new Announcement { Content = body.Content };

    await _mongoDB.AnnounceCollection.InsertOneAsync(announce);

    return Ok(announce);
  }

  [HttpDelete]
  [Route("manage/announce/delete")]
  public async Task<IActionResult> DeleteAnnounce()
  {
    await _mongoDB.AnnounceCollection.DeleteManyAsync(_ => true);
    return NoContent();
  }


  [AllowAnonymous]
  [HttpGet]
  [Route("mock/mock/mock/data")]
  public async Task<IActionResult> MockData()
  {

    List<User> newUsers = new List<User>()
    {
      new User { Username = "admin1", Password = PasswordEncryption.Encrypt("admin123"), Name = "Admin Prachya", Role = "admin" },
      new User { Username = "admin2", Password = PasswordEncryption.Encrypt("admin123"), Name = "Admin Thana", Role = "admin" },
      new User { Username = "admin3", Password = PasswordEncryption.Encrypt("admin123"), Name = "Admin Thanadol", Role = "admin" },
      new User { Username = "admin4", Password = PasswordEncryption.Encrypt("admin123"), Name = "Admin Thanakorn", Role = "admin" },
      new User { Username = "admin5", Password = PasswordEncryption.Encrypt("admin123"), Name = "Admin Rinraphat", Role = "admin" },
      new User { Username = "aiden", Password = PasswordEncryption.Encrypt("user123"), Name = "Aiden" },
      new User { Username = "tanya", Password = PasswordEncryption.Encrypt("user123"), Name = "Tanya" },
      new User { Username = "marvin", Password = PasswordEncryption.Encrypt("user123"), Name = "Marvin" , Banned = true},
      new User { Username = "leonardo", Password = PasswordEncryption.Encrypt("user123"), Name = "Leonardo", Deleted = true},
      new User { Username = "juan", Password = PasswordEncryption.Encrypt("user123"), Name = "Juan" , Banned = true},
      new User { Username = "herman", Password = PasswordEncryption.Encrypt("user123"), Name = "Herman" },
      new User { Username = "dewey", Password = PasswordEncryption.Encrypt("user123"), Name = "Dewey" , Banned = true, Deleted = true}
    };

    List<Blog> newBlogs = new List<Blog>()
    {
      new Blog { Topic = "ลือลั่น!! จารย์แดงมาแล้ว", Detail = "สวัสดีท่านผู้เจริญ", Category = {"คำสอน", "ความเชื่อ"}, Like = {"alfred", "marvin", "herman" } , UserId = newUsers[0].Id},
      new Blog { Topic = "วัดเกิดใหม่กลางน้ำ", Detail = "มีวัดเกิดใหม่กลางแม่น้ำไนล์สาย21", Category = {"ท่องเที่ยว" }, Like = {"alfred", "dewey", "herman" } , UserId = newUsers[3].Id},
      new Blog { Topic = "คำสอนมหานิยม!! คนดังคนเด็ดรวยๆๆ", Detail = "เร่เข้ามาๆ คำสอนดีๆเพียง 200 บาทเท่านั้น", Category = {"ความเชื่อ", "คำสอน" }, Like = {"alfred", "dewey", "marvin" } , UserId = newUsers[0].Id},
      new Blog { Topic = "ขายตรงคติประจำใจจากมนุษย์ต่างดาว", Detail = "คติที่จะทำให้คุณได้เบิกบานใจในทุกๆวันเพียง 300 บาทต่อเดือน", Category = {"ความเชื่อ", "คำสอน" }, Like = {"juan", "dewey" } , UserId = newUsers[2].Id},
      new Blog { Topic = "พส.จีวรบิน ที่วัดพระกระจกแก้ว", Detail = "ท่านผู้เจริญที่มากไปด้วยปัญญาผู้หนึ่งได้เป็นอรหันต์จากการกระตุกจิตกระชากใจของอาจารย์แดง", Category = {"ความเชื่อ", "พระสงฆ์" }, Like = {"alfred", "juan", "herman" } , UserId = newUsers[0].Id},
      new Blog { Topic = "วัดทองคำน่าเลื่อมใส โดนโจรขโมยเจดีย์เมื่อ 10.00 น.นี้", Detail = "วัดทองคำในเมืองเอลโดลาโด้ได้โดนโจรขึ้นวัดขโมยเจดีย์ไป 1 หน่วยรวมมูลค่า 2 สลึง", Category = {"ท่องเที่ยว"}, Like = {"leonardo", "herman" } , UserId = newUsers[1].Id},
      new Blog { Topic = "คำสอนใหม่ พระมหามโนดม", Detail = "วัดปะละครับน้อนนนๆ", Category = {"คำสอน", "พระสงฆ์" }, Like = {"alfred", "dewey", "leonardo" } , UserId = newUsers[4].Id},
    };

    List<Comment> newComments = new List<Comment>()
    {
      new Comment { Detail = "สวัสดีท่านผู้เจริญ",  Like = {"alfred", "marvin", "herman" } , UserId = newUsers[0].Id, ContentId = newBlogs[1].Id},
      new Comment { Detail = "มีวัดเกิดใหม่กลางแม่น้ำไนล์สาย21",  Like = {"alfred", "dewey", "herman" } , UserId = newUsers[3].Id, ContentId = newBlogs[0].Id},
      new Comment { Detail = "เร่เข้ามาๆ คำสอนดีๆเพียง 200 บาทเท่านั้น",  Like = {"alfred", "dewey", "marvin" } , UserId = newUsers[0].Id, ContentId = newBlogs[2].Id},
      new Comment { Detail = "คติที่จะทำให้คุณได้เบิกบานใจในทุกๆวันเพียง 300 บาทต่อเดือน",  Like = {"juan", "dewey" } , UserId = newUsers[2].Id, ContentId = newBlogs[0].Id},
      new Comment { Detail = "ท่านผู้เจริญที่มากไปด้วยปัญญาผู้หนึ่งได้เป็นอรหันต์จากการกระตุกจิตกระชากใจของอาจารย์แดง", Like = {"alfred", "juan", "herman" } , UserId = newUsers[0].Id, ContentId = newBlogs[4].Id},
      new Comment { Detail = "วัดทองคำในเมืองเอลโดลาโด้ได้โดนโจรขึ้นวัดขโมยเจดีย์ไป 1 หน่วยรวมมูลค่า 2 สลึง", Like = {"leonardo", "herman" } , UserId = newUsers[1].Id, ContentId = newBlogs[1].Id},
      new Comment { Detail = "วัดปะละครับน้อนนนๆ", Like = {"alfred", "dewey", "leonardo" } , UserId = newUsers[4].Id, ContentId = newBlogs[1].Id},
    };

    await _mongoDB.UserCollection.InsertManyAsync(newUsers);
    await _mongoDB.BlogCollection.InsertManyAsync(newBlogs);
    await _mongoDB.CommentCollection.InsertManyAsync(newComments);

    return Ok(new { users = newUsers, blogs = newBlogs, comments = newComments });
  }

  [AllowAnonymous]
  [HttpPost]
  [Route("manage/user/magicadd")]
  public async Task<IActionResult> UserMagicAdd([FromBody] User newUser)
  {
    await _mongoDB.UserCollection.InsertOneAsync(newUser);
    return Ok(newUser);
  }

  [AllowAnonymous]
  [HttpPost]
  [Route("manage/blog/magicadd")]
  public async Task<IActionResult> BlogMagicAdd([FromBody] Blog newBlog)
  {
    await _mongoDB.BlogCollection.InsertOneAsync(newBlog);
    return Ok(newBlog);
  }

  [AllowAnonymous]
  [HttpPost]
  [Route("manage/comment/magicadd")]
  public async Task<IActionResult> CommentMagicAdd([FromBody] Comment newComment)
  {
    await _mongoDB.CommentCollection.InsertOneAsync(newComment);
    return Ok(newComment);
  }

  [AllowAnonymous]
  [HttpDelete]
  [Route("manage/database/destroytheworld")]
  public async Task<IActionResult> DestroyDatabase()
  {
    await _mongoDB.UserCollection.DeleteManyAsync(_ => true);
    await _mongoDB.BlogCollection.DeleteManyAsync(_ => true);
    await _mongoDB.CommentCollection.DeleteManyAsync(_ => true);
    return NoContent();
  }
}