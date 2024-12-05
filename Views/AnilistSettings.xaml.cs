using System.Windows;
using System.Windows.Controls;
using Flow.Launcher.Plugin.Anilist.ViewModels;

namespace Flow.Launcher.Plugin.Anilist.Views;

public partial class AnilistSettings : UserControl
{
    public AnilistSettings(AnilistSettingsViewModel viewModel)
    {
        InitializeComponent();
        this.DataContext = viewModel;
    }
}