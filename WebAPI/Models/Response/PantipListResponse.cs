
using System.Text.Json.Serialization;

namespace software_studio_backend.Models;

public class PantipListResponse
{
  [JsonPropertyName("pantips")]
  public List<PantipResponse> Pantips = new List<PantipResponse>();
}