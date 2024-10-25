using DryIoc;
using GenshenApp.Events;
using Prism.Events;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            SetScrollViewerOffset(scrollView.VerticalOffset + dis);
            e.Handled = true;
        }

        private void InitStoryBoard()
        {
            ani.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200));
            ani.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut };
            Storyboard.SetTarget(ani, scrollView);
            Storyboard.SetTargetProperty(ani, new PropertyPath("(ext:ScrollViewExtension.MyVerticalOffset)"));
            sb.Children.Add(ani);
        }

        private void SetScrollViewerOffset(double value)
        {
            ani.To = value;
            sb.Begin();
        }
    }
}
