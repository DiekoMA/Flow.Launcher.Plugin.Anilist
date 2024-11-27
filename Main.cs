using AniListNet;
using AniListNet.Objects;
using AniListNet.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Flow.Launcher.Plugin.Anilist.Views;
using JetBrains.Annotations;

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
                            result.Action = e =>
                            {
                                string url = $"https://anilist.co/anime/{anime.Id}";
                                _context.API.OpenUrl(url);
                                return true;
                            };
                            result.ContextData = anime;
                            results.Add(result);
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

                            result.IcoPath = manga.Cover.ExtraLargeImageUrl.ToString();
                            result.Action = e =>
                            {
                                string url = $"https://anilist.co/anime/{manga.Id}";
                                _context.API.OpenUrl(url);
                                return true;
                            };
                            result.ContextData = manga;
                            results.Add(result);
                        }

                        break;
                }
            }

            return results;
        }

        public Control CreateSettingPanel()
        {
            return new AnilistSettings(_context, _settings);
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
                        _context.API.ShowMsgError("Error", "There was an issue Updating your list");
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
                        catch (Exception e)
                        {
                            results.Add(new Result()
                            {
                                Title = "Error" + e.Message ,
                                SubTitle = "No access token found, please add one in the plugin settings",
                            });
                            /*_context.API.ShowMsgError("Not Authenticated",
                                "Please add your token in the plugin settings");*/
                        }

                        return true;
                    },
                });
                results.Add(new Result()
                {
                    Title = "Copy Anilist id",
                    IcoPath = "Assets\\AniListlogo.png",
                    AsyncAction = async c =>
                    {
                        Clipboard.SetText(media.Id.ToString());
                        return true;
                    }
                });
                
                results.Add(new Result()
                {
                    Title = "Copy MyAnimeList id",
                    IcoPath = "Assets\\AniListlogo.png",
                    AsyncAction = async c =>
                    {
                        Clipboard.SetText(media.MalId.ToString());
                        return true;
                    }
                });
            }

            return results;
        }
    }
}