using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        #region Property
        private ProgramData programData;
        private ProgramData ProgramData
        {
            get => programData;
            set
            {
                if (programData != value)
                {
                    programData = value;
                    RaisePropertyChanged(nameof(CityDatas));
                }
            }
        }

        private ProgramSettingData settingData;

        // 主界面Video
        public string HomeVideoUrl => settingData.HomeVideoUrl;
        
        // 区域数据
        public List<CityData> CityDatas => programData.CityData;

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
        private ObservableCollection<HomeNewData> homeNewDatas;
        public ObservableCollection<HomeNewData> HomeNewDatas
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

        public HomeViewModel(IRegionManager regionManager,ILoadDataService loadDataService) 
        {
            this.regionManager = regionManager;
            this.loadDataService = loadDataService;

            ItemClickCommand = new DelegateCommand<CityData>(ItemClick);
            NewsDetailClickCommand = new DelegateCommand(NewsDetailClick);
            NewsSelectionChangedCommand = new DelegateCommand<string>(NewsSelectionChanged);
            NewClickCommand = new DelegateCommand<string>(NewClick);
            HomeNewClickCommand = new DelegateCommand<string>(HomeNewClick);

            settingData = (Application.Current.MainWindow.DataContext as MainWindowViewModel).SettingData;
            ProgramData = (Application.Current.MainWindow.DataContext as MainWindowViewModel).ProgramData;
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
            ProgramData = (Application.Current.MainWindow.DataContext as MainWindowViewModel).ProgramData;

            loadDataService.LoadJsonBaseData<NewData>(settingData.NewDataUrl, 1, 5, -1).ContinueWith(result =>
            {
                ListItems = new ObservableCollection<NewData>(result.Result);
            });

            loadDataService.LoadJsonBaseData<HomeNewData>(settingData.HomeNewsDataUrl, 1, 4, -1).ContinueWith(result =>
            {
                HomeNewDatas = new ObservableCollection<HomeNewData>(result.Result);
            });
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}
