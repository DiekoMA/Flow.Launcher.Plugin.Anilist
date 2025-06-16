using AniListNet.Objects;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Flow.Launcher.Plugin.Anilist.ViewModels;

public class AnilistCharacterPreviewViewModel : ObservableObject
{
    private Character _character;
    public Character Character
    {
        get => _character;
        set
        {
            if (SetProperty(ref _character, value))
            {
                OnPropertyChanged();
            }
        }
    }

    public AnilistCharacterPreviewViewModel(Character character)
    {
        Character = character;
    }

}