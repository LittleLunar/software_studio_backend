
using System.ComponentModel.DataAnnotations;

namespace software_studio_backend.Models;

public class Token
{
  [Required]
  public string Type { get; set; } = null!;

  [Required]
  public User User { get; set; } = null!;
  [Required]
  public string EncryptedString { get; set; } = null!; // store the username

  [Required]
  public DateTime Expires { get; set; }
  public bool IsExpired => DateTime.UtcNow >= Expires;

  public override string ToString()
  {
    return User.ToString() + " " + Type + " " + EncryptedString;
  }
}