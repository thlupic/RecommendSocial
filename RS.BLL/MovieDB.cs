using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RS.Core;
using WebService;
using MongoDB.Bson;

namespace RS.BLL
{
    public class MovieDB
    {
        public static string apiKey = "14526d01d666a48a486e579c77ba4d74";
        public static string RTapiKey = "sn6wtjvtm67kh73j72vvhk57";
        private static Methods webMethods = new Methods();
        private static Serialization serializer = new Serialization();

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

        public static void getGenres()
        {
            HashSet<moviesCore.movieDataTMDB> allMovies = new HashSet<moviesCore.movieDataTMDB>();
            HashSet<moviesCore.movieDataOMDB> OMDBMovies = new HashSet<moviesCore.movieDataOMDB>();
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            string URI = String.Format("http://api.themoviedb.org/3/genre/movie/list?api_key={0}", apiKey);

            var genres = JsonConvert.DeserializeObject<moviesCore.movieGenres>(webMethods.GET(URI));

            int totalResults = 0;

            foreach (var item in genres.genreList)
            {
                URI = String.Format("http://api.themoviedb.org/3/genre/{0}/movies?api_key={1}", item.genreID, apiKey);

                var movies = JsonConvert.DeserializeObject<moviesCore.moviesData>(webMethods.GET(URI));

                for (var i = 1; i <= movies.totalPages; i++)
                {
                    URI = String.Format("http://api.themoviedb.org/3/genre/{0}/movies?api_key={1}&page={2}", item.genreID, apiKey,i);

                    var movies1 = JsonConvert.DeserializeObject<moviesCore.moviesData>(webMethods.GET(URI));

                    foreach (var movie in movies1.movies)
                    {
                        var name = movie.originalTitle;
                        var nameParts = name.Split(' ');
                        string nameSearch = "";
                        foreach (var namepart in nameParts)
                        {
                            nameSearch += String.Format("+{0}", namepart);
                        }
                        nameSearch = nameSearch.Substring(1, nameSearch.Length - 1);
                        var releaseDate = movie.date;
                        string year = "";
                        if (releaseDate != "") { year = Convert.ToDateTime(releaseDate).Year.ToString(); }
                        URI = String.Format("http://www.omdbapi.com/?t={0}&y={1}&plot=short&r=json", nameSearch.ToLower(),year);                        
                        allMovies.Add(movie);

                        var OMDBmovie =JsonConvert.DeserializeObject<moviesCore.movieDataOMDB>(webMethods.GET(URI));
                        var imdbID = OMDBmovie.imdbID;
                        if (imdbID != null)
                        {
                            imdbID = imdbID.Substring(2, imdbID.Length - 2);
                        }

                        URI = String.Format("http://api.rottentomatoes.com/api/public/v1.0/movie_alias.json?apikey={0}&type=imdb&id={1}", RTapiKey, imdbID);
                        var RTdata = webMethods.GET(URI);
                        var RTcore = JsonConvert.DeserializeObject<moviesCore.movieDataRT>(RTdata);
                    }
                }
            }

            totalResults += 0;
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

        public static List<moviesCore.movieData> MappToCore(List<movie> JSONMovies)
        {
            List<moviesCore.movieData> moviesData = new List<moviesCore.movieData>();

            foreach (var item in JSONMovies)
            {
                var movie = new moviesCore.movieData();
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
                moviesData.Add(movie);
            }

            return moviesData;
        }

    }
}