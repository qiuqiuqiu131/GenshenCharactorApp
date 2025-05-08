using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using GenshenApp.Helper;
using GenshenApp.Services.Interface;

namespace GenshenApp.Services;

public class BitmapImageManager : IBitmapImageManager
{
    private ConcurrentDictionary<string, BitmapImage> bitmapImageCache = new();

    public async Task<BitmapImage> GetBitmapImage(string url)
    {
        bitmapImageCache.TryGetValue(url, out var bitmap);

        if(bitmap != null)
            return bitmap;

        bitmap = await HttpHelper.GetImageAsync(url);

        bitmapImageCache.TryAdd(url, bitmap);
        return bitmap;
    }
}

