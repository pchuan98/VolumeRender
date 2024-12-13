using System.Diagnostics;
using System.Drawing.Imaging;
using System.Numerics;
using System.Windows.Media.Media3D;
using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Utilities;
using OpenCvSharp;
using SharpDX;
using SharpDX.Direct3D9;
using VolumeRender.Extensions;
using Camera = HelixToolkit.Wpf.SharpDX.Camera;
using Material = HelixToolkit.Wpf.SharpDX.Material;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
using PerspectiveCamera = HelixToolkit.Wpf.SharpDX.PerspectiveCamera;

namespace VolumeRender.Models;

public partial class ViewportModel : ObservableObject
{
    [ObservableProperty]
    public partial Camera PerspectiveCamera { get; set; }
        = new PerspectiveCamera()
        {
            Position = new Point3D(0, 0, 1),
            LookDirection = new Vector3D(0, 0, -1),
            UpDirection = new Vector3D(0, 1, 0),
            NearPlaneDistance = 0.01,
            FarPlaneDistance = 1000,
            FieldOfView = 45,
        };

    [ObservableProperty]
    public partial EffectsManager Effects { get; set; } = new DefaultEffectsManager();

    [ObservableProperty]
    public partial Transform3D Transform { get; set; } = Transform3D.Identity;

    [ObservableProperty]
    public partial Material? Material { get; set; } = null;

    public void Generate(IEnumerable<Mat> mats)
    {
        Transform = new ScaleTransform3D(1, 0.7, 0.5);

        //return;

        var src = mats.Select(item => item.ToF32()).ToArray();

        var width = src[0].Cols;
        var height = src[0].Rows;
        var depth = src.Length;

        Serilog.Log.Debug("Render Model Data Size: {width},{height},{depth}", width, height, depth);

        src.ToFloatArray(out var array);

        var points = DoNothing(array, width, height, depth);
        //var points = VolumeDataHelper.GenerateGradients(array, width, height, depth, 1);
        //var points = VolumeDataHelper.DoNothing(array, width, height, depth, 1f, 1, 1);
        //var index = 1000;
        //foreach (var point in points)
        //{
        //    if (index < 0) break;

        //    if (point.W <= 0)
        //    {
        //        index--;
        //        Debug.WriteLine($"{point.X},{point.Y},{point.Z},{point.W}");
        //    }
        //}

        var level = 1000;

        Material = new VolumeTextureDiffuseMaterial()
        {
            Texture = new VolumeTextureGradientParams(points, src[0].Width, src[0].Height, src.Length),
            SampleDistance = 1,
            EnablePlaneAlignment = true,
            //Color = new Color4(1, 1, 1, 0.01f),
            TransferMap = Enumerable.Range(0, level).Select(item =>
            {
                var value = 1f * MathF.Pow(item / (float)level, 2);
                return new Color4(0, value, 0, value);
            }).ToArray()
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="depth"></param>
    /// <param name="thread"></param>
    /// <returns></returns>
    public Half4[] DoNothing(float[] data, int width, int height, int depth, int thread = 32)
    {
        var gradients = new Half4[data.Length];

        var halfx = width / 2f;
        var halfy = height / 2f;
        var halfz = depth / 2f;

        Parallel.For(0, depth, new ParallelOptions() { MaxDegreeOfParallelism = thread }, (z) =>
        {
            var index = z * width * height;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++, ++index)
                {
                    gradients[index] = new Half4((x - halfx) / halfx, (y - halfy) / halfy, (z - halfz) / halfz, data[index]);
                }
            }
        });

        return gradients;
    }
}