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

        public void Init(CharactorData charactorData)
        {
            charactor = charactorData;
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
