using System.Windows.Controls;
using OpenCvSharp;

namespace VolumeRender.Extensions;

public static class MatExtension
{
    public static Mat ToU8(this Mat mat)
    {
        Mat dst;

        if (mat.Depth() == 0)
            dst = mat;
        else if (mat.Depth() == 2 && mat.Channels() == 1)
            dst = mat.Normalize(256, 0, NormTypes.MinMax, MatType.CV_8U);
        else throw new ArgumentException();

        return dst;
    }

    /// <summary>
    /// 缩放图形，要求大于0
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    public static Mat Scale(this Mat mat, double scale)
    {
        if (scale <= 0) return mat;

        var dst = new Mat();
        Cv2.Resize(mat, dst, new Size(), scale, scale);

        return dst;
    }

    /// <summary>
    /// y = ax + b
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="contrast"></param>
    /// <param name="brightness"></param>
    /// <returns></returns>
    public static Mat Adjust(this Mat mat, double contrast, double brightness)
    {
        var dst = new Mat();

        if (Math.Abs(contrast - 1) < 0.00001 && brightness == 0) return mat;

        mat.ConvertTo(dst, -1, contrast, brightness);

        return dst;
    }

    /// <summary>
    /// 色阶修改
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Mat RangeLevel(this Mat mat, int left, int right)
    {
        var mask = new Mat();
        Cv2.InRange(mat, new Scalar(left, left, left), new Scalar(right, right, right), mask);

        var dst = new Mat();
        Cv2.BitwiseAnd(mat, mat, dst, mask);

        // resize to 0-255
        dst = dst.Normalize(256, 0, NormTypes.MinMax);

        return dst;
    }

    /// <summary>
    /// 设置图像的gamma值
    ///
    /// 均匀到0到10
    /// </summary>
    /// <param name="img"></param>
    /// <param name="gamma"></param>
    /// <returns></returns>
    public static Mat Gamma(this Mat img, double gamma)
    {
        if (img.Type() != MatType.CV_8UC1) return img;
        if (gamma is <= 0 or > 10) return img;

        var center = 5;

        gamma = gamma <= 1 ? gamma * center : center + (gamma - 1) * (center / 9.0);

        var lut = new Mat(1, 256, MatType.CV_8U);
        for (var i = 0; i < 256; i++)
            lut.Set<byte>(0, i, (byte)(Math.Pow(i / 255.0, gamma) * 255.0));

        var output = new Mat();
        Cv2.LUT(img, lut, output);

        return output;
    }
}