using GenshenCharactorApp.Events;
using GenshenCharactorApp.UserControls;
using GenshenCharactorApp.ViewModels;
using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GenshenCharactorApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer Bg1 = new MediaPlayer();
        private MediaPlayer Bg2 = new MediaPlayer();
        private readonly IRegionManager regionManager;

        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();
            PlayAudio();

            this.regionManager = regionManager;
            TopPanel.StopAudioEvent += AudioStop;
            TopPanel.PlayAudioEvent += AudioPlay;
        }

        private void PlayAudio()
        {
            string url1 = (DataContext as MainWindowViewModel).SettingData.Bg1Url;
            Bg1.Open(new Uri(url1, UriKind.Absolute));
            Bg1.Volume = 1;
            Bg1.Play();
            Bg1.MediaEnded += (o, e) =>
            {
                Bg1.Position = TimeSpan.Zero;
                Bg1.Play();
            };

            string url2 = (DataContext as MainWindowViewModel).SettingData.Bg2Url;
            Bg2.Open(new Uri(url2, UriKind.Absolute));
            Bg2.Volume = 1;
            Bg2.Play();
            Bg2.MediaEnded += (o, e) =>
            {
                Bg2.Position = TimeSpan.Zero;
                Bg2.Play();
            };
        }

        private void AudioStop()
        {
            Bg1.Volume = 0;
            Bg2.Volume = 0;
        }

        private void AudioPlay()
        {
            Bg1.Volume = 1;
            Bg2.Volume = 1;
        }
    
        public void HttpFailed()
        {
            TopPanel.Opacity = 0;
            regionManager.Regions["MainViewRegion"].RequestNavigate("FailedView");
        }

        public void Quit()
        {
            Application.Current.Shutdown();
        }
    }
}
