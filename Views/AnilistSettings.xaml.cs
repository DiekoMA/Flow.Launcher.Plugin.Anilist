using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;
using AniListNet.Objects;

namespace Flow.Launcher.Plugin.Anilist.Views
{
    /// <summary>
    /// Interaction logic for AnilistSettings.xaml
    /// </summary>
    public partial class AnilistSettings : UserControl
    {
        private readonly PluginInitContext _context;
        private readonly Settings _settings;
        /// <inheritdoc />
        public AnilistSettings(PluginInitContext context, Settings settings)
        {
            InitializeComponent();
            _context = context; 
            _settings = settings;
        }

        private void DefaultMediaTypeCb_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _settings.DefaultMediaType = (MediaType)DefaultMediaTypeCb.SelectedIndex;
            _context.API.SavePluginSettings();
        }

        private void DefaultMediaSortCb_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _settings.DefaultMediaSort = (MediaSort)DefaultMediaSortCb.SelectedIndex;
            _context.API.SavePluginSettings();
        }

        private void AnilistSettings_OnLoaded(object sender, RoutedEventArgs e)
        {
            AccessTokenBox.Password = _settings?.AnilistToken ?? string.Empty;
            DefaultMediaTypeCb.SelectedIndex = (int)_settings.DefaultMediaType;
            DefaultMediaSortCb.SelectedIndex = (int)_settings.DefaultMediaSort;
        }

        private void RefreshTokenBtn_OnClick(object sender, RoutedEventArgs e) =>
            _context.API.OpenUrl("https://anilist.co/api/v2/oauth/authorize?client_id=12239&response_type=token");
        
        private void AccessTokenBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            _settings.AnilistToken = AccessTokenBox.Password;
            _context.API.SavePluginSettings();
        }
    }
}
