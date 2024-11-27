using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AniListNet.Objects;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Flow.Launcher.Plugin.Anilist.ViewModels;

public partial class AnilistSettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private string _currentMediaType;

    [ObservableProperty]
    private string _currentSortType;

    [ObservableProperty]
    public string _defaultMediaType;

    [ObservableProperty]
    private string _defaultSortType;

    [ObservableProperty]
    private string _accessToken;

    private PluginInitContext _context;
    private Settings _settings;
    public AnilistSettingsViewModel(PluginInitContext context, Settings settings)
    {
        _context = context;
        _settings = settings;
        _accessToken = _settings.AnilistToken;
        _defaultMediaType = _settings.DefaultMediaType.ToString();
        _defaultSortType = _settings.DefaultMediaSort.ToString();
    }
    
    
}

