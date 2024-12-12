using CommunityToolkit.Mvvm.ComponentModel;
using VolumeRender.Models;

namespace VolumeRender.ViewModels;

public partial class ShellViewModel : ObservableObject
{

    [ObservableProperty]
    public partial ImagesObject ImagesObj { get; set; } = new();


}