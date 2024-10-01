using GenshenCharactorApp.ViewModels;
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

namespace GenshenCharactorApp.Views
{
    /// <summary>
    /// WorldView.xaml 的交互逻辑
    /// </summary>
    public partial class WorldView : UserControl
    {
        private bool isMoving;

        private DoubleAnimation ani = new();
        private Storyboard sb = new();

        private int selectIndex;
        private int SelectIndex
        {
            get => selectIndex;
            set
            {
                if (selectIndex != value) {
                    if(value == 0)
                    {
                        var story = rightPanel.Resources["Unshow"] as Storyboard;
                        story.Begin();
                    }
                    else if(selectIndex == 0)
                    {
                        var story = rightPanel.Resources["Show"] as Storyboard;
                        story.Begin();
                    }

                    selectIndex = value;
                    double itemHeight = ((FrameworkElement)itemsControl.ItemContainerGenerator.ContainerFromIndex(0))
                        .ActualHeight;
                    SetScrollViewerOffset(selectIndex * itemHeight);
                }
            }
        }
        
        private WorldViewModel vm => DataContext as WorldViewModel;

        public WorldView()
        {
            InitializeComponent();
            InitStoryBoard();

            rightPanel.selectChanged += RightPanel_selectChanged;
        }

        private void RightPanel_selectChanged(int index)
        {
            SelectIndex = index;
        }

        private void InitStoryBoard()
        {
            ani.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 700));
            ani.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut };
            Storyboard.SetTarget(ani,scrollView);
            Storyboard.SetTargetProperty(ani,new PropertyPath("(ext:ScrollViewExtension.MyVerticalOffset)"));
            sb.Children.Add(ani);
            sb.Completed += (s, e) => isMoving = false;
        }

        private void SetScrollViewerOffset(double value)
        {
            isMoving = true;
            ani.To = value;
            sb.Begin();
        }

        private void ScrollView_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (isMoving)
            {
                e.Handled = true;
                return;
            }
            
            if (e.Delta > 0)
            {
                vm.SelectIndex--;
            }
            else if (e.Delta < 0)
            {
                vm.SelectIndex++;
            }

            e.Handled = true;
        }

        private void ScrollView_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (isMoving)
            {
                e.Handled = true;
                return;
            }
            
            if(e.Key == Key.Up)
            {
                vm.SelectIndex--;
            }
            else if(e.Key == Key.Down)
            {
                vm.SelectIndex++;
            }
            
            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            vm.SelectIndex++;
        }
    }
}
