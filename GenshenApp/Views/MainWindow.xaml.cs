using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Media;
using GenshenApp.UserControls;
using GenshenApp.ViewModels;
using Prism.Events;
using GenshenApp.Events;
using System.Windows.Media.Animation;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows.Controls;
using Prism.Ioc;

namespace GenshenApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MediaPlayer Bg1 = new MediaPlayer();
        private readonly MediaPlayer Bg2 = new MediaPlayer();

        private readonly IContainerProvider container;
        private readonly IEventAggregator eventAggregator;

        public MainWindow(IContainerProvider container,
            IEventAggregator eventAggregator)
        {
            InitializeComponent();
            this.container = container;
            this.eventAggregator = eventAggregator;

            eventAggregator.GetEvent<InitOver>().Subscribe(InitOver);
            eventAggregator.GetEvent<HttpFailed>().Subscribe(HttpFailed);

            TopPanel.StopAudioEvent += AudioStop;
            TopPanel.PlayAudioEvent += AudioPlay;

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadingContent.Visibility = Visibility.Visible;
            LoadingContent.Opacity = 1;
            LoadingContent.Content = container.Resolve<Loading>();
        }

        private void InitOver()
        {
            PlayAudio();
            Storyboard storyboard = new Storyboard();
            DoubleAnimation anim = new DoubleAnimation();
            anim.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));
            Storyboard.SetTarget(anim, LoadingContent);
            Storyboard.SetTargetProperty(anim, new PropertyPath("Opacity"));
            storyboard.Children.Add(anim);
            storyboard.FillBehavior = FillBehavior.Stop;

            anim.To = 0;
            storyboard.Completed += (o, s) =>
            {
                LoadingContent.Visibility = Visibility.Collapsed;
                LoadingContent.Content = null;
            };
            storyboard.Begin();
        }

        #region Audio
        private void PlayAudio()
        {
            var url1 = ((DataContext as MainWindowViewModel)!).SettingData.Bg1Url;
            Bg1.Open(new Uri(url1, UriKind.Absolute));
            Bg1.Volume = 1;
            Bg1.Play();
            Bg1.MediaEnded += (_, _) =>
            {
                Bg1.Position = TimeSpan.Zero;
                Bg1.Play();
            };

            var url2 = ((DataContext as MainWindowViewModel)!).SettingData.Bg2Url;
            Bg2.Open(new Uri(url2, UriKind.Absolute));
            Bg2.Volume = 1;
            Bg2.Play();
            Bg2.MediaEnded += (_, _) =>
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
        #endregion


        public void HttpFailed()
        {
            eventAggregator.GetEvent<HttpFailed>().Unsubscribe(HttpFailed);

            LoadingContent.Visibility = Visibility.Visible;
            LoadingContent.Opacity = 1;
            LoadingContent.Content = container.Resolve<FailedView>();  
        }

        public void Quit()
        {
            Application.Current.Shutdown();
        }
    }
}
