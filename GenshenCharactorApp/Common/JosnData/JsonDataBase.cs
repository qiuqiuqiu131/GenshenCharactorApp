using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenshenCharactorApp.Common.JosnData
{
    public class JsonDataBase
    {
        public string Name { get; set; }
        protected List<IndexElement> Exts { get; set; } = new();

        protected string GetExt(int index)
        {
            if (Exts == null || Exts.Count == 0)
                return string.Empty;

            var value = Exts.FirstOrDefault(d => d.Index.Equals(index)).Value;
            if (value is UrlData)
                return ((UrlData)value).Url;
            else
                return value == null ?string.Empty: value.ToString();
        }

        public static T ParseByJToken<T>(JToken token)
            where T : JsonDataBase,new()
        {
            T result = new T();

            // 角色名
            result.Name = token["sTitle"].ToString();
            result.ParseProcess(token);

            string exts = token["sExt"].ToString();

            // 角色拓展
            JObject ext = JObject.Parse(exts);
            foreach (var data in ext.Properties())
            {
                // index
                int index = int.Parse(data.Name.Split('_').Last());

                // value
                object value = null;
                if (data.Value.Type == JTokenType.String)
                    value = data.Value.ToString();
                else if(data.Value.Type == JTokenType.Array)
                {
                    if(data.Value.ToList().Count > 0)
                    {
                        var d = data.Value.ToList()[0];
                        value = new UrlData { Name = d["name"].ToString(), Url = d["url"].ToString() };
                    }
                }

                result.Exts.Add(new IndexElement() { Index = index, Value = value });
            }

            return result;
        }

        protected virtual void ParseProcess(JToken token) { } 
    }

    public class IndexElement
    {
        public int Index { get; set; }
        public object Value { get; set; }
    }

    public class UrlData
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
