using DryIoc;
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
using GenshenApp.ViewModels;

namespace GenshenApp.Views
{
    /// <summary>
    /// NewsView.xaml 的交互逻辑
    /// </summary>
    public partial class NewsView : UserControl
    {
        private bool isMoving;
        private bool cantMove;
        private bool isTop = true;

        private DoubleAnimation ani = new();
        private Storyboard sb = new();

        public NewsView()
        {
            InitializeComponent();

            Loaded += NewsView_Loaded;
            Unloaded += (s, e) => SetScrollViewerOffset(0);

            TopButton.TopEvent += TopButton_TopEvent;

            InitStoryBoard();
        }

        private void TopButton_TopEvent()
        {
            SetScrollViewerOffset(0);
        }

        private void NewsView_Loaded(object sender, RoutedEventArgs e)
        {
            (DataContext as NewsViewModel).MoreDetailLoadOver += NewsView_MoreDetailLoadOver;
        }

        private void NewsView_MoreDetailLoadOver()
        {
            NewsList.button.IsEnabled = true;
        }

        private void InitStoryBoard()
        {
            ani.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200));
            ani.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut };
            Storyboard.SetTarget(ani, scrollView);
            Storyboard.SetTargetProperty(ani, new PropertyPath("(ext:ScrollViewExtension.MyVerticalOffset)"));
            sb.Children.Add(ani);
            sb.Completed += (s, e) => { isMoving = false; };
        }

        private void SetScrollViewerOffset(double value)
        {
            sb.Stop();

            ani.To = value;
            sb.Begin();

            if (value > 800 && isTop)
            {
                isTop = false;
                TopButton.Visibility = Visibility.Visible;
            }
            else if (value < 700 && !isTop)
            {
                isTop = true;
                TopButton.Visibility = Visibility.Hidden;
            }
        }

        private void scrollView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double dis;
            if (e.Delta > 0)
                dis = -300;
            else
                dis = 300;
            SetScrollViewerOffset(scrollView.VerticalOffset + dis);
            e.Handled = true;
        }
    }
}
