using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace GenshenApp.Common.JosnData
{
    public class CharactorData : JsonDataBase
    {
        public string Icon => GetExt(0);

        public string CharactorImage => GetExt(1);

        public string NameImage => GetExt(2);

        public string PropertyImage => GetExt(3);

        public string LinesImage => GetExt(8);

        public string Content => GetContent(7);

        public string CV_cn => GetExt(5);

        public string CV_jp => GetExt(6);

        private List<string> audios_cn = null;
        public List<string> Audios_cn
        {
            get
            {
                if (audios_cn == null)
                {
                    audios_cn =
                    [
                        GetExt(9),
                        GetExt(10),
                        GetExt(11)
                    ];
                }
                return audios_cn;  
            }
        }

        private List<string> audios_jp = null;
        public List<string> Audios_jp
        {
            get
            {
                if (audios_jp == null)
                {
                    audios_jp =
                    [
                        GetExt(12),
                        GetExt(13),
                        GetExt(14)
                    ];
                }
                return audios_jp;
            }
        }
    
        private string GetContent(int index)
        {
            string value = GetExt(index);
            string result = string.Empty;
            while(true)
            {
                int i = value.IndexOf("<p");
                if (i == -1)
                    break;
                if(result != string.Empty)
                    result += Environment.NewLine;
                value = value.Substring(i + 2);
                i = value.IndexOf(">");
                value = value.Substring(i + 1);
                i = value.IndexOf("</p>");
                result += value.Substring(0, i);
                value = value.Substring(i + 4);
            }
            return result
                .Replace("<br />","")
                .Replace("&mdash;", "—")
                .Replace("&hellip;","...");
        }
    }
}
