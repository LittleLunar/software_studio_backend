
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class ErrorMessage
{
  [JsonPropertyName("error_message")]
  public string Errormessage { get; set; } = "";

  public ErrorMessage(string message)
  {
    this.Errormessage = message;
  }
}