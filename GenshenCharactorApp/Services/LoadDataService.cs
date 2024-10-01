using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenshenCharactorApp.Common;
using GenshenCharactorApp.Common.JosnData;
using GenshenCharactorApp.Helper;
using GenshenCharactorApp.Services.Interface;
using Newtonsoft.Json.Linq;

namespace GenshenCharactorApp.Services;

public class LoadDataService :ILoadDataService
{
    public async Task<List<T>> LoadJsonBaseData<T>(string ChanId, int pageIndex = 1, int pageSize = 50, int iOrder = 6) where T : JsonDataBase, new()
    {
        List<T> result = new List<T>();

        string url = ProgramSettingData.GetUrl(ChanId, pageIndex, pageSize, iOrder);
        var httpRes = await HttpHelper.GetJsonAsync(url);

        var cityDatas = JObject.Parse(httpRes);
        cityDatas["data"]["list"].ToList().ForEach(cityData =>
        {
            result.Add(JsonDataBase.ParseByJToken<T>(cityData));
        });

        return result;
    }
}