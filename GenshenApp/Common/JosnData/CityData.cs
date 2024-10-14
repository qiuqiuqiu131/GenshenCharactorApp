using ImTools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace GenshenApp.Common.JosnData
{
    public class CityData:JsonDataBase
    {
        public string CharactorData { get; set; }

        public string ItemBackground => GetExt(1);

        public string ItemCharactor => GetExt(2);

        public string BackgroundUrl1 => GetExt(4);

        public string BackgroundUrl2 => GetExt(5);
    }
}
