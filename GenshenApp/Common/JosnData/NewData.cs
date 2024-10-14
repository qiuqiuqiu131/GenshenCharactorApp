using ImTools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenshenApp.Common.JosnData
{
    public class NewData:JsonDataBase
    {
        public DateOnly StartTime { get; set; }
        public DateOnly EndTime { get; set; }
        public string IInfoId {  get; set; }
        public string Image => GetExt(1) == string.Empty ? "https://ys.mihoyo.com/main/_nuxt/img/holder.37207c1.jpg" : GetExt(1);

        protected override void ParseProcess(JToken token)
        {
            StartTime = DateOnly.FromDateTime(DateTime.Parse(token["dtStartTime"].ToString()));
            EndTime = DateOnly.FromDateTime(DateTime.Parse(token["dtEndTime"].ToString()));
            IInfoId = token["iInfoId"].Value<int>().ToString();
        }
    }
}
