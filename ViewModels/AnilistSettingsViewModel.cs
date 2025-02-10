using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using AniListNet.Objects;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flow.Launcher.Plugin.Anilist.Views;

namespace Flow.Launcher.Plugin.Anilist.ViewModels;

public class AnilistSettingsViewModel : ObservableObject
{
    private MediaSort _currentMediaSort;

    public MediaSort CurrentMediaSort
    {
        get => _currentMediaSort;
        set
        {
            if (SetProperty(ref _currentMediaSort, value))
            {
                _settings.DefaultMediaSort = value;
                _context.API.SavePluginSettings();
            }
        }
    }
    
    private string _accessToken;

    public string AccessToken
    {
        get => _settings.AnilistToken;
        set
        {
            if (SetProperty(ref _accessToken, value)) 
            {
                _settings.AnilistToken = value;
                _context.API.SavePluginSettings();
            }
        }
    }

    private PluginInitContext _context;
    private Settings _settings;
    public List<MediaSort> MediaSort { get; } = Enum.GetValues(typeof(MediaSort)).Cast<MediaSort>().ToList();
    public IRelayCommand LaunchUrlCommand { get; }

    public AnilistSettingsViewModel(PluginInitContext context, Settings settings)
    {
        this._context = context;
        this._settings = settings;
        CurrentMediaSort = AniListNet.Objects.MediaSort.Relevance;
        LaunchUrlCommand = new RelayCommand(LaunchUrl);
    }


    private void LaunchUrl()
    {
        _context.API.OpenUrl("https://anilist.co/api/v2/oauth/authorize?client_id=12239&response_type=token");
    }
}