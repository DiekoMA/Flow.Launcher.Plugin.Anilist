using System.Collections.Generic;
using AniListNet;
using AniListNet.Objects;
using AniListNet.Parameters;

namespace Flow.Launcher.Plugin.Anilist.Utils;

public static class AnilistHandler
{
    public static AniPagination<Media> SearchAnime(string query)
    {
        var client = new AniClient();

        var results = client.SearchMediaAsync(new SearchMediaFilter
        {
            Query = query,
            Type = MediaType.Anime,
            Sort = MediaSort.Popularity
        });
        return results.Result;
    }
}