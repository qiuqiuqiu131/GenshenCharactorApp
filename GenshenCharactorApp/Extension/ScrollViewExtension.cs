using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace GenshenCharactorApp.Extension
{
    public static class ScrollViewExtension
    {
        public static readonly DependencyProperty MyHorizontalOffsetProperty = DependencyProperty.RegisterAttached("MyHorizontalOffset", typeof(double), typeof(ScrollViewExtension), new UIPropertyMetadata(0.0, OnMyHorizontalOffsetChanged));
        public static void SetMyHorizontalOffset(FrameworkElement target, double value) => target.SetValue(MyHorizontalOffsetProperty, value);
        public static double GetMyHorizontalOffset(FrameworkElement target) => (double)target.GetValue(MyHorizontalOffsetProperty);
        private static void OnMyHorizontalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e) => (target as ScrollViewer)?.ScrollToHorizontalOffset((double)e.NewValue);

        public static readonly DependencyProperty MyVerticalOffsetProperty = DependencyProperty.RegisterAttached("MyVerticalOffset", typeof(double), typeof(ScrollViewExtension), new UIPropertyMetadata(0.0, OnMyVerticalOffsetChanged));
        public static void SetMyVerticalOffset(FrameworkElement target, double value) => target.SetValue(MyVerticalOffsetProperty, value);
        public static double GetMyVerticalOffset(FrameworkElement target) => (double)target.GetValue(MyVerticalOffsetProperty);
        private static void OnMyVerticalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e) => (target as ScrollViewer)?.ScrollToVerticalOffset((double)e.NewValue);
    }
}
