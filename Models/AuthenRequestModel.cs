using System.ComponentModel.DataAnnotations;

namespace software_studio_backend.Models;

public class AuthenRequest
{
  [Required]
  public string username { get; set; } = null!;

  [Required]
  public string password { get; set; } = null!;
}