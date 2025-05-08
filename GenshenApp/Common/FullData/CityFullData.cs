using System.Windows.Media.Imaging;
using Prism.Mvvm;

namespace GenshenApp.Common;

public class CityFullData:BindableBase
{
    public string Name { get; set; }
    
    private BitmapImage itemBackground;

    public BitmapImage ItemBackground
    {
        get => itemBackground;
        set => SetProperty(ref itemBackground, value);
    }

    private BitmapImage itemCharactor;

    public BitmapImage ItemCharactor
    {
        get => itemCharactor;
        set => SetProperty(ref itemCharactor, value);
    }
}