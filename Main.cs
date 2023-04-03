using System.Collections.Generic;
using AniListNet;
using AniListNet.Objects;
using AniListNet.Parameters;

namespace Flow.Launcher.Plugin.Anilist
{
    public class Anilist : IPlugin
    {
        private readonly AniClient _client = new AniClient();
        private PluginInitContext _context;

        public void Init(PluginInitContext context)
        {
            _context = context;
        }

        public List<Result> Query(Query query)
        {
            List<Result> results = new List<Result>();

            if (query.Search.Length > 2)
            {
                string searchQuery = query.Search;

                var animeSearchResults =  _client.SearchMediaAsync(new SearchMediaFilter
                {
                    Query = searchQuery,
                    Type = MediaType.Anime,
                    Sort = MediaSort.Popularity,
                    Format = new Dictionary<MediaFormat, bool>
                    {
                        { MediaFormat.TV, true},
                        { MediaFormat.Movie, true},
                        { MediaFormat.TVShort, true},
                        { MediaFormat.ONA, true},
                    }
                });

                foreach (var anime in animeSearchResults.Result.Data)
                {
                    results.Add(new Result
                    {
                        Title = anime.Title.EnglishTitle ?? anime.Title.RomajiTitle,
                        SubTitle = $"Format: {anime.Format} \nStatus: {anime.Status}",
                        IcoPath = anime.Cover.ExtraLargeImageUrl.ToString(),
                        Action = e =>
                        {
                            string url = $"https://anilist.co/anime/{anime.Id}";
                            _context.API.OpenUrl(url);
                            return true;
                        }
                    });
                }
            }
            return results;
        }
    }
}