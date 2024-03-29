using AniListNet;
using AniListNet.Objects;
using AniListNet.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

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

                switch (_settings.DefaultMediaType)
                {
                    case MediaType.Anime:
                        var animeSearchResults = _client.SearchMediaAsync(new SearchMediaFilter
                        {
                            Query = searchQuery,
                            Type = MediaType.Anime,
                            Sort = _settings.DefaultMediaSort,
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
                        break;
                    case MediaType.Manga:
                        var mangaSearchResults = _client.SearchMediaAsync(new SearchMediaFilter
                        {
                            Query = searchQuery,
                            Type = MediaType.Manga,
                            Sort = MediaSort.Popularity,
                            Format = new Dictionary<MediaFormat, bool>
                    {
                        { MediaFormat.Manga, true},
                        { MediaFormat.OneShot, true}
                    }
                        });

                        foreach (var manga in mangaSearchResults.Result.Data)
                        {
                            results.Add(new Result
                            {
                                Title = manga.Title.EnglishTitle ?? manga.Title.RomajiTitle,
                                SubTitle = $"Format: {manga.Format} \nStatus: {manga.Status}",
                                IcoPath = manga.Cover.ExtraLargeImageUrl.ToString(),
                                Action = e =>
                                {
                                    string url = $"https://anilist.co/anime/{manga.Id}";
                                    _context.API.OpenUrl(url);
                                    return true;
                                }
                            });
                        }
                        break;
                }
            }

            /*results.Add(new Result
            {
                Title = "Search",
                Action = c =>
                {
                    
                    return true;
                }
            });*/

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
                    IcoPath = "Assets\\AniListlogo.png",
                    AsyncAction = async c =>
                    {
                        try
                        {
                            var anilistEntry = await _client.SearchMediaAsync(new SearchMediaFilter
                            {
                                Query = selectedResult.Title,
                                Type = _settings.DefaultMediaType,
                                Sort = MediaSort.Popularity
                            });
                            await _client.SaveMediaEntryAsync(anilistEntry.Data.FirstOrDefault()!.Id, new MediaEntryMutation()
                            {
                                StartDate = DateTime.Today,
                                Progress = 0,
                                Status = MediaEntryStatus.Planning
                            });
                        }
                        catch (Exception)
                        {
                            _context.API.ShowMsgError("Not Authenticated", "Please add your token in the plugin settings");
                        }
                        return true;
                    },
                },
                new ()
                {
                    Title = "Remove from list",
                    IcoPath = "Assets\\AniListlogo.png",
                    AsyncAction = async c =>
                    {
                        try
                        {
                            var anilistEntry = await _client.SearchMediaAsync(new SearchMediaFilter
                            {
                                Query = selectedResult.Title,
                                Type = _settings.DefaultMediaType,
                                Sort = MediaSort.Popularity
                            });
                            await _client.DeleteMediaEntryAsync(anilistEntry.Data.FirstOrDefault()!.Id);
                        }
                        catch (Exception)
                        {
                            _context.API.ShowMsgError("Not Authenticated", "Please add your token in the plugin settings");
                        }

                        return true;
                    },
                }
            };

            return results;
        }
    }
}