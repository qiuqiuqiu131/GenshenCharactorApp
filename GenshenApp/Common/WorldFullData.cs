using GenshenApp.Common.JosnData;
using GenshenApp.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GenshenApp.Common
{
    public class WorldFullData:IDisposable
    {
        private WorldData worldData;

        public string Content => worldData.Content;
        public string Name => worldData.Name;

        public BitmapImage Icon { get; set; }
        public BitmapImage Background { get; set; }
        private List<WorldFullDetail> details;
        public List<WorldFullDetail> Details => details; 


        public async Task Init(WorldData worldData)
        {
            this.worldData = worldData;

            var tasks = worldData.Detail.AsParallel().AsOrdered().Select(async x =>
            {
                BitmapImage image = await HttpHelper.GetImageAsync(x.Image);
                return new WorldFullDetail
                {
                    Title = x.Title,
                    Content = x.Content,
                    Image = image
                };
            }).ToList();
            List<Task> tasks2 = new List<Task>
            {
                HttpHelper.GetImageAsync(worldData.Icon).ContinueWith(e => Icon = e.Result),
                HttpHelper.GetImageAsync(worldData.Background).ContinueWith(e => Background = e.Result),
                Task.WhenAll(tasks).ContinueWith(x =>
                {
                    details = tasks.Select(x => x.Result).ToList();
                })
            };

            await Task.WhenAll(tasks2);
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
