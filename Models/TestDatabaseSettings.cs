namespace software_studio_backend.Models;

public class TestDatabaseSettings
{
  public string ConnectingString { get; set; } = null!;

  public string DatabaseName { get; set; } = null!;

  public string UsersCollectionName { get; set; } = null!;

  public string ContentsCollectionName { get; set; } = null!;

  public string CommentsCollectionName { get; set; } = null!;
}