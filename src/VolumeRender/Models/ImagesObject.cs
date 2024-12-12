using System.ComponentModel;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using VolumeRender.Extensions;

namespace VolumeRender.Models;

public partial class ImagesObject : ObservableObject
{
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName is nameof(Scale)
            or nameof(Contrast)
            or nameof(Brightness)
            or nameof(LeftLevel)
            or nameof(RightLevel)
            or nameof(Gamma))
        {
            var index = ImageIndex;
            ImageIndex = 0;
            ImageIndex = index;
        }
    }

    /// <summary>
    /// 读取的原始数据
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ImageCount))]
    public partial Mat[]? Images { get; set; } = null;

    partial void OnImagesChanged(Mat[]? value)
    {
        ImageIndex = 0;
        ImageIndex = 1;
    }

    /// <summary>
    /// 图像的范围
    /// </summary>
    public int ImageCount => Images?.Length ?? 0;

    /// <summary>
    /// 当前选择的图像
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Image))]
    public partial int ImageIndex { get; set; } = 0;

    /// <summary>
    /// 当前显示的图片
    /// </summary>
    public BitmapFrame? Image => QueryFrame();

    internal BitmapFrame? QueryFrame()
    {
        var mat = QueryImage(ImageIndex);

        if (mat is null) return null;

        Serilog.Log.Debug($"Current Show Mat : {ImageIndex}\n" +
                          $"Scale: {Scale}\n" +
                          $"Contrast: {Contrast}\n" +
                          $"Brightness: {Brightness}\n" +
                          $"LeftLevel: {LeftLevel}\n" +
                          $"RightLevel: {RightLevel}\n" +
                          $"Gamma: {Gamma}\n" +
                          $"Mat Type: {mat.Type()}\n" +
                          $"Mat Depth: {mat.Depth()}\n" +
                          $"Mat Channels: {mat.Channels()}\n" +
                          $"Mat Size: {mat.Size()}\n");

        return BitmapFrame.Create(mat.ToBitmapSource());
    }

    /// <summary>
    /// 图像压缩倍数
    /// </summary>
    [ObservableProperty]
    public partial double Scale { get; set; } = 0.5;


    /// <summary>
    /// 亮度调节，0-3
    /// </summary>
    [ObservableProperty]
    public partial double Contrast { get; set; } = 1;

    /// <summary>
    /// 亮度调节，0-255
    /// </summary>
    [ObservableProperty]
    public partial int Brightness { get; set; } = 0;

    /// <summary>
    /// 左色阶 (0-255)
    /// </summary>
    [ObservableProperty]
    public partial int LeftLevel { get; set; } = 0;

    /// <summary>
    /// 右色阶 (0-255)
    /// </summary>
    [ObservableProperty]
    public partial int RightLevel { get; set; } = 255;

    /// <summary>
    /// > 0
    /// </summary>
    [ObservableProperty]
    public partial double Gamma { get; set; } = 5;

    /// <summary>
    /// 请求单张图像
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Mat? QueryImage(int index)
    {
        if (index >= Images?.Length) return null;

        var mat = Images![index];

        Serilog.Log.Debug($"Current Mat Index: {ImageIndex}\n" +
                          $"Scale: {Scale}\n" +
                          $"Contrast: {Contrast}\n" +
                          $"Brightness: {Brightness}\n" +
                          $"LeftLevel: {LeftLevel}\n" +
                          $"RightLevel: {RightLevel}\n" +
                          $"Gamma: {Gamma}\n" +
                          $"Mat Type: {mat.Type()}\n" +
                          $"Mat Depth: {mat.Depth()}\n" +
                          $"Mat Channels: {mat.Channels()}\n" +
                          $"Mat Size: {mat.Size()}\n");


        // note 大概率是错的 要改
        // converter to u8
        if (mat.Depth() != MatType.CV_8U)
            mat = mat.ToU8();

        // scale
        mat = mat.Scale(Scale);

        // left and right range
        mat = mat.RangeLevel(LeftLevel, RightLevel);

        // brightness
        if (Math.Abs(Contrast) > 0.0001 || Brightness != 0)
            mat = mat.Adjust(Contrast, Brightness);

        // gamma
        if (Gamma is > 0 or <= 10)
            mat = mat.Gamma(Gamma);

        return mat;
    }

    public Mat?[] QueryImages()
        => Enumerable.Range(0, Images?.Length ?? 0).Select(QueryImage).ToArray();
}