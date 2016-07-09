namespace Nancy.Demo.MarkdownViewEngine
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using MarkdownSharp;

    [Serializable]
    public class BlogModel
    {
        private static readonly Regex SSVESubstitution = new Regex("^@[^$]*?$",
                                                                   RegexOptions.Compiled | RegexOptions.IgnoreCase |
                                                                   RegexOptions.Multiline);

        private readonly Markdown parser = new Markdown();

        private readonly Dictionary<string, string> metaData = new Dictionary<string, string>();

        public string Title { get; private set; }

        public string Abstract { get; private set; }

        public DateTime BlogDate { get; private set; }

        public string FriendlyDate
        {
            get { return BlogDate.ToString("dddd,MMMM dd, yyyy"); }
        }

        public IEnumerable<string> Tags { get; private set; }

        public string Slug { get; private set; }

        public BlogModel(string markdown)
        {
            string metadata = markdown.Contains("@Tags")
                           ? markdown.Substring(markdown.IndexOf("@Tags", StringComparison.Ordinal) + 5,
                                                markdown.IndexOf("@EndTags", StringComparison.Ordinal) - 7)
                           : string.Empty;

            var metadataSplit = metadata.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in metadataSplit)
            {
                var itemdata = item.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                if (itemdata.Length < 2)
                    continue;
                metaData.Add(itemdata[0].Trim(), itemdata[1].Trim());
            }

            BlogDate = GetBlogDate();
            Title = GetTitle();
            Slug = GenerateSlug();
            Abstract = GetAbstract(markdown);
            Tags = GetTags();
        }

        private IEnumerable<string> GetTags()
        {
            if (!metaData.Any(x => x.Key == "Tags"))
                return Enumerable.Empty<string>();

            var csv = metaData.FirstOrDefault(x => x.Key == "Tags").Value;

            return csv.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
        }

        private DateTime GetBlogDate()
        {
            if (!metaData.Any(x => x.Key == "Date"))
                return DateTime.MinValue;

            var kvp = metaData.FirstOrDefault(x => x.Key == "Date");

            DateTime blogDateTime = DateTime.ParseExact(kvp.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            return blogDateTime;
        }

        private string GetTitle()
        {
            if (!metaData.Any(x => x.Key == "Title"))
                return string.Empty;

            return metaData.FirstOrDefault(x => x.Key == "Title").Value;
        }

        private string GetAbstract(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            if (content.Contains("@Tags"))
            {
                content = content.Substring(content.IndexOf("@EndTags", StringComparison.Ordinal) + 8);
            }

            string ssveRemoved = SSVESubstitution.Replace(content, "").Trim();
            var abstractpost = ssveRemoved.Substring(0, 175).Trim();
            var html = parser.Transform(abstractpost);
            html = html + "<p><a href='" + this.Slug + "'>Read More...</a>";
            return html.Substring(html.IndexOf("<p>", StringComparison.Ordinal));
        }

        private string GenerateSlug(char spacer = '-', bool removeStopWords = false)
        {
            string str = RemoveAccent(Title).ToLower();
            str = str.Replace("-", " ");
            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space   
            str = str.Substring(0, str.Length <= 100 ? str.Length : 100).Trim(); // cut and trim it   
            str = Regex.Replace(str, @"\s", spacer.ToString(CultureInfo.InvariantCulture)); // hyphens   

            if (removeStopWords)
            {
                var stopWords = "a,all,also,am,an,and,as,at,be,but,by,can,could,did,do,does,for,from,get,got,had,has,have,he,how,i,if,in,into,is,it,its,me,my,no,nor,not,of,off,on,or,other,our,so,some,than,that,the,their,them,then,there,these,they,this,tis,to,too,us,was,we,were,yet,you,your".Split(',');
                str = string.Join(spacer.ToString(CultureInfo.InvariantCulture), str.Split(spacer).Where(o => !stopWords.Contains(o)));
            }

            return str;
        }

        public string RemoveAccent(string txt)
        {
            byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return Encoding.ASCII.GetString(bytes);
        }
    }
}