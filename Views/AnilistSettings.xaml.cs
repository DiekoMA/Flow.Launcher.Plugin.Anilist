using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Flow.Launcher.Plugin.Anilist.ViewModels;

namespace Flow.Launcher.Plugin.Anilist.Views
{
    /// <summary>
    /// Interaction logic for AnilistSettings.xaml
    /// </summary>
    public partial class AnilistSettings : UserControl
    {
        private readonly Settings _settings;
        private readonly AnilistSettingsViewModel _viewModel;
        private readonly PluginInitContext _context;
        public AnilistSettings(AnilistSettingsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel; 
            this.DataContext = _viewModel;
            this.InitializeComponent();
        }
    }
}
