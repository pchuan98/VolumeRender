using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VolumeRender.Models;

namespace VolumeRender.ViewModels;

public partial class ShellViewModel : ObservableObject
{

    [ObservableProperty]
    public partial ImagesObject ImagesObj { get; set; } = new();


    [ObservableProperty]
    public partial ViewportModel Viewport { get; set; } = new();

    [RelayCommand]
    void Test()
    {

        Viewport.Generate(ImagesObj.QueryImages()!);
    }
}