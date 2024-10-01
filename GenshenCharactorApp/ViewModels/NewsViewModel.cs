using GenshenCharactorApp.Common;
using GenshenCharactorApp.Common.JosnData;
using GenshenCharactorApp.Helper;
using GenshenCharactorApp.Services.Interface;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GenshenCharactorApp.ViewModels
{
    public class NewsViewModel : BindableBase, INavigationAware
    {
        #region Property
        private ObservableCollection<NewData> newTops;
        public ObservableCollection<NewData> NewTops
        {
            get => newTops;
            set => SetProperty(ref newTops, value);
        }

        private ObservableCollection<RatioData> newBars = new()
        {
            new(){IsChecked = true,Text="最新"},
            new(){IsChecked = false,Text="新闻"},
            new(){IsChecked = false,Text="公告"},
            new(){IsChecked = false,Text="活动"}
        };
        public ObservableCollection<RatioData> NewBars => newBars;

        private ObservableCollection<NewData> listItems;
        public ObservableCollection<NewData> ListItems
        {
            get => listItems;
            set => SetProperty(ref listItems, value);
        }

        private string currentOption;
        public string CurrentOption
        {
            get => currentOption;
            private set => SetProperty(ref currentOption, value);
        }
        #endregion

        private int pageIndex = 1;

        public DelegateCommand<string> NewsOptionChangedCommand {  get; private set; }
        public DelegateCommand MoreNewCommand {  get; private set; }
        public DelegateCommand<string> NewClickCommand {  get; private set; }

        private ProgramSettingData settingData;

        private Dictionary<string, List<NewData>> newDatas = new Dictionary<string, List<NewData>>
        {
            {"最新",new()},
            {"新闻",new()},
            {"公告",new()},
            {"活动",new()}
        };

        public event Action MoreDetailLoadOver;

        private readonly ILoadDataService loadDataService;

        public NewsViewModel(ILoadDataService loadDataService)
        {
            this.loadDataService = loadDataService;

            settingData = (Application.Current.MainWindow.DataContext as MainWindowViewModel).SettingData;

            NewsOptionChangedCommand = new DelegateCommand<string>(NewsOptionChanged);
            MoreNewCommand = new DelegateCommand(MoreNew);
            NewClickCommand = new DelegateCommand<string>(NewClick);

            InitData();
        }

        private void InitData()
        {
            currentOption = "最新";

            loadDataService.LoadJsonBaseData<NewData>(settingData.NewTopDataUrl, 1, 3, -1).ContinueWith(result =>
            {
                NewTops = new ObservableCollection<NewData>(result.Result);
            });

            loadDataService.LoadJsonBaseData<NewData>(settingData.NewDataUrl, 1, 5, -1).ContinueWith(result =>
            {
                ListItems = new ObservableCollection<NewData>(result.Result);
            });
        }

        private async void MoreNew()
        {
            string chanId = ParseChanId(currentOption);
            if (chanId == string.Empty)
                return;

            pageIndex++;
            List<NewData> result;
            if(settingData.LowMemoryMode)
                result = await loadDataService.LoadJsonBaseData<NewData>(chanId, pageIndex, 5, -1);
            else
            {
                if (newDatas[CurrentOption].Count >= pageIndex * 5)
                    result = newDatas[CurrentOption].Skip((pageIndex-1)*5).Take(5).ToList();
                else
                {
                    result = await loadDataService.LoadJsonBaseData<NewData>(chanId, pageIndex, 5, -1);
                    lock (newDatas[currentOption])
                    {
                        if (newDatas[currentOption].Count < pageIndex * 5)
                            newDatas[currentOption].AddRange(result);
                    }
                }
            }
            ListItems.AddRange(result);
            result = null;
            MoreDetailLoadOver?.Invoke();
        }

        private void NewsOptionChanged(string obj)
        {
            string chanId = ParseChanId(obj);
            if (chanId == string.Empty)
                return;

            CurrentOption = obj;
            pageIndex = 1;

            if (newDatas[currentOption].Count == 0)
            {
                loadDataService.LoadJsonBaseData<NewData>(chanId, 1, 5, -1).ContinueWith(result =>
                {
                    ListItems = new ObservableCollection<NewData>(result.Result);
                    if(!settingData.LowMemoryMode)
                        newDatas[currentOption].AddRange(result.Result);
                });
            }
            else
                ListItems = new ObservableCollection<NewData>(ListItems.Take(5));
        }

        private string ParseChanId(string option)
        {
            string chanId = option switch
            {
                "最新" => settingData.NewDataUrl,
                "新闻" => settingData.NewsDataUrl,
                "公告" => settingData.NoticeDataUrl,
                "活动" => settingData.ActiveDataUrl,
                _ => string.Empty,
            };
            return chanId;
        }

        private void NewClick(string obj)
        {
            HttpHelper.OpenWebPage("https://ys.mihoyo.com/main/news/detail/" + obj);
        }

        #region INavigateAware
        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            // todo 更新新闻列表
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }
        #endregion
    }


    public class RatioData
    {
        public string Text { get; set; }
        public bool IsChecked { get; set; }
    }
}
