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
    public class CharactorFullData
    {
        public string Name { get; set; }
        public string CV_cn {  get; set; }
        public string CV_jp { get; set; }
        public string Content {  get; set; }
        public BitmapImage Icon { get; set; }
        public BitmapImage CharactorImage { get; set; }
        public BitmapImage NameImage { get; set; }
        public BitmapImage PropertyImage { get; set; }
        public BitmapImage LinesImage { get; set; }
        public List<string> Audios_cn { get; set; }
        public List<string> Audios_jp { get; set; }

        public async Task Init(CharactorData charactorData,Dictionary<string,BitmapImage> images)
        {
            Name = charactorData.Name;
            CV_cn = charactorData.CV_cn;
            CV_jp = charactorData.CV_jp;
            Content = charactorData.Content;
            Audios_cn = charactorData.Audios_cn;
            Audios_jp = charactorData.Audios_jp;

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
                        }
                    }));
            }
            await Task.WhenAll(tasks);
        }
    }
}
