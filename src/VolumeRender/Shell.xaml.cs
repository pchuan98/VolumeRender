using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using OpenCvSharp;
using VolumeRender.Models;
using VolumeRender.ViewModels;

namespace VolumeRender;

/// <summary>
/// Interaction logic for Shell.xaml
/// </summary>
public partial class Shell
{
    public Shell()
    {
        InitializeComponent();

        Viewport.FixedRotationPointEnabled = true;
        Viewport.FixedRotationPoint = new Point3D(0, 0, 0);

        Cv2.ImReadMulti(@"D:\.test\pollen_8bit.tif", out var mats, ImreadModes.Unchanged);
        DataContext = new ShellViewModel()
        {
            ImagesObj = new ImagesObject()
            {
                Images = mats
            }
        };
    }

    private int index = 0;

    private void MenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        Mat[] mats;

        var value = index++ % 5;

        switch (value)
        {
            case 0:
                Cv2.ImReadMulti(@"D:\.test\motic\c0r.tif", out mats, ImreadModes.Unchanged);
                break;
            case 1:
                Cv2.ImReadMulti(@"D:\.test\motic\c1.tif", out mats, ImreadModes.Unchanged);
                break;
            case 2:
                Cv2.ImReadMulti(@"D:\.test\profiling001.tif", out mats, ImreadModes.Unchanged);
                break;
            case 3:
                Cv2.ImReadMulti(@"D:\.test\pollen_8bit.tif", out mats, ImreadModes.Unchanged);
                break;
            case 4:
                Cv2.ImReadMulti(@"D:\.test\profiling002.tif", out mats, ImreadModes.Unchanged);
                break;
            default:
                throw new Exception();
        }

        DataContext = new ShellViewModel()
        {
            ImagesObj = new ImagesObject()
            {
                Images = mats
            }
        };
    }
}