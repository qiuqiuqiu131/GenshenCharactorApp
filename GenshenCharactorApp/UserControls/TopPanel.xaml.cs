using GenshenCharactorApp.Events;
using GenshenCharactorApp.ViewModels;
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

namespace GenshenCharactorApp.UserControls
{
    /// <summary>
    /// TopPanel.xaml 的交互逻辑
    /// </summary>
    public partial class TopPanel : UserControl
    {
        DoubleAnimation vi = new DoubleAnimation();
        Storyboard sb = new();

        public event Action StopAudioEvent;
        public event Action PlayAudioEvent;

        public TopPanel()
        {
            InitializeComponent();

            InitStoryBoard(rectangle);
        }

        private void InitStoryBoard(Rectangle rect)
        {
            vi.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200));
            vi.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut };

            sb.Children.Add(vi);
            Storyboard.SetTarget(vi, rect);
            Storyboard.SetTargetProperty(vi, new PropertyPath("(Canvas.Left)"));
        }

        private void TopListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ListBox listbox)
                return;

            double dis = listbox.SelectedIndex * 90;
            if (rectangle != null)
            {
                vi.To = dis;
                sb.Begin();
            }
        }

        private void ListBoxItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is not ListBoxItem item)
                return;
            var listbox = ItemsControl.ItemsControlFromItemContainer(item) as ListBox;
            if (listbox == null)
                return;

            double dis = listbox.ItemContainerGenerator.IndexFromContainer(item) * 90;
            if (rectangle != null)
            {
                vi.To = dis;
                sb.Begin();
            }
        }

        private void ListBoxItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is not ListBoxItem item)
                return;
            var listbox = ItemsControl.ItemsControlFromItemContainer(item) as ListBox;
            if (listbox == null)
                return;

            double dis = listbox.SelectedIndex * 90;
            if (rectangle != null)
            {
                vi.To = dis;
                sb.Begin();
            }
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            PlayAudioEvent?.Invoke();
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            StopAudioEvent?.Invoke();
        }
    }
}
