using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Flow.Launcher.Plugin.Anilist;

public partial class AnilistSetting : UserControl
{
    private readonly Settings _settings;
    private readonly PluginInitContext _context;
    public AnilistSetting(PluginInitContext context,Settings settings)
    {
        InitializeComponent();
        _context = context;
        _settings = settings;
    }

    private void AnilistSetting_OnLoaded(object sender, RoutedEventArgs e)
    {
        TokenBox.Password = _settings?.AnilistToken ?? string.Empty;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        _context.API.OpenUrl("https://anilist.co/api/v2/oauth/authorize?client_id=12239&response_type=token");
    }

    private void Savebutton_OnClick(object sender, RoutedEventArgs e)
    {
        _settings.AnilistToken = TokenBox.Password;
        _context.API.SavePluginSettings();
    }
}