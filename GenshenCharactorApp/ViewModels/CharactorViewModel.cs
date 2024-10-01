using GenshenCharactorApp.Common;
using GenshenCharactorApp.Helper;
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
using GenshenCharactorApp.Services.Interface;
using Prism.Regions;
using GenshenCharactorApp.Common.JosnData;

namespace GenshenCharactorApp.ViewModels
{
    public class CharactorViewModel:BindableBase,INavigationAware
    {
        #region Property
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
        private ProgramData programData;
        private ProgramSettingData settingData;
        public List<CityData> CityDatas => programData.CityData;

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

        private readonly ILoadDataService loadDataService;

        private bool isLoading;

        public event Action CharactorChanged;
        public event Action NavigationChanged;

        public CharactorViewModel(ILoadDataService loadDataService)
        {
            this.loadDataService = loadDataService;

            ArrowCommand = new DelegateCommand<string>(Arrow);

            programData = (Application.Current.MainWindow.DataContext as MainWindowViewModel).ProgramData;
            settingData = (Application.Current.MainWindow.DataContext as MainWindowViewModel).SettingData;

            SelectedCity = CityDatas[0];
        }

        private void InitAllData()
        {
            foreach (var city in CityDatas)
            {
                List<Task<Action>> tasks = new List<Task<Action>>
                {
                    LoadCityCharactor(city.CharactorData),
                    LoadBackground1(city.BackgroundUrl1),
                    LoadBackground2(city.BackgroundUrl2)
                };
            }
        }

        private async void UpdateCharactor()
        {
            if (selectedChara == null)
                return;

            var chara = new CharactorFullData();

            if(settingData.LowMemoryMode)
            {
                await chara.Init(selectedChara, propertyImages);
                CharactorData = chara;
            }
            else
            {
                if (charactors.ContainsKey(selectedChara.Name))
                    CharactorData = charactors[selectedChara.Name];
                else
                {
                    await chara.Init(selectedChara, propertyImages);
                    CharactorData = chara;
                    lock (charactors)
                    {
                        if (!charactors.ContainsKey(chara.Name))
                            charactors.Add(chara.Name, chara);
                    }
                }
            }
            CharactorChanged?.Invoke();
        }

        // 更换区域
        private async void SelectCity(CityData cityData)
        {
            isLoading = true;

            List<Task<Action>> tasks = new List<Task<Action>>
            {
                LoadCityCharactor(cityData.CharactorData),
                LoadBackground1(cityData.BackgroundUrl1),
                LoadBackground2(cityData.BackgroundUrl2)
            };

            await Task.WhenAll(tasks);

            foreach (var task in tasks)
                task.Result?.Invoke();

            isLoading = false;
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
                    }
                }
            }
            else
                results = await loadDataService.LoadJsonBaseData<CharactorData>(ChanId);

            if(!settingData.LowMemoryMode)
            {
                foreach (var chara in results)
                {
                    if (charactors.ContainsKey(chara.Name))
                        continue;
                    Task t = Task.Run(async () =>
                    {
                        var fChara = new CharactorFullData();
                        await fChara.Init(chara, propertyImages);
                        lock (charactors)
                        {
                            if (!charactors.ContainsKey(fChara.Name))
                                charactors.Add(fChara.Name, fChara);
                        }
                    }
                    );
                }
            }

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
                    bitmapImage = await HttpHelper.GetImageAsync(url);
                    lock(backgroundImages){
                        if(!backgroundImages.ContainsKey(url))
                        backgroundImages.Add(url, bitmapImage);
                    }
                }
            }
            else
                bitmapImage = await HttpHelper.GetImageAsync(url);

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
                    bitmapImage = await HttpHelper.GetImageAsync(url);
                    lock (backgroundImages)
                    {
                        if (!backgroundImages.ContainsKey(url))
                            backgroundImages.Add(url, bitmapImage);
                    }
                }
            }
            else
                bitmapImage = await HttpHelper.GetImageAsync(url);

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


        #region INavigationAware
        public bool IsNavigationTarget(NavigationContext navigationContext) => true;
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            NavigationChanged?.Invoke();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
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
    }
}
