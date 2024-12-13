using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace VolumeRender.Extensions;

public static class MatsExtension
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mats"></param>
    /// <returns></returns>
    public static (double min, double max) MinMax(this IEnumerable<Mat> mats)
    {
        var array = mats.ToArray();

        var mins = new List<double>();
        var maxs = new List<double>();

        foreach (var mat in array)
        {
            mat.MinMaxLoc(out double min, out double max);

            mins.Add(min);
            maxs.Add(max);
        }

        return (mins.Min(), maxs.Max());
    }

    public static bool SameShape(this Mat[] mats, int thread = 8)
    {
        // todo:后面可以使用多线程+二分法快速完成整个过程

        var isSame = false;
        var count = mats.Length;
        if (count == 0)
            throw new ArgumentException("The count of mats can`t zero.");

        var tp = mats[0].Type();
        var size = mats[0].Size();

        var lSame = new bool[count];
        lSame[0] = true;

        Parallel.For(1, count, new ParallelOptions() { MaxDegreeOfParallelism = thread }, (z) =>
        {
            lSame[z] = mats[z].Type() == tp && mats[z].Size() == size;
        });

        isSame = lSame.All(x => x);
        return isSame;
    }

    /// <summary>
    /// 从左到右，从上到下，从低到高
    /// </summary>
    /// <param name="mats"></param>
    /// <param name="array"></param>
    /// <param name="thread"></param>
    /// <exception cref="System.Exception"></exception>
    public static void ToFloatArray(this Mat[] mats, out float[] array, int thread = 16)
    {
        if (!mats.SameShape())
            throw new ArgumentException("The stack image must the same as each others that type and size.");

        var header = mats[0];

        var width = header.Width;
        var height = header.Height;
        var depth = mats.Length;

        var lenght = width * height * depth;
        var data = new float[lenght];
        var channel = header.Channels();

        if (channel != 1) throw new System.Exception();

        Parallel.For(0, depth, new ParallelOptions() { MaxDegreeOfParallelism = thread }, (z) =>
        {
            var mat = mats[z];
            var index = z * width * height;

            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++, ++index)
                    data[index] = mat.At<float>(y, x);
        });

        array = new float[lenght];
        Array.Copy(data, array, width * height * depth);
    }
}