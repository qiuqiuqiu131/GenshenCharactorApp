using GenshenApp.Events;
using Prism.Events;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace GenshenApp.UserControls.Dialog
{
    /// <summary>
    /// WorldDetailView.xaml 的交互逻辑
    /// </summary>
    public partial class WorldDetailView : UserControl
    {
        private readonly IEventAggregator eventAggregator;

        private DoubleAnimation ani = new();
        private Storyboard sb = new();

        private double currentPos;

        public WorldDetailView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            this.eventAggregator = eventAggregator;

            InitStoryBoard();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            eventAggregator.GetEvent<DialogHide>().Publish();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double dis;
            if (e.Delta > 0)
                dis = -350;
            else
                dis = 350;
            double value = currentPos + dis;
            if (value < 0) value = 0;
            if (value > scrollView.ScrollableHeight) value = scrollView.ScrollableHeight;
            SetScrollViewerOffset(value);
            e.Handled = true;
        }

        private void InitStoryBoard()
        {
            ani.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200));
            ani.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(ani, scrollView);
            Storyboard.SetTargetProperty(ani, new PropertyPath("(ext:ScrollViewExtension.MyVerticalOffset)"));
            sb.Children.Add(ani);
        }

        private void SetScrollViewerOffset(double value)
        {
            currentPos = value;
            
            ani.To = value;
            sb.Begin();
        }
    }
}
