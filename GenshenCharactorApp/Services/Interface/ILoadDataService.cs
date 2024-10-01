using System.Collections.Generic;
using System.Threading.Tasks;
using GenshenCharactorApp.Common.JosnData;

namespace GenshenCharactorApp.Services.Interface;

public interface ILoadDataService
{
    public Task<List<T>> LoadJsonBaseData<T>(string ChanId, int pageIndex = 1, int pageSize = 50, int iOrder = 6) where T : JsonDataBase, new();
}