using Prism.Common;
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
using System.Windows.Threading;

namespace GenshenApp.UserControls
{
    /// <summary>
    /// CharactorList.xaml 的交互逻辑
    /// </summary>
    public partial class CharactorList : UserControl
    {
        public CharactorList()
        {
            InitializeComponent();
        }

        private void CharactorList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (charactorList.SelectedItem == null || charactorList.Items.Count == 0) return;

            double viewWidth;
            double itemWidth;

            // item元素
            var item = (ListBoxItem)charactorList.ItemContainerGenerator.ContainerFromIndex(0);
            itemWidth = item.ActualWidth;

            var scrollView = GetVisualChild<ScrollViewer>(charactorList);
            viewWidth = scrollView.ActualWidth;

            double itemPosX = charactorList.SelectedIndex * itemWidth;

            double horOffset = itemPosX + itemWidth - viewWidth / 2;
            GetStoryBoard(scrollView,horOffset).Begin();
        }

        private Storyboard GetStoryBoard(DependencyObject target,double horOffset)
        {
            var horOffsetAnim = new DoubleAnimation();
            var sb = new Storyboard();

            horOffsetAnim.To = horOffset;
            horOffsetAnim.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200));
            horOffsetAnim.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut };
            Storyboard.SetTarget(horOffsetAnim, target);
            Storyboard.SetTargetProperty(horOffsetAnim, new PropertyPath("(ext:ScrollViewExtension.MyHorizontalOffset)"));
            sb.Children.Add(horOffsetAnim);

            return sb;
        }

        private T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                {
                    return typedChild;
                }

                var childOfChild = GetVisualChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }
    }
}
