using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GenshenApp.Services.Interface;

public interface IBitmapImageManager
{
    Task<BitmapImage> GetBitmapImage(string url);
}

