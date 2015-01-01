using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RS.Core
{
    public class TMDBCore    
    {
        public class movieDataJson
        {
            [JsonProperty("adult")]
            public string adult { get; set; }

            [JsonProperty("backdrop_path")]
            public string path { get; set; }

            [JsonProperty("id")]
            public string id { get; set; }

            [JsonProperty("original_title")]
            public string originalTitle { get; set; }

            [JsonProperty("release_date")]
            public string date { get; set; }

            [JsonProperty("poster_path")]
            public string posterPath { get; set; }

            [JsonProperty("popularity")]
            public string popularity { get; set; }

            [JsonProperty("title")]
            public string title { get; set; }

            [JsonProperty("vote_average")]
            public string voteAverage { get; set; }

            [JsonProperty("vote_count")]
            public string voteCount { get; set; }
        }

        public class movieData
        {
            public string adult { get; set; }
            public string path { get; set; }
            public string id { get; set; }
            public string originalTitle { get; set; }
            public string date { get; set; }
            public string posterPath { get; set; }
            public string popularity { get; set; }
            public string title { get; set; }
            public string voteAverage { get; set; }
            public string voteCount { get; set; }
        }

    }
}
