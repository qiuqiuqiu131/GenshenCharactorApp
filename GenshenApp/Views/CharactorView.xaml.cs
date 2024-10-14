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
    /// CharactorView.xaml 的交互逻辑
    /// </summary>
    public partial class CharactorView : UserControl
    {
        private MediaPlayer charactorAudio = new MediaPlayer();

        public CharactorView()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            (DataContext as CharactorViewModel).CharactorChanged += CharactorView_CharactorChanged;
            (DataContext as CharactorViewModel).NavigationChanged += CharactorView_NavigationChanged;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var Audios = (DataContext as CharactorViewModel).Audios;
            if (Audios == null)
                return;

            audioButton.IsEnabled = false;

            Random random = new Random(DateTime.Now.Millisecond);
            string url = Audios[random.Next(0,Audios.Count - 1)];
            charactorAudio.Open(new Uri(url, UriKind.Absolute));
            charactorAudio.Volume = 1;
            charactorAudio.Play();
            charactorAudio.MediaEnded += (o, e) =>
            {
                audioButton.IsEnabled = true;
            };
        }

        private void CharactorView_CharactorChanged()
        {
            (Resources["CharactorImageIn"] as Storyboard).Begin();

            if (!audioButton.IsEnabled)
            {
                charactorAudio.Stop();
                charactorAudio.Position = new TimeSpan(0);
                audioButton.IsEnabled = true;
            }
        }

        private void CharactorView_NavigationChanged()
        {
            if (!audioButton.IsEnabled)
            {
                charactorAudio.Stop();
                charactorAudio.Position = new TimeSpan(0);
                audioButton.IsEnabled = true;
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!audioButton.IsEnabled)
            {
                charactorAudio.Stop();
                charactorAudio.Position = new TimeSpan(0);
                audioButton.IsEnabled = true;
            }
        }
    }
}
