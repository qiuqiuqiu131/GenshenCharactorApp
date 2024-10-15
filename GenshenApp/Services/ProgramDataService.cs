using GenshenApp.Common.JosnData;
using GenshenApp.Common;
using GenshenApp.Services.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GenshenApp.Services
{
    public class ProgramDataService:IProgramDataService
    {
        private readonly ILoadDataService loadDataService;

        public ProgramSettingData SettingData { get; set; }
        public ProgramData ProgramData { get; set; }

        public ProgramDataService(ILoadDataService loadDataService)
        {
            this.loadDataService = loadDataService;
        }

        // 加载配置文件
        public void LoadSettingData()
        {
            string data = File.ReadAllText("ProgramSetting.json");
            SettingData = JsonConvert.DeserializeObject<ProgramSettingData>(data);
        }

        public async Task LoadProgramData()
        {
            ProgramData = new ProgramData();

            var tasks = new List<Task>{
                LoadCityData(),
                LoadWorldData()
            };

            // 加载结束启动程序
            await Task.WhenAll(tasks);
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
    }
}
