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

namespace GenshenApp.UserControls
{
    /// <summary>
    /// MyDialogView.xaml 的交互逻辑
    /// </summary>
    public partial class MyDialogView : UserControl
    {
        private readonly IEventAggregator eventAggregator;

        private readonly Storyboard sb1;
        private readonly Storyboard sb2;

        public event Action HideOverEvent;
        public event Action ShowOverEvent;

        public MyDialogView(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.eventAggregator = eventAggregator;

            sb1 = (Storyboard)this.Resources["Show"];
            sb1.Completed += delegate { ShowOverEvent?.Invoke(); };

            sb2 = (Storyboard)this.Resources["Hide"];
            sb2.Completed += delegate { HideOverEvent?.Invoke(); DiagContent = null; };
        }
        
        public object DiagContent
        {
            get => DialogContent.Content;
            set => DialogContent.Content = value;
        }

        public void Show()
        {
            sb1.Begin();
        }

        public void Hide()
        {
            sb2.Begin();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //eventAggregator.GetEvent<DialogHide>().Publish();
        }
    }
}
