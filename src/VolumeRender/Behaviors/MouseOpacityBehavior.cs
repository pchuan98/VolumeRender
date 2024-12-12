using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Media.Animation;

namespace VolumeRender.Behaviors;

/// <summary>
/// 鼠标移入的时候
/// </summary>
public class MouseOpacityBehavior : Behavior<UIElement>
{
    /// <summary>
    /// 淡入结果透明度
    /// </summary>
    public double OpacityFadeIn { get; set; } = 1;

    /// <summary>
    /// 淡出结果透明度
    /// </summary>
    public double OpacityFadeOut { get; set; } = 0;

    /// <summary>
    /// 淡入开始节点 s
    /// </summary>
    public double StartFadeIn { get; set; } = 1;

    /// <summary>
    /// 淡出开始节点 s
    /// </summary>
    public double StartFadeOut { get; set; } = 0;

    /// <summary>
    /// 淡入动画时间
    /// </summary>
    public double DurationFadeIn { get; set; } = 0.5;

    /// <summary>
    /// 淡出动画时间
    /// </summary>
    public double DurationFadeOut { get; set; } = 1;

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Opacity = 0;

        AssociatedObject.MouseEnter += OnMouseEnter;
        AssociatedObject.MouseLeave += OnMouseLeave;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.MouseEnter -= OnMouseEnter;
        AssociatedObject.MouseLeave -= OnMouseLeave;
    }

    private void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        var fadeInAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.FromSeconds(StartFadeIn),
            To = OpacityFadeIn,
            Duration = TimeSpan.FromSeconds(DurationFadeIn)
        };
        AssociatedObject.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
    }

    private void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        var fadeOutAnimation = new DoubleAnimation
        {
            BeginTime = TimeSpan.FromSeconds(StartFadeOut),
            To = OpacityFadeOut,
            Duration = TimeSpan.FromSeconds(DurationFadeOut)
        };
        AssociatedObject.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
    }
}