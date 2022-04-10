

namespace software_studio_backend.Shared;

public class Configuration
{
  public static IConfiguration staticConfig { get; private set; } = null!;

  public Configuration(IConfiguration configuration)
  {
    Console.WriteLine("Configuration Instatiated!!!!");
    staticConfig = configuration;
  }
}