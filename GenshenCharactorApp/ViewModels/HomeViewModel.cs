using GenshenCharactorApp.Common;
using GenshenCharactorApp.Common.JosnData;
using GenshenCharactorApp.Helper;
using GenshenCharactorApp.Services.Interface;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GenshenCharactorApp.ViewModels
{
    public class HomeViewModel:BindableBase
    {
        private readonly IRegionManager regionManager;
        private readonly ILoadDataService loadDataService;
        private ProgramData programData;
        private ProgramSettingData settingData;

        public string HomeVideoUrl => settingData.HomeVideoUrl;
        public List<CityData> CityDatas => programData.CityData;

        private ObservableCollection<string> newBars = new()
        {
            "最新","新闻","公告","活动"
        };
        public ObservableCollection<string> NewBars => newBars;

        private ObservableCollection<NewData> listItems;
        public ObservableCollection<NewData> ListItems
        {
            get => listItems;
            set => SetProperty(ref listItems, value);
        }


        public DelegateCommand<CityData> ItemClickCommand { get; private set; }
        public DelegateCommand NewsDetailClickCommand {  get; private set; }
        public DelegateCommand<string> NewsSelectionChangedCommand {  get; private set; }
        public DelegateCommand<string> NewClickCommand { get; private set; }

        public HomeViewModel(IRegionManager regionManager,ILoadDataService loadDataService) 
        {
            this.regionManager = regionManager;
            this.loadDataService = loadDataService;

            settingData = (Application.Current.MainWindow.DataContext as MainWindowViewModel).SettingData;
            programData = (Application.Current.MainWindow.DataContext as MainWindowViewModel).ProgramData;

            ItemClickCommand = new DelegateCommand<CityData>(ItemClick);
            NewsDetailClickCommand = new DelegateCommand(NewsDetailClick);
            NewsSelectionChangedCommand = new DelegateCommand<string>(NewsSelectionChanged);
            NewClickCommand = new DelegateCommand<string>(NewClick);

            loadDataService.LoadJsonBaseData<NewData>(settingData.NewDataUrl, 1, 5, -1).ContinueWith(result=>
            {
                ListItems = new ObservableCollection<NewData>(result.Result);
            });
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
    }
}
