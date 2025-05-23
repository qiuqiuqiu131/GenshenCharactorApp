﻿using Prism.Regions;
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
using System.Runtime.InteropServices;
using GenshenApp.Services.Interface;

namespace GenshenApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IContainerProvider container;
        private readonly IEventAggregator eventAggregator;
        private readonly IProgramDataService programDataService;
        private readonly MediaPlayer Bg1 = new MediaPlayer();
        private readonly MediaPlayer Bg2 = new MediaPlayer();

        private MyDialogView dialogView;

        #region 任务栏
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        #endregion

        public MainWindow(IContainerProvider container,
            IEventAggregator eventAggregator,IProgramDataService programDataService)
        {
            InitializeComponent();
            
            this.container = container;
            this.eventAggregator = eventAggregator;
            this.programDataService = programDataService;

            eventAggregator.GetEvent<InitOver>().Subscribe(InitOver);
            eventAggregator.GetEvent<HttpFailed>().Subscribe(HttpFailed);
            eventAggregator.GetEvent<DialogHide>().Subscribe(DialogHide);
            eventAggregator.GetEvent<DialogShow>().Subscribe(DialogShow);

            TopPanel.StopAudioEvent += AudioStop;
            TopPanel.PlayAudioEvent += AudioPlay;

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadingContent.Visibility = Visibility.Visible;
            LoadingContent.Opacity = 1;
            LoadingContent.Content = container.Resolve<Loading>();

            DialogContent.Content = (dialogView = container.Resolve<MyDialogView>());
            dialogView.HideOverEvent += () => DialogContent.Visibility = Visibility.Collapsed;
        }

        private void InitOver()
        {
            PlayAudio();
            Storyboard storyboard = new Storyboard();
            DoubleAnimation anim = new DoubleAnimation();
            anim.FillBehavior = FillBehavior.Stop;
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
                storyboard = null;
                anim = null;
            };
            storyboard.Begin();
        }

        #region Audio
        private void PlayAudio()
        {
            var url1 = programDataService.SettingData.Bg1Url;
            Bg1.Open(new Uri(url1, UriKind.Absolute));
            Bg1.Volume = 1;
            Bg1.Play();
            Bg1.MediaEnded += (_, _) =>
            {
                Bg1.Position = TimeSpan.Zero;
                Bg1.Play();
            };

            var url2 = programDataService.SettingData.Bg2Url;
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

        #region Dialog
        public void DialogShow(object obj)
        {
            dialogView.DiagContent = obj ;
            DialogContent.Visibility = Visibility.Visible;
            dialogView.Show();
        }

        public void DialogHide()
        {
            dialogView.Hide();
        }
        #endregion

        private void HttpFailed()
        {
            eventAggregator.GetEvent<HttpFailed>().Unsubscribe(HttpFailed);

            LoadingContent.Visibility = Visibility.Visible;
            LoadingContent.Opacity = 1;
            LoadingContent.Content = container.Resolve<FailedView>();  

            GC.Collect();
        }

        public void Quit()
        {
            var story = (Storyboard)this.Resources["HideWindow"];
            if (story != null)
            {
                story.Completed += delegate { Application.Current.Shutdown(); };
                story.Begin(this);
            }
        }

        public void Minimized()
        {
            this.WindowState = WindowState.Minimized;
        }

        private void instance_StateChanged(object sender, EventArgs e)
        {
        }
    }
}
