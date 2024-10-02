using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenshenCharactorApp.Common.JosnData
{
    public class HomeNewData:JsonDataBase
    {
        public string Url {  get; set; }
        public string Image => GetExt(0);

        protected override void ParseProcess(JToken token)
        {
            Url = token["sUrl"].ToString();
        }
    }
}
