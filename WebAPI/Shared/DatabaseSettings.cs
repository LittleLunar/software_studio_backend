namespace software_studio_backend.Shared;

public class DatabaseSettings
{
  public string? ConnectionString { get; set; }

  public string? DatabaseName { get; set; }

  public string? UsersCollectionName { get; set; }

  public string? BlogsCollectionName { get; set; }

  public string? AnnouncementsCollectionName { get; set; }

  public string? CommentsCollectionName { get; set; }
}