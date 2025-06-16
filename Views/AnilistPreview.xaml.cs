using System.Windows.Controls;
using Flow.Launcher.Plugin.Anilist.ViewModels;

namespace Flow.Launcher.Plugin.Anilist.Views;

public partial class AnilistPreview : UserControl
{
    public AnilistPreview(AnilistPreviewViewModel viewModel)
    {
        InitializeComponent();
        this.DataContext = viewModel;
    }
}