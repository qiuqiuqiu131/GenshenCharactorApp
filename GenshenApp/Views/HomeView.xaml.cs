using GenshenApp.Events;
using GenshenApp.UserControls;
using Prism.Events;
using Prism.Ioc;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GenshenApp.Views
{
    /// <summary>
    /// HomeView.xaml 的交互逻辑
    /// </summary>
    public partial class HomeView : UserControl
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IContainerProvider container;
        private bool isTop = true;

        private DoubleAnimation ani = new();
        private Storyboard sb = new();

        private double currentPos = 0;

        public HomeView(IEventAggregator eventAggregator,IContainerProvider container)
        {
            InitializeComponent();

            Loaded += HomeView_Loaded;
            Unloaded += (s, e) => SetScrollViewerOffset(0);

            TopButton.TopEvent += TopButton_TopEvent;

            InitStoryBoard();

            this.eventAggregator = eventAggregator;
            this.container = container;
        }

        private void TopButton_TopEvent()
        {
            SetScrollViewerOffset(0);
        }

        private void HomeView_Loaded(object sender, RoutedEventArgs e)
        {
            MyMediaElement.Play();
        }

        private void scrollView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double dis;
            if (e.Delta > 0)
                dis = -200;
            else
                dis = 200;

            var value = currentPos + dis;
            if (value < 0) value = 0;
            if (value > scrollView.ScrollableHeight) value = scrollView.ScrollableHeight;
            
            SetScrollViewerOffset(value);
            e.Handled = true;
        }


        private void MyMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            eventAggregator.GetEvent<InitOver>().Publish();
        }

        private void MyMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            MyMediaElement.Position = TimeSpan.Zero;
            MyMediaElement.Play();
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

            if(value > 800 && isTop)
            {
                isTop = false;
                TopButton.Visibility = Visibility.Visible;
            }
            else if(value < 700 && !isTop)
            {
                isTop = true;
                TopButton.Visibility = Visibility.Hidden;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            object obj = container.Resolve<YearView>();
            eventAggregator.GetEvent<DialogShow>().Publish(obj);
        }
    }
}
