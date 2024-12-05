using System.Security;
using AniListNet.Objects;

namespace Flow.Launcher.Plugin.Anilist;

public class Settings
{
    public string AnilistToken { get; set; }
    public MediaType DefaultMediaType { get; set; }
    public MediaSort DefaultMediaSort { get; set; }
}