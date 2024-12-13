using System.Linq;
using System.Windows.Media.Media3D;
using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Utilities;
using OpenCvSharp;
using SharpDX;
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

    [ObservableProperty] public partial EffectsManager Effects { get; set; } = new DefaultEffectsManager();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Transform))]
    public partial double ScaleTransformX { get; set; } = 1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Transform))]
    public partial double ScaleTransformY { get; set; } = 1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Transform))]
    public partial double ScaleTransformZ { get; set; } = 1;

    public Transform3D Transform => new ScaleTransform3D(ScaleTransformX, ScaleTransformY, ScaleTransformZ);

    [ObservableProperty]
    public partial double MaterialAlpha { get; set; } = 1;

    partial void OnMaterialAlphaChanged(double value)
    {
        if (Material is not VolumeTextureDiffuseMaterial material) return;

        material.TransferMap = MaterialTransferMap;
    }

    [ObservableProperty]
    public partial double IsoValue { get; set; } = 0.1;

    partial void OnIsoValueChanged(double value)
    {
        if (Material is not VolumeTextureDiffuseMaterial material) return;

        material.IsoValue = value;
    }

    public Color4[] MaterialTransferMap
    {
        get
        {
            var scale = 0.5f;
            return Enumerable.Range(0, 2000).Select(item =>
            {
                var materialAlpha = (float)MaterialAlpha * MathF.Pow(item / (float)1000, 2);

                return new Color4(0, materialAlpha, 0, materialAlpha);
            }).ToArray();
        }
    }

    [ObservableProperty]
    public partial Material? Material { get; set; } = null;

    public void Generate(IEnumerable<Mat> mats)
    {
        var src = mats.Select(item => item.ToF32()).ToArray();

        var width = src[0].Cols;
        var height = src[0].Rows;
        var depth = src.Length;

        Serilog.Log.Debug("Render Model Data Size: {width},{height},{depth}", width, height, depth);

        src.ToFloatArray(out var array);

        //var points = DoNothing(array, width, height, depth);
        var points = VolumeDataHelper.GenerateGradients(array, width, height, depth, 1);
        //var points = VolumeDataHelper.DoNothing(array, width, height, depth, 1f, 1, 1);

        Material = new VolumeTextureDiffuseMaterial()
        {
            Texture = new VolumeTextureGradientParams(points, src[0].Width, src[0].Height, src.Length),
            SampleDistance = 1,
            EnablePlaneAlignment = false,
            TransferMap = MaterialTransferMap,
            IsoValue = IsoValue
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

            var k = 0.5f;
            var scale = 1 - k + (1 - k) * MathF.Pow((float)z / depth, 0.5f);

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++, ++index)
                {
                    gradients[index] = new Half4((x - halfx) / halfx, (y - halfy) / halfy, 0.1f * (z - halfz) / halfz, data[index]);
                }
            }
        });

        return gradients;
    }
}