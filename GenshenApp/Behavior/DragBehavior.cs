using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GenshenApp.Behavior
{
    public class DragBehavior:Behavior<FrameworkElement>
    {
        public Window Target
        {
            get { return (Window)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(Window), typeof(DragBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            Target.MouseDown += MouseDown;
        }

        protected override void OnDetaching()
        {
            Target.MouseDown -= MouseDown;
        }

        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Target.DragMove();
            }
        }
    }
}
