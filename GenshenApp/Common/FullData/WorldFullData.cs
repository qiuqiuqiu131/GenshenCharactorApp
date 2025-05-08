using GenshenApp.Common.JosnData;
using GenshenApp.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Prism.Mvvm;

namespace GenshenApp.Common
{
    public class WorldFullData:BindableBase,IDisposable
    {
        private WorldData worldData;

        public string Content => worldData.Content;
        public string Name => worldData.Name;

        private BitmapImage icon;

        public BitmapImage Icon
        {
            get => icon;
            set => SetProperty(ref icon, value);
        }

        private BitmapImage background;
        public BitmapImage Background
        {
            get => background;
            set => SetProperty(ref background, value);
        }

        private List<WorldFullDetail> details;

        public List<WorldFullDetail> Details
        {
            get => details;
            set => SetProperty(ref details, value);
        }

        public void Init(WorldData worldData)
        {
            this.worldData = worldData;
        }

        public void Dispose()
        {
            worldData = null;
            Icon = null;
            Background = null;
            Details.Clear();
        }
    }

    public class WorldFullDetail
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public BitmapImage Image { get; set; }
    }
}
