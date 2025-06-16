using System.Windows.Controls;
using Flow.Launcher.Plugin.Anilist.ViewModels;

namespace Flow.Launcher.Plugin.Anilist.Views;

public partial class AnilistCharacterPreview : UserControl
{
    public AnilistCharacterPreview(AnilistCharacterPreviewViewModel viewModel)
    {
        InitializeComponent();
        this.DataContext = viewModel;
    }
}