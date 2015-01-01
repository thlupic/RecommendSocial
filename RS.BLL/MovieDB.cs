using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RS.Core;

namespace RS.BLL
{
    public class MovieDB
    {
        #region JSON
        public class movieJSON
        {
            [JsonProperty("page")]
            public int Page { get; set; }

            [JsonProperty("results")]
            public List<movie> movies { get; set; }

            [JsonProperty("total_pages")]
            public int totalPages { get; set; }

            [JsonProperty("total_results")]
            public int totalResults { get; set; }
        }

        public class movie
        {
            [JsonProperty("adult")]
            public string adult {get;set;}

            [JsonProperty("backdrop_path")]
            public string path {get;set;}

            [JsonProperty("id")]
            public string id {get;set;}

            [JsonProperty("original_title")]
            public string originalTitle {get;set;}

            [JsonProperty("release_date")]
            public string date {get;set;}

            [JsonProperty("poster_path")]
            public string posterPath {get;set;}

            [JsonProperty("popularity")]
            public string popularity {get;set;}

            [JsonProperty("title")]
            public string title {get;set;}

            [JsonProperty("vote_average")]
            public string  voteAverage {get;set;}

            [JsonProperty("vote_count")]
            public string voteCount { get; set; }
        }
        #endregion

        public static List<movie> getSearch(string name)
        {
            //Common testing requirement. If you are consuming an API in a sandbox/test region, uncomment this line of code ONLY for non production uses.
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            string apiKey = "14526d01d666a48a486e579c77ba4d74";

            string URI = String.Format("http://api.themoviedb.org/3/search/movie?api_key={0}&query={1}", apiKey, name);

            var request = System.Net.WebRequest.Create(URI) as System.Net.HttpWebRequest;
            request.KeepAlive = true;

            request.Method = "GET";

            request.Accept = "application/json";
            request.ContentLength = 0;
            var serializer = new JsonSerializer();
            using (var response = request.GetResponse() as System.Net.HttpWebResponse)
            {
                var jsonResponse = DeserializeFromStream(response.GetResponseStream());
                var results = JsonConvert.DeserializeObject<movieJSON>(jsonResponse.ToString());
                return results.movies;
            }
        }

        public static object DeserializeFromStream(Stream stream)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize(jsonTextReader);
            }
        }

        public static List<TMDBCore.movieData> MappToCore(List<movie> JSONMovies)
        {
            List<TMDBCore.movieData> movieData = new List<TMDBCore.movieData>();

            foreach (var item in JSONMovies)
            {
                var movie = new TMDBCore.movieData();
                movie.adult = item.adult;
                movie.date = item.date;
                movie.id = item.id;
                movie.originalTitle = item.originalTitle;
                movie.path = item.path;
                movie.popularity = item.popularity;
                movie.posterPath = item.posterPath;
                movie.title = item.title;
                movie.voteAverage = item.voteAverage;
                movie.voteCount = item.voteCount;
                movieData.Add(movie);
            }

            return movieData;
        }

    }
}