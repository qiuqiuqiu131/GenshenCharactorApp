using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;
using GenshenApp.Common;
using GenshenApp.Common.JosnData;
using GenshenApp.Helper;
using GenshenApp.Services.Interface;
using Prism.Regions;
using Prism.Events;
using GenshenApp.Events;
using System.Diagnostics;

namespace GenshenApp.ViewModels
{
    public class CharactorViewModel:BindableBase,INavigationAware
    {
        private readonly ILoadDataService loadDataService;
        private readonly IEventAggregator eventAggregator;
        private readonly IBitmapImageManager bitmapImageManager;
        private readonly IProgramDataService programDataService;

        public ProgramData programData
            => programDataService.ProgramData;

        private ProgramSettingData settingData
            => programDataService.SettingData;

        #region Property
        public List<CityData> CityDatas => programData.CityData;


        // 角色数据数组
        private ObservableCollection<CharactorData> charaList = new();
        public ObservableCollection<CharactorData> CharaList
        {
            get => charaList;
            set => SetProperty(ref charaList, value);
        }

        // 选中的角色
        private CharactorData selectedChara;
        public CharactorData SelectedChara
        {
            get => selectedChara;
            set
            {
                if(selectedChara ==  value) return;

                PreCharactorImage = CharactorData?.CharactorImage;

                SetProperty(ref selectedChara, value);

                UpdateCharactor();
            }
        }

        // 完整的角色数据
        private CharactorFullData charactorData;
        public CharactorFullData CharactorData
        {
            get => charactorData;
            set
            {
                SetProperty(ref charactorData, value);
                RaisePropertyChanged(nameof(Audios));
                RaisePropertyChanged(nameof(CV));
            }
        }

        private BitmapImage preCharactorImage;
        public BitmapImage PreCharactorImage
        {
            get => preCharactorImage;
            set => SetProperty(ref preCharactorImage, value);
        }

        // 选中的城市
        private CityData selectedCity;
        public CityData SelectedCity
        {
            get => selectedCity;
            set
            {
                if(selectedCity != value)
                {
                    SetProperty(ref selectedCity, value);
                    SelectCity(selectedCity);
                }
            }
        }

        // 背景图1
        private BitmapImage background1;
        public BitmapImage Background1
        {
            get => background1;
            set => SetProperty(ref background1, value);
        }

        // 背景图2
        private BitmapImage background2;
        public BitmapImage Background2
        {
            get => background2;
            set => SetProperty(ref background2, value);
        }

        private bool isChina = true;
        public bool IsChina
        {
            get => isChina;
            set
            {
                SetProperty(ref isChina, value);
                RaisePropertyChanged(nameof(Audios));
                RaisePropertyChanged(nameof(CV));
            }
        }

        public List<string> Audios => IsChina ? CharactorData?.Audios_cn : CharactorData?.Audios_jp;

        public string CV => IsChina ? CharactorData?.CV_cn : CharactorData?.CV_jp;

        #endregion

        #region Data
        // 属性图集
        private Dictionary<string, BitmapImage> propertyImages = new ();

        // 角色集合
        private Dictionary<string, CharactorFullData> charactors = new();

        // 背景图集
        private Dictionary<string, BitmapImage> backgroundImages = new();

        private Dictionary<string, List<CharactorData>> charaListDatas = new();

        // 命令
        public DelegateCommand<string> ArrowCommand { get; private init; }
        #endregion

        public event Action CharactorChanged;
        public event Action NavigationChanged;

        public CharactorViewModel(ILoadDataService loadDataService,IEventAggregator eventAggregator,
            IBitmapImageManager bitmapImageManager,
            IProgramDataService programDataService)
        {
            this.loadDataService = loadDataService;
            this.eventAggregator = eventAggregator;
            this.bitmapImageManager = bitmapImageManager;
            this.programDataService = programDataService;

            eventAggregator.GetEvent<HttpFailed>().Subscribe(Free);

            ArrowCommand = new DelegateCommand<string>(Arrow);
        }

        private async void UpdateCharactor()
        {
            if (selectedChara == null)
                return;

            if(settingData.LowMemoryMode)
            {
                var chara = await CreateCharactorFullData(selectedChara);
                CharactorData?.Dispose();
                CharactorData = chara;
                GC.Collect();
            }
            else
            {
                if (charactors.ContainsKey(selectedChara.Name))
                    CharactorData = charactors[selectedChara.Name];
                else
                {
                    var chara = await CreateCharactorFullData(selectedChara);
                    lock (charactors)
                    {
                        if (!charactors.ContainsKey(chara.Name))
                            charactors.Add(chara.Name, chara);
                        else chara = null;
                    }
                    CharactorData = charactors[selectedChara.Name];
                }
            }
            CharactorChanged?.Invoke();
        }

        // 更换区域
        private async void SelectCity(CityData cityData)
        {
            List<Task<Action>> tasks = new List<Task<Action>>
            {
                LoadCityCharactor(cityData.CharactorData),
                LoadBackground1(cityData.BackgroundUrl1),
                LoadBackground2(cityData.BackgroundUrl2)
            };

            await Task.WhenAll(tasks);

            foreach (var task in tasks)
                task.Result?.Invoke();
            tasks.Clear();
        }

        // 加载区域角色
        private async Task<Action> LoadCityCharactor(string ChanId)
        {
            List<CharactorData> results;
            if (!settingData.LowMemoryMode)
            {
                if(charaListDatas.ContainsKey(ChanId))
                    results = charaListDatas[ChanId];
                else
                {
                    results = await loadDataService.LoadJsonBaseData<CharactorData>(ChanId);
                    lock (charaListDatas)
                    {
                        if(!charaListDatas.ContainsKey(ChanId))
                            charaListDatas.Add(ChanId, results);
                       else
                            results = charaListDatas[ChanId];
                    }
                }
            }
            else
                results = await loadDataService.LoadJsonBaseData<CharactorData>(ChanId);

            List<Task> tasks = new List<Task>();

            // 开启线程加载city中的角色数据
            if (!settingData.LowMemoryMode)
            {
                foreach (var chara in results)
                {
                    if (charactors.ContainsKey(chara.Name))
                        continue;
                    Task t = Task.Run(async () =>
                        {
                            var fChara = await CreateCharactorFullData(chara);
                            lock (charactors)
                            {
                                if (!charactors.ContainsKey(fChara.Name))
                                    charactors.Add(fChara.Name, fChara);
                                else
                                    fChara = null;
                            }
                        }
                    );
                    tasks.Add(t);
                }
            }

            await Task.WhenAll(tasks);

            return () =>
            {
                CharaList = new ObservableCollection<CharactorData>(results);
                SelectedChara = CharaList[0];
            };
        }

        // 加载区域背景1
        private async Task<Action> LoadBackground1(string url)
        {
            BitmapImage bitmapImage;
            if (!settingData.LowMemoryMode)
            {
                if (backgroundImages.ContainsKey(url))
                    bitmapImage = backgroundImages[url];
                else
                {
                    bitmapImage = await bitmapImageManager.GetBitmapImage(url);
                    lock(backgroundImages){
                        if(!backgroundImages.ContainsKey(url))
                            backgroundImages.Add(url, bitmapImage);
                        else
                            bitmapImage = backgroundImages[url];
                    }
                }
            }
            else
                bitmapImage = await bitmapImageManager.GetBitmapImage(url);

            return () =>
            {
                Background1 = bitmapImage;
            };
        }

        // 加载区域背景2
        private async Task<Action> LoadBackground2(string url)
        {
            BitmapImage bitmapImage;
            if (!settingData.LowMemoryMode)
            {
                if (backgroundImages.ContainsKey(url))
                    bitmapImage = backgroundImages[url];
                else
                {
                    bitmapImage = await bitmapImageManager.GetBitmapImage(url);
                    lock (backgroundImages)
                    {
                        if (!backgroundImages.ContainsKey(url))
                            backgroundImages.Add(url, bitmapImage);
                        else
                            bitmapImage = backgroundImages[url];
                    }
                }
            }
            else
                bitmapImage = await bitmapImageManager.GetBitmapImage(url);

            return () =>
            {
                Background2 = bitmapImage;
            };
        }

        // 左右切换
        private void Arrow(string type)
        {
            int index = CharaList.IndexOf(SelectedChara);
            switch (type)
            {
                case "left":
                    if (index != 0) index--;
                    else index = CharaList.Count - 1;
                    break;
                case "right":
                    if (index != CharaList.Count - 1) index++;
                    else index = 0;
                    break;
            }
            SelectedChara = CharaList[index];
        }

        private async Task<CharactorFullData> CreateCharactorFullData(CharactorData charactor)
        {
            var fChara = new CharactorFullData();
            fChara.Init(charactor);

            List<Task> tasks = new List<Task> {
                bitmapImageManager.GetBitmapImage(charactor.Icon)
                    .ContinueWith(e => fChara.Icon = e.Result),
                bitmapImageManager.GetBitmapImage(charactor.CharactorImage)
                    .ContinueWith(e => fChara.CharactorImage = e.Result),
                bitmapImageManager.GetBitmapImage(charactor.NameImage)
                    .ContinueWith(e => fChara.NameImage = e.Result),
                bitmapImageManager.GetBitmapImage(charactor.LinesImage)
                    .ContinueWith(e => fChara.LinesImage = e.Result),
            };

            // 属性图
            if (propertyImages.ContainsKey(charactor.PropertyImage))
                fChara.PropertyImage = propertyImages[charactor.PropertyImage];
            else
            {
                tasks.Add(bitmapImageManager.GetBitmapImage(charactor.PropertyImage).ContinueWith(e =>
                {
                    fChara.PropertyImage = e.Result;
                    lock (propertyImages)
                    {
                        if (!propertyImages.ContainsKey(charactor.PropertyImage))
                            propertyImages.Add(charactor.PropertyImage, fChara.PropertyImage);
                        else
                            fChara.PropertyImage = propertyImages[charactor.PropertyImage];
                    }
                }));
            }
            await Task.WhenAll(tasks);

            return fChara;
        }

        #region INavigationAware
        public bool IsNavigationTarget(NavigationContext navigationContext) => true;
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            if (settingData.LowMemoryMode)
                Free();
            GC.Collect();

            NavigationChanged?.Invoke();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            SelectedCity = CityDatas[0];

            var param = navigationContext.Parameters;

            if (param.ContainsKey("city"))
            {
                CityData city = param["city"] as CityData;
                SelectedCity = city;
            }
            if(charaList.Count > 0)
                SelectedChara = CharaList[0];
        }
        #endregion
    
        private void Free()
        {
            propertyImages.Clear();
            charactors.Clear();
            backgroundImages.Clear();
            charaListDatas.Clear();
        }
    }
}
