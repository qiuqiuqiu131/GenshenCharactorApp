using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using GenshenApp.Common.JosnData;
using GenshenApp.Helper;

namespace GenshenApp.Common
{
    public class CharactorFullData:IDisposable
    {
        private CharactorData charactor;

        public string Name => charactor.Name;
        public string CV_cn => charactor.CV_cn;
        public string CV_jp => charactor.CV_jp;
        public string Content => charactor.Content;
        public List<string> Audios_cn => charactor.Audios_cn;
        public List<string> Audios_jp => charactor.Audios_jp;

        public BitmapImage Icon { get; set; }
        public BitmapImage CharactorImage { get; set; }
        public BitmapImage NameImage { get; set; }
        public BitmapImage PropertyImage { get; set; }
        public BitmapImage LinesImage { get; set; }

        public async Task Init(CharactorData charactorData,Dictionary<string,BitmapImage> images)
        {
            charactor = charactorData;

            List<Task> tasks = new List<Task> {
                HttpHelper.GetImageAsync(charactorData.Icon)
                    .ContinueWith(e => Icon = e.Result),
                HttpHelper.GetImageAsync(charactorData.CharactorImage)
                    .ContinueWith(e => CharactorImage = e.Result),
                HttpHelper.GetImageAsync(charactorData.NameImage)
                    .ContinueWith(e => NameImage = e.Result),
                HttpHelper.GetImageAsync(charactorData.LinesImage)
                    .ContinueWith(e => LinesImage = e.Result),
            };
            if(images.ContainsKey(charactorData.PropertyImage))
                PropertyImage = images[charactorData.PropertyImage];
            else
            {
                tasks.Add(HttpHelper.GetImageAsync(charactorData.PropertyImage).ContinueWith(e =>
                    {
                        PropertyImage = e.Result;
                        lock(images)
                        {
                            if (!images.ContainsKey(charactorData.PropertyImage))
                                images.Add(charactorData.PropertyImage, PropertyImage);
                            else
                                PropertyImage = images[charactorData.PropertyImage];
                        }
                    }));
            }
            await Task.WhenAll(tasks);
            tasks.Clear();
        }

        public void Dispose()
        {
            charactor = null;
            Icon = null;
            CharactorImage = null;
            NameImage = null;
            PropertyImage = null;
            LinesImage = null;
        }
    }
}
