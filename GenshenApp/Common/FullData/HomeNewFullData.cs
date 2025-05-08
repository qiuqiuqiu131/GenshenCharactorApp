using System.Windows.Media.Imaging;
using GenshenApp.Common.JosnData;
using Prism.Mvvm;

namespace GenshenApp.Common;

public class HomeNewFullData:BindableBase
{
    private HomeNewData homeNew;

    public string Url => homeNew.Url;

    private BitmapImage image;

    public BitmapImage Image
    {
        get => image;
        set => SetProperty(ref image, value);
    }

    public void Init(HomeNewData homeNewData)
    {
        homeNew = homeNewData;
    }
}