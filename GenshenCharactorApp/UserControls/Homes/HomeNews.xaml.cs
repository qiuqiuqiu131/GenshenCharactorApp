using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace GenshenCharactorApp.UserControls
{
    /// <summary>
    /// HomeNews.xaml 的交互逻辑
    /// </summary>
    public partial class HomeNews : UserControl
    {
        private int currentIndex = 0;

        private DoubleAnimation ani = new();
        private Storyboard sb = new();

        private bool isSelctionChanged = false;

        public HomeNews()
        {
            InitializeComponent();

            InitStoryBoard();

            Loaded += HomeNews_Loaded;
        }

        private void HomeNews_Loaded(object sender, RoutedEventArgs e)
        {
            PlayAnim();
        }

        private async void PlayAnim()
        {
            await Task.Delay(new TimeSpan(0, 0, 7));

            if(isSelctionChanged)
            {
                isSelctionChanged = false;
                return;
            }

            currentIndex++;
            if (currentIndex == 4)
                currentIndex = 0;
            Listbox.SelectedIndex = currentIndex;
            SetScrollViewerOffset(currentIndex * 550);
            PlayAnim();

        }

        private void InitStoryBoard()
        {
            ani.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));
            ani.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut };
            Storyboard.SetTarget(ani, scrollView);
            Storyboard.SetTargetProperty(ani, new PropertyPath("(ext:ScrollViewExtension.MyHorizontalOffset)"));
            sb.Children.Add(ani);
        }

        private void SetScrollViewerOffset(double value)
        {
            ani.To = value;
            sb.Begin();
        }

        private void Listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isSelctionChanged = true;

            currentIndex = Listbox.SelectedIndex;
            SetScrollViewerOffset(currentIndex * 550);
            PlayAnim();
        }
    }
}
