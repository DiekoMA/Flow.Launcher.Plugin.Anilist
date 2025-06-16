using AniListNet;
using AniListNet.Objects;
using AniListNet.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Flow.Launcher.Plugin.Anilist.ViewModels;
using Flow.Launcher.Plugin.Anilist.Views;

namespace Flow.Launcher.Plugin.Anilist
{
    public class Anilist : IPlugin, ISettingProvider, IContextMenu
    {
        private readonly AniClient _client = new AniClient();
        private User authenticatedUser;
        private static AnilistSettingsViewModel _viewModel;
        private PluginInitContext _context;
        private Settings _settings;

        public void Init(PluginInitContext context)
        {
            _context = context;
            _settings = context.API.LoadSettingJsonStorage<Settings>();
            _viewModel = new AnilistSettingsViewModel(_context, _settings);
            if (_settings.AnilistToken is not null)
            {
                _client.TryAuthenticateAsync(_settings.AnilistToken);
                authenticatedUser = _client.GetAuthenticatedUserAsync().Result;
            }
            
        }

        private List<Result> SearchAnime(string searchQuery)
        {
            List<Result> results = new List<Result>();
            var animeSearchResults = _client.SearchMediaAsync(new SearchMediaFilter
            {
                Query = searchQuery,
                Type = MediaType.Anime,
                Sort = _settings.DefaultMediaSort,
                Format = new Dictionary<MediaFormat, bool>
                { 
                    { MediaFormat.TV, true }, 
                    { MediaFormat.Movie, true }, 
                    { MediaFormat.TVShort, true }, 
                    { MediaFormat.ONA, true },
                }
            });

            foreach (var anime in animeSearchResults.Result.Data)
            {
                var result = new Result();
                result.Title = anime.Title.EnglishTitle ?? anime.Title.RomajiTitle;
                if (anime.Entry != null)
                {
                    result.SubTitle = $"Format: {anime.Format}  |  Status: {anime.Status} \n" +
                                      $"Watched: {anime.Entry?.Progress ?? 0} / {anime.Episodes}";
                }
                else
                {
                    result.SubTitle = $"Format: {anime.Format}  |  Status: {anime.Status} \n" +
                                      $"Episodes: {anime.Episodes?.ToString() ?? "?"}";
                }

                result.IcoPath = anime.Cover.ExtraLargeImageUrl.ToString();
                result.PreviewPanel = new Lazy<UserControl>(() => new AnilistPreview(new AnilistPreviewViewModel(anime)));
                result.Action = e =>
                {
                    var url = $"https://anilist.co/anime/{anime.Id}";
                    _context.API.OpenUrl(url);
                    return true;
                };
                result.CopyText = $"https://anilist.co/anime/{anime.Id}";
                result.ContextData = anime;
                results.Add(result);
            }
            return results;
        }

        private List<Result> SearchManga(string searchQuery)
        {
            List<Result> results = new List<Result>();

            var mangaSearchResults = _client.SearchMediaAsync(new SearchMediaFilter
            {
                Query = searchQuery,
                Type = MediaType.Manga,
                Sort = MediaSort.Popularity,
                Format = new Dictionary<MediaFormat, bool>
                {
                    { MediaFormat.Manga, true },
                    { MediaFormat.OneShot, true }
                }
            });

            foreach (var manga in mangaSearchResults.Result.Data)
            {
                var result = new Result();
                
                result.Title = manga.Title.EnglishTitle ?? manga.Title.RomajiTitle;
                if (manga.Entry != null)
                {
                    result.SubTitle = $"Format: {manga.Format}  |  Status: {manga.Status} \n" +
                                      $"Chapters Read: {manga.Entry?.Progress ?? 0} / {manga.Chapters}  |  Volumes Read: {manga.Entry?.VolumeProgress ?? 0} / {manga.Volumes}";
                }
                else
                {
                    result.SubTitle = $"Format: {manga.Format}  |  Status: {manga.Status} \n" +
                                      $"Chapters: {manga.Chapters}  |  Volumes: {manga.Volumes}";
                }

                result.IcoPath = manga.Cover.MediumImageUrl.ToString();
                result.PreviewPanel = new Lazy<UserControl>(() => new AnilistPreview(new AnilistPreviewViewModel(manga)));
                result.Action = e =>
                {
                    var url = $"https://anilist.co/manga/{manga.Id}";
                    _context.API.OpenUrl(url);
                    return true;
                };
                result.CopyText = $"https://anilist.co/manga/{manga.Id}";
                result.ContextData = manga;
                results.Add(result);
            }
            return results;
        }

        private List<Result> SearchCharacters(string searchQuery)
        {
            List<Result> results = new List<Result>();

            var characterSearchResults = _client.SearchCharacterAsync(new SearchCharacterFilter()
            {
                Query = searchQuery,
                Sort = CharacterSort.Relevance,
            });

            foreach (var character in characterSearchResults.Result.Data)
            {
                var result = new Result();
                result.Title = character.Name.FullName;
                result.IcoPath = character.Image.LargeImageUrl.ToString();
                result.PreviewPanel = new Lazy<UserControl>(() => new AnilistCharacterPreview(new AnilistCharacterPreviewViewModel(character)));
                result.Action = e =>
                {
                    var url = $"https://anilist.co/character/{character.Id}/{character.Name.FullName}";
                    _context.API.OpenUrl(url);
                    return true;
                };
                result.CopyText = $"https://anilist.co/manga/{character.Name.FullName}";
                result.ContextData = character;
                results.Add(result);
            }
            return results;
        }
        
        public List<Result> Query(Query query)
        {
            List<Result> results = new List<Result>();

            if (query.Search.Length < 2)
            {
                results.Add(new Result
                {
                    Title = "a:<Anime>",
                    SubTitle = "Search for any Anime e.g al a:My Hero",
                    IcoPath = "Assets\\AniListLogo.png"
                });
                results.Add(new Result
                {
                    Title = "m:<Manga>",
                    SubTitle = "Search for Manga e.g al m:Kagurabachi",
                    IcoPath = "Assets\\AniListLogo.png"
                });
                results.Add(new Result
                {
                    Title = "c:<Character>",
                    SubTitle = "Search for any Character e.g al c:Rimuru",
                    IcoPath = "Assets\\AniListLogo.png"
                });
            }
            else
            {
                var searchQuery = query.Search;
                if (searchQuery.ToLower().StartsWith("a:") || searchQuery.ToLower().StartsWith("m:") || searchQuery.ToLower().StartsWith("c:") || searchQuery.ToLower().StartsWith("u:"))
                    switch (searchQuery.ToLower().Substring(0, 2))
                    {
                        case "a:":
                            searchQuery = searchQuery.Substring(2).Trim();
                            results.AddRange(SearchAnime(searchQuery));
                            break;
                        case "m:":
                            searchQuery = searchQuery.Substring(2).Trim();
                            results.AddRange(SearchManga(searchQuery));
                            break;
                        case "c:":
                            searchQuery = searchQuery.Substring(2).Trim();
                            results.AddRange(SearchCharacters(searchQuery));
                            break;
                    }
            }

            return results;
        }

        public Control CreateSettingPanel()
        {
            return new AnilistSettings(_viewModel);
        }

        public List<Result> LoadContextMenus(Result selectedResult)
        {
            var results = new List<Result>();
            var media = selectedResult.ContextData as Media;

            Func<ActionContext, ValueTask<bool>> PlusOneActionCreator(bool updateVolume)
            {
                return async ValueTask<bool> (ActionContext c) =>
                {
                    try
                    {
                        var mediaEntryMutation = new MediaEntryMutation();
                        switch (media.Type)
                        {
                            case MediaType.Anime:
                                mediaEntryMutation.Progress = media.Entry.Progress + 1;
                                if (mediaEntryMutation.Progress >= media.Episodes)
                                {
                                    mediaEntryMutation.Status = MediaEntryStatus.Completed;
                                    mediaEntryMutation.CompleteDate = DateTime.Now;
                                }

                                break;
                            case MediaType.Manga:
                                // TODO: Support volume progress
                                if (updateVolume)
                                {
                                    mediaEntryMutation.VolumeProgress = media.Entry.VolumeProgress + 1;
                                }
                                else
                                {
                                    mediaEntryMutation.Progress = media.Entry.Progress + 1;
                                }

                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        await _client.SaveMediaEntryAsync(media.Id, mediaEntryMutation);
                    }
                    catch (Exception e)
                    {
                        // ignored
                    }

                    return true;
                };
            }

            // If already on list
            if (media.Entry != null)
            {
                if (media.Type == MediaType.Anime)
                {
                    // Add a +1 progress option
                    results.Add(new()
                    {
                        Title = "+1 Watched",
                        IcoPath = "Assets\\AniListLogo.png",
                        AsyncAction = PlusOneActionCreator(false),
                    });
                }
                else
                {
                    results.Add(new()
                    {
                        Title = "+1 Chapter Read",
                        IcoPath = "Assets\\AniListLogo.png",
                        AsyncAction = PlusOneActionCreator(false)
                    });
                    results.Add(new()
                    {
                        Title = "+1 Volume Read",
                        IcoPath = "Assets\\AniListLogo.png",
                        AsyncAction = PlusOneActionCreator(true)
                    });
                }

                results.Add(new Result
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
                            _context.API.ShowMsgError("Not Authenticated",
                                "Please add your token in the plugin settings");
                        }

                        return true;
                    },
                });
            }
            // If not on list, add an option to add to list
            else
            {
                results.Add(new Result
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
                            await _client.SaveMediaEntryAsync(anilistEntry.Data.FirstOrDefault()!.Id,
                                new MediaEntryMutation()
                                {
                                    StartDate = DateTime.Today,
                                    Progress = 0,
                                    Status = MediaEntryStatus.Planning
                                });
                        }
                        catch (Exception)
                        {
                            _context.API.ShowMsgError("Not Authenticated",
                                "Please add your token in the plugin settings");
                        }

                        return true;
                    },
                });
            }
            return results;
        }
    }
    // string[] EasterEgg = new[] { "What", "Are", "You", "Looking", "For", "Down", "Here"};
}