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

        Cv2.ImReadMulti(@"D:\.test\profiling001.tif", out var mats, ImreadModes.Unchanged);


        DataContext = new ShellViewModel()
        {
            ImagesObj = new ImagesObject()
            {
                Images = mats
            }
        };
    }

}