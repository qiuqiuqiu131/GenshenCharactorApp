using Prism.Mvvm;
using Newtonsoft.Json;
using Prism.Commands;
using System.IO;
using System.Collections.Generic;
using GenshenApp.Common;
using GenshenApp.Common.JosnData;
using GenshenApp.Helper;
using GenshenApp.Services.Interface;
using Prism.Regions;

namespace GenshenApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private readonly IRegionManager regionManager;
        private readonly ILoadDataService loadDataService;

        public ProgramSettingData SettingData { get; set; }
        public List<NavigateBar> NavigateBars => SettingData.NavigateBar;

        private ProgramData programData = new();
        public ProgramData ProgramData
        {
            get => programData;
            set => SetProperty(ref programData, value);
        }

        public DelegateCommand<NavigateBar> SelectionChangedCommand { get; private set; }
        public DelegateCommand<string> ClickCommand { get; private set; }

        private int selectIndex = 0;
        public int SelectIndex
        {
            get => selectIndex;
            set => SetProperty(ref selectIndex, value);
        }

        public MainWindowViewModel(IRegionManager regionManager,ILoadDataService loadDataService)
        {
            this.regionManager = regionManager;
            this.loadDataService = loadDataService;

            SelectionChangedCommand = new DelegateCommand<NavigateBar>(SelectionChanged);
            ClickCommand = new DelegateCommand<string>(Click);

            LoadSettingData();
        }

        // 加载配置文件
        private void LoadSettingData()
        {
            string data = File.ReadAllText("ProgramSetting.json");
            SettingData = JsonConvert.DeserializeObject<ProgramSettingData>(data);

            LoadCityData();
            LoadWorldData();
        }

        private async void LoadCityData()
        {
            var result = await loadDataService.LoadJsonBaseData<CityData>(SettingData.CityDataUrl);
            foreach (var cityData in result)
                cityData.CharactorData = SettingData.CityData[cityData.Name];
            ProgramData.CityData = result;
        }

        private async void LoadWorldData()
        {
            var result = await loadDataService.LoadJsonBaseData<WorldData>(SettingData.WorldDataUrl);
            ProgramData.WorldData = result;
        }

        private void SelectionChanged(NavigateBar navigateBar)
        {
            if (navigateBar.Target != null)
            {
                regionManager.Regions["MainViewRegion"].RequestNavigate(navigateBar.Target);
            }
            else if(navigateBar.Url != null) 
            {
                HttpHelper.OpenWebPage(navigateBar.Url);
                SelectIndex = 0;
            }
        }

        private void Click(string obj)
        {
            HttpHelper.OpenWebPage(obj);
        }
    }
}
