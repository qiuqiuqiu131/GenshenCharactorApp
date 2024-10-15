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
using System.Threading.Tasks;

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
        private async void LoadSettingData()
        {
            string data = File.ReadAllText("ProgramSetting.json");
            SettingData = JsonConvert.DeserializeObject<ProgramSettingData>(data);

            var tasks = new List<Task>{
                LoadCityData(),
                LoadWorldData()
            };

            // 加载结束启动程序
            await Task.WhenAll(tasks);
            regionManager.Regions["MainViewRegion"].RequestNavigate("HomeView");
        }

        // 加载区域角色数据
        private async Task LoadCityData()
        {
            var result = await loadDataService.LoadJsonBaseData<CityData>(SettingData.CityDataUrl);
            foreach (var cityData in result)
                cityData.CharactorData = SettingData.CityData[cityData.Name];
            ProgramData.CityData = result;
        }

        // 加载区域数据
        private async Task LoadWorldData()
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
