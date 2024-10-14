using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using GenshenApp.Common;
using GenshenApp.Common.JosnData;
using GenshenApp.Events;
using GenshenApp.Helper;
using GenshenApp.Services.Interface;
using GenshenApp.Views;
using Newtonsoft.Json.Linq;
using Prism.Events;

namespace GenshenApp.Services;

public class LoadDataService :ILoadDataService
{
    private readonly IEventAggregator eventAggregator;

    public LoadDataService(IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
    }

    public async Task<List<T>> LoadJsonBaseData<T>(string ChanId, int pageIndex = 1, int pageSize = 50, int iOrder = 6) where T : JsonDataBase, new()
    {
        List<T> result = new List<T>();

        string url = ProgramSettingData.GetUrl(ChanId, pageIndex, pageSize, iOrder);

        try
        {
            var httpRes = await HttpHelper.GetJsonAsync(url);

            var cityDatas = JObject.Parse(httpRes);
            cityDatas["data"]["list"].ToList().ForEach(cityData =>
            {
                result.Add(JsonDataBase.ParseByJToken<T>(cityData));
            });
        }
        catch (HttpRequestException ex)
        {
            eventAggregator.GetEvent<HttpFailed>().Publish();
        }

        return result;
    }
}