using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using AniListNet;
using AniListNet.Objects;
using AniListNet.Parameters;

namespace Flow.Launcher.Plugin.Anilist
{
    public class Anilist : IPlugin, ISettingProvider, IContextMenu
    {
        private readonly AniClient _client = new AniClient();
        private PluginInitContext _context;
        private Settings _settings;

        public void Init(PluginInitContext context)
        {
            _context = context;
            _settings = context.API.LoadSettingJsonStorage<Settings>();
            _client.TryAuthenticateAsync(_settings.AnilistToken);
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
                        SubTitle = $"Format: {anime.Format} \nStatus: {anime.Status} \n{anime.Entry.Status}",
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

        public Control CreateSettingPanel()
        {
            return new AnilistSetting(_context, _settings);
        }
        
        public List<Result> LoadContextMenus(Result selectedResult)
        {
            var results = new List<Result>
            {
                new ()
                {
                    Title = "Add to list",
                    IcoPath = "Assets\\add.svg",
                    AsyncAction = async c =>
                    {
                        var anilistEntry = await _client.SearchMediaAsync(new SearchMediaFilter
                        {
                            Query = selectedResult.Title,
                            Type = MediaType.Anime,
                            Sort = MediaSort.Popularity
                        });
                        await _client.SaveMediaEntryAsync(anilistEntry.Data.FirstOrDefault()!.Id, new MediaEntryMutation()
                        {
                            StartDate = DateTime.Today,
                            Progress = 0,
                            Status = MediaEntryStatus.Planning
                        });
                        return true;
                    },
                },
                new ()
                {
                    Title = "Remove from list",
                    IcoPath = "Assets\\remove.svg",
                    AsyncAction = async c =>
                    {
                        var anilistEntry = await _client.SearchMediaAsync(new SearchMediaFilter
                        {
                            Query = selectedResult.Title,
                            Type = MediaType.Anime,
                            Sort = MediaSort.Popularity
                        });
                        await _client.DeleteMediaEntryAsync(anilistEntry.Data.FirstOrDefault()!.Id);
                        return true;
                    },
                }
            };

            return results;
        }
    }
}