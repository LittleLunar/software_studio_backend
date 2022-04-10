using software_studio_backend.Models;
using software_studio_backend.Shared;
namespace software_studio_backend.Services;

public static class SessionCollection
{
  public static void Create(Token newToken)
  {
    Console.WriteLine("Create new session: {0}", newToken.ToString());
    Collection.Sessions.Add(item: newToken);
    Console.WriteLine("All Sessions :");
    Collection.Sessions.ForEach(Console.WriteLine);
  }

  public static void Invalidate(Token token)
  {
    Console.WriteLine("Invalidate : {0}", token.ToString());
    Collection.Sessions.Remove(item: Collection.Sessions.SingleOrDefault(x => x.EncryptedString == token.EncryptedString)!);

    Console.WriteLine("Remaining Sessions :");
    Collection.Sessions.ForEach(Console.WriteLine);
  }

  public static Token? GetSession(string payload)
  {
    return Collection.Sessions.Find(x => x.EncryptedString == payload || x.User.Username == payload);
  }
}