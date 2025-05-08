using Prism.Common;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GenshenApp.UserControls
{
    /// <summary>
    /// CharactorList.xaml 的交互逻辑
    /// </summary>
    public partial class CharactorList : UserControl
    {
        private bool isMoving = false;
        private Storyboard sb = new();
        private DoubleAnimation anim = new();
        private ScrollViewer scrollViewer;

        public CharactorList()
        {
            InitializeComponent();

            Loaded += CharactorList_Loaded;
        }

        private void CharactorList_Loaded(object sender, RoutedEventArgs e)
        {
            scrollViewer = GetVisualChild<ScrollViewer>(charactorList);
            InitStoryBoard(scrollViewer);
        }

        private void CharactorList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (charactorList.SelectedItem == null || charactorList.Items.Count == 0 || isMoving) return;

            isMoving = true;

            double viewWidth;
            double itemWidth;

            // item元素
            var item = (ListBoxItem)charactorList.ItemContainerGenerator.ContainerFromIndex(0);
            itemWidth = item.ActualWidth;

            viewWidth = scrollViewer.ActualWidth;

            double itemPosX = charactorList.SelectedIndex * itemWidth;

            double horOffset = itemPosX + itemWidth - viewWidth / 2;
            
            anim.To = horOffset;
            sb.Begin();
        }

        private void InitStoryBoard(DependencyObject target)
        {
            anim.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200));
            anim.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut };
            Storyboard.SetTarget(anim, target);
            Storyboard.SetTargetProperty(anim, new PropertyPath("(ext:ScrollViewExtension.MyHorizontalOffset)"));
            sb.Children.Add(anim);
            sb.Completed += (_,_) => isMoving = false;
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
