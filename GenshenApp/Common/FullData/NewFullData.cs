using System;
using System.Windows.Media.Imaging;
using GenshenApp.Common.JosnData;

namespace GenshenApp.Common;

public class NewFullData
{
    private NewData newData;

    public DateOnly StartTime => newData.StartTime;
    public DateOnly EndTime => newData.EndTime;
    public string IInfoId => newData.IInfoId;
    
    public BitmapImage Image { get; set; }

    public void Init(NewData newData)
    {
        this.newData = newData;
    }
}