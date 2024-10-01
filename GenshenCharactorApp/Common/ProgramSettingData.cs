using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenshenCharactorApp.Common
{
    public class ProgramSettingData
    {
        public bool LowMemoryMode {  get; set; }

        public string WorldDataUrl {  get; set; }
        public string CityDataUrl {  get; set; }
        
        public string NewDataUrl {  get; set; }
        public string NewsDataUrl { get; set; }
        public string NoticeDataUrl { get; set; }
        public string ActiveDataUrl { get; set; }
        public string NewTopDataUrl {  get; set; }

        public string HomeVideoUrl {  get; set; }

        public string Bg1Url {  get; set; }
        public string Bg2Url {  get; set; }
        public Dictionary<string,string> CityData { get; set; }
        public List<NavigateBar> NavigateBar {  get; set; }

        public static string GetUrl(string url,int pageIndex = 1,int pageSize = 10,int iOrder = -1)
        {
            string res = $"https://api-takumi-static.mihoyo.com/content_v2_user/app/16471662a82d418a/getContentList?iAppId=43&iChanId={url}&iPageSize={pageSize}&iPage={pageIndex}&sLangKey=zh-cn";
            if (iOrder != -1)
                res += $"&iOrder={iOrder}";
            return res;
        }
    }
}
