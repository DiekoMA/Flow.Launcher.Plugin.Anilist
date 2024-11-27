/*using AniListNet.Objects;
using System.Windows;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.Anilist;

public partial class AnilistSetting : UserControl
{
   
    public AnilistSetting(PluginInitContext context, Settings settings)
    {
        InitializeComponent();
        
    }

    private void AnilistSetting_OnLoaded(object sender, RoutedEventArgs e)
    {
        TokenBox.Password = _settings?.AnilistToken ?? string.Empty;
        switch (_settings.DefaultMediaType)
        {
            case MediaType.Anime:
                DefaultMediaTypeCB.SelectedIndex = 0;
                break;

            case MediaType.Manga:
                DefaultMediaTypeCB.SelectedIndex = 1;
                break;
        }

        switch (_settings.DefaultMediaSort)
        {
            case MediaSort.Relevance:
                DefaultSortCB.SelectedIndex = 0;
                break;

            case MediaSort.Score:
                DefaultSortCB.SelectedIndex = 1;
                break;

            case MediaSort.Popularity:
                DefaultSortCB.SelectedIndex = 2;
                break;

            case MediaSort.Trending:
                DefaultSortCB.SelectedIndex = 3;
                break;

            case MediaSort.Favorites:
                DefaultSortCB.SelectedIndex = 4;
                break;

            default:
                DefaultSortCB.SelectedIndex = 2;
                break;

        }
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        
    }

    private void Savebutton_OnClick(object sender, RoutedEventArgs e)
    {
        _settings.AnilistToken = TokenBox.Password;
        _context.API.SavePluginSettings();
    }

    private void DefaultMediaTypeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        switch (DefaultMediaTypeCB.SelectedIndex)
        {
            case 0:
                _settings.DefaultMediaType = AniListNet.Objects.MediaType.Anime;
                _context.API.SavePluginSettings();
                break;

            case 1:
                _settings.DefaultMediaType = AniListNet.Objects.MediaType.Manga;
                _context.API.SavePluginSettings();
                break;
        }
    }

    private void DefaultSortCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        switch (DefaultSortCB.SelectedIndex)
        {
            case 0:
                _settings.DefaultMediaSort = MediaSort.Relevance;
                _context.API.SavePluginSettings();
                break;

            case 1:
                _settings.DefaultMediaSort = MediaSort.Score;
                _context.API.SavePluginSettings();
                break;

            case 3:
                _settings.DefaultMediaSort = MediaSort.Popularity;
                _context.API.SavePluginSettings();
                break;

            case 4:
                _settings.DefaultMediaSort = MediaSort.Trending;
                _context.API.SavePluginSettings();
                break;

            case 5:
                _settings.DefaultMediaSort = MediaSort.Favorites;
                _context.API.SavePluginSettings();
                break;

            default:
                _settings.DefaultMediaSort = MediaSort.Popularity;
                _context.API.SavePluginSettings();
                break;
        }
    }
}*/