using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using GenshenApp.Common;
using GenshenApp.Common.JosnData;
using GenshenApp.Helper;
using GenshenApp.Services.Interface;

namespace GenshenApp.ViewModels
{
    public class HomeViewModel:BindableBase,INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly ILoadDataService loadDataService;
        private readonly IBitmapImageManager bitmapImageManager;
        private readonly IProgramDataService programDataService;

        private ProgramData programData
            => programDataService.ProgramData;

        private ProgramSettingData settingData
            => programDataService.SettingData;

        #region Property
        // 主界面Video
        public string HomeVideoUrl => settingData.HomeVideoUrl;
        
        // 区域数据
        private List<CityFullData> _cityDatas;

        public List<CityFullData> CityDatas
        {
            get => _cityDatas;
            set => SetProperty(ref _cityDatas, value);
        }

        private BitmapImage newsBackground;
        public BitmapImage NewsBackground
        {
            get => newsBackground;
            set => SetProperty(ref newsBackground, value);
        }
        
        private BitmapImage defaultAreaImage;
        public BitmapImage DefaultAreaImage
        {
            get => defaultAreaImage;
            set => SetProperty(ref defaultAreaImage, value);
        }

        // 新闻bar
        private ObservableCollection<string> newBars = new()
        {
            "最新","新闻","公告","活动"
        };
        public ObservableCollection<string> NewBars => newBars;

        // 展示的新闻
        private ObservableCollection<NewData> listItems;
        public ObservableCollection<NewData> ListItems
        {
            get => listItems;
            set => SetProperty(ref listItems, value);
        }

        // 左侧滚动图标
        private ObservableCollection<HomeNewFullData> homeNewDatas;
        public ObservableCollection<HomeNewFullData> HomeNewDatas
        {
            get => homeNewDatas;
            set => SetProperty(ref homeNewDatas, value);
        }

        public DelegateCommand<CityData> ItemClickCommand { get; private set; }
        public DelegateCommand NewsDetailClickCommand {  get; private set; }
        public DelegateCommand<string> NewsSelectionChangedCommand {  get; private set; }
        public DelegateCommand<string> NewClickCommand { get; private set; }
        public DelegateCommand<string> HomeNewClickCommand { get; private set; }
        #endregion

        public HomeViewModel(IRegionManager regionManager,
            ILoadDataService loadDataService,
            IBitmapImageManager bitmapImageManager,
            IProgramDataService programDataService) 
        {
            this.regionManager = regionManager;
            this.loadDataService = loadDataService;
            this.bitmapImageManager = bitmapImageManager;
            this.programDataService = programDataService;

            ItemClickCommand = new DelegateCommand<CityData>(ItemClick);
            NewsDetailClickCommand = new DelegateCommand(NewsDetailClick);
            NewsSelectionChangedCommand = new DelegateCommand<string>(NewsSelectionChanged);
            NewClickCommand = new DelegateCommand<string>(NewClick);
            HomeNewClickCommand = new DelegateCommand<string>(HomeNewClick);

            InitSources();
        }

        private async void InitSources()
        {
            NewsBackground =
                await bitmapImageManager.GetBitmapImage("https://ys.mihoyo.com/main/_nuxt/img/news_bg.29770c4.jpg");
            DefaultAreaImage =
                await bitmapImageManager.GetBitmapImage("https://ys.mihoyo.com/main/_nuxt/img/c3.9a62be8.jpg");
            
            var cityDatas = new List<CityFullData>();
            foreach (var cityData in programData.CityData)
            {
                var cityFullData = new CityFullData();
                cityFullData.Name = cityData.Name;
                cityFullData.ItemBackground = await bitmapImageManager.GetBitmapImage(cityData.ItemBackground);
                cityFullData.ItemCharactor = await bitmapImageManager.GetBitmapImage(cityData.ItemCharactor);
                cityDatas.Add(cityFullData);
            }

            CityDatas = cityDatas;
        }

        private void NewsSelectionChanged(string obj)
        {
            string chanId = obj switch
            {
                "最新" => settingData.NewDataUrl,
                "新闻" => settingData.NewsDataUrl,
                "公告" => settingData.NoticeDataUrl,
                "活动" => settingData.ActiveDataUrl,
                _ => default
            };

            loadDataService.LoadJsonBaseData<NewData>(chanId, 1, 5, -1).ContinueWith(result =>
            {
                ListItems = new ObservableCollection<NewData>(result.Result);
            });
        }

        private void NewsDetailClick()
        {
            var vm = Application.Current.MainWindow.DataContext as MainWindowViewModel;
            vm.SelectIndex = 1;

            regionManager.Regions["MainViewRegion"].RequestNavigate("NewsView");
        }

        private void ItemClick(CityData data)
        {
            var vm = Application.Current.MainWindow.DataContext as MainWindowViewModel;
            vm.SelectIndex = 2;
            
            regionManager.Regions["MainViewRegion"].RequestNavigate("CharactorView", 
                new NavigationParameters
            {
                { "city", data }
            });
        }

        private void NewClick(string obj)
        {
            HttpHelper.OpenWebPage("https://ys.mihoyo.com/main/news/detail/" + obj);
        }

        private void HomeNewClick(string obj)
        {
            HttpHelper.OpenWebPage(obj);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            loadDataService.LoadJsonBaseData<NewData>(settingData.NewDataUrl, 1, 5, -1).ContinueWith(result =>
            {
                ListItems = new ObservableCollection<NewData>(result.Result);
            });

            loadDataService.LoadJsonBaseData<HomeNewData>(settingData.HomeNewsDataUrl, 1, 4, -1).ContinueWith(result =>
            {
                var homeNews = result.Result;
                var homeNewDatas = new ObservableCollection<HomeNewFullData>();
                foreach (var homeNew in homeNews)
                {
                    var homeFullNew = new HomeNewFullData();
                    homeFullNew.Init(homeNew);
                    homeNewDatas.Add(homeFullNew);
                    Task.Run(async () =>
                    {
                        homeFullNew.Image = await bitmapImageManager.GetBitmapImage(homeNew.Image);
                    });
                }

                HomeNewDatas = homeNewDatas;
            });
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}
