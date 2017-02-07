using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DoubanList
{
    class DoubanInfo
    {
        public rating rating { get; set; }

        public string reviews_count { get; set; }

        public string wish_count { get; set; }

        public string douban_site { get; set; }

        public string year { get; set; }

        public images images { get; set; }

        public string alt { get; set; }

        public string id { get; set; }

        public string mobile_url { get; set; }

        public string title { get; set; }

        public string do_count { get; set; }

        public string share_url { get; set; }

        public string seasons_count { get; set; }

        public string schedule_url { get; set; }

        public string episodes_count { get; set; }

        public string[] countries { get; set; }

        public string[] genres { get; set; }

        public string collect_count { get; set; }

        public List<people_class> casts { get; set; }

        public string current_season { get; set; }

        public string original_title { get; set; }

        public string summary { get; set; }

        public string subtype { get; set; }

        public List<people_class> directors { get; set; }

        public string comments_count { get; set; }

        public string ratings_count { get; set; }

        public string[] aka { get; set; }
    }

    class SearchResult
    {
        public string count { get; set; }

        public string start { get; set; }

        public string total { get; set; }

        public List<SimpleDoubanInfo> subjects { get; set; }

        public string titles { get; set; }
    }

    class SimpleDoubanInfo
    {
        public rating rating { get; set; }

        public string[] genres { get; set; }

        public string title { get; set; }

        public List<people_class> casts { get; set; }

        public string collect_count { get; set; }

        public string original_title { get; set; }

        public string subtype { get; set; }

        public List<people_class> directors { get; set; }

        public string year { get; set; }

        public images images { get; set; }

        public string alt { get; set; }

        public string id { get; set; }
    }

    public class rating
    {
        public string max { get; set; }
        public string average { get; set; }
        public string stars { get; set; }
        public string min { get; set; }
    }

    public class images
    {
        public string small { get; set; }
        public string large { get; set; }
        public string medium { get; set; }
    }

    public class people_class
    {
        public string alt { get; set; }
        public images avatars { get; set; }
        public string name { get; set; }
        public string id { get; set; }
    }
}
