using ImTools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenshenApp.Common.JosnData
{
    public class WorldData:JsonDataBase
    {
        public string Icon => GetExt(0);

        public string Background => GetExt(2);

        public string Content => GetContent(1);

        public List<WorldDetail> Detail => GetWorldDetail(4);

        private string GetContent(int index = 1)
        {
            string value = GetExt(index)
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("<br />", Environment.NewLine)
                .Replace("<br>", Environment.NewLine);
            int i = value.IndexOf('>');
            value = value.Substring(i + 1);
            i = value.IndexOf('<');
            return value.Substring(0, i);
        }

        private List<WorldDetail> GetWorldDetail(int index = 4)
        {
            string value = GetExt(index);
            List<WorldDetail> result = new List<WorldDetail>();

            while (true)
            {
                var data = new WorldDetail();

                int i = value.IndexOf("<h1>");
                if (i == -1)
                    break;
                value = value.Substring(i + 4);
                int j = value.IndexOf("</h1>");
                data.Title = value.Substring(0, j);
                value = value.Substring(j + 5);

                i = value.IndexOf("<p");
                value = value.Substring(i + 2);
                i = value.IndexOf(">");
                value = value.Substring(i + 1);
                j = value.IndexOf("</p>");
                data.Content = value.Substring(0, j)
                    .Replace("\n", "")
                    .Replace("<br />", Environment.NewLine);
                value = value.Substring(j + 4);

                i = value.IndexOf("src=");
                value = value.Substring(i + 5);
                j = value.IndexOf("\"");
                data.Image = value.Substring(0, j);

                result.Add(data);
            }

            return result;
        }
    }

    public class WorldDetail
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
    }

}
