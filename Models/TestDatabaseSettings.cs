namespace software_studio_backend.Models;

public class TestDatabaseSettings
{
  public string? ConnectionString { get; set; }

  public string? DatabaseName { get; set; }

  public string? UsersCollectionName { get; set; }

  public string? ContentsCollectionName { get; set; }

  public string? CommentsCollectionName { get; set; }
}