using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using AniListNet.Objects;

namespace Flow.Launcher.Plugin.Anilist.ViewModels;

public class AnilistPreviewViewModel : ObservableObject
{
    
    private Media _content;

    public Media Content
    {
        get => _content;
        set
        {
            if (SetProperty(ref _content, value))
            {
                OnPropertyChanged();
            }
        }
    }
    public AnilistPreviewViewModel(Media content)
    {
        Content = content;
    }
}