using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;

namespace RS.Core
{
    public class moviesCore    
    {
        public class movieDB
        {
            public ObjectId _id { get; set; }
            public movieDBData data { get; set; }

            public movieDB()
            {
                data = new movieDBData();
            }
        }

        public class movieDBData
        {
            public string IMDBID { get; set; }
            public string OMDBID { get; set; }
            public string RTID { get; set; }
            public string title { get; set; }
            public int year { get; set; }
            public List<actor> cast { get; set; }
            public List<moviesGenreTMDB> genres { get; set; }
            public double imdbScore { get; set; }
            public string director { get; set; }

            public movieDBData()
            {
                cast = new List<actor>();
                genres = new List<moviesGenreTMDB>();
            }
        }

        public class movieDataTMDB
        {
            [JsonProperty("id")]
            public string id { get; set; }

            [JsonProperty("original_title")]
            public string originalTitle { get; set; }

            [JsonProperty("release_date")]
            public string date { get; set; }

            [JsonProperty("title")]
            public string title { get; set; }

            [JsonProperty("genres")]
            public List<moviesGenreTMDB> genres { get; set; }

            public movieDataTMDB()
            {
                genres = new List<moviesGenreTMDB>();
            }
        }

        public class moviesData
        {
            [JsonProperty("page")]
            public int Page { get; set; }

            [JsonProperty("results")]
            public List<movieDataTMDB> movies { get; set; }

            [JsonProperty("total_pages")]
            public int totalPages { get; set; }

            [JsonProperty("total_results")]
            public int totalResults { get; set; }
        }

        public class movieGenres
        {
            [JsonProperty("genres")]
            public List<moviesGenreTMDB> genreList { get; set; }

            public movieGenres()
            {
                genreList = new List<moviesGenreTMDB>();
            }
        }

        public class moviesGenreTMDB
        {
            [JsonProperty("id")]
            public int genreID { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }
        }

        public class movieDataOMDB
        {
            [JsonProperty("Title")]
            public string title { get; set; }

            [JsonProperty("Year")]
            public string year { get; set; }

            [JsonProperty("Rated")]
            public string rated { get; set; }

            [JsonProperty("Released")]
            public string released { get; set; }

            [JsonProperty("Runtime")]
            public string runtime { get; set; }

            [JsonProperty("Genre")]
            public string genre { get; set; }

            [JsonProperty("Director")]
            public string director { get; set; }

            [JsonProperty("Writer")]
            public string writer { get; set; }

            [JsonProperty("Actors")]
            public string actors { get; set; }

            [JsonProperty("Plot")]
            public string plot { get; set; }

            [JsonProperty("Language")]
            public string language { get; set; }

            [JsonProperty("Country")]
            public string country { get; set; }

            [JsonProperty("Awards")]
            public string awards { get; set; }

            [JsonProperty("Poster")]
            public string poster { get; set; }

            [JsonProperty("Metascore")]
            public string metascore { get; set; }

            [JsonProperty("imdbRating")]
            public string imdbRating { get; set; }

            [JsonProperty("imdbVotes")]
            public string imdbVotes { get; set; }

            [JsonProperty("imdbID")]
            public string imdbID { get; set; }

            [JsonProperty("Type")]
            public string type { get; set; }

            [JsonProperty("Response")]
            public string response { get; set; }
        }

        public class movieDataRT
        {
            [JsonProperty("id")]
            public int id { get; set; }
        }

        public class castRT
        {
            [JsonProperty("cast")]
            public List<actorRT> cast { get; set; }

            public castRT()
            {
                cast = new List<actorRT>();
            }
        }

        public class actorRT
        {
            [JsonProperty("id")]
            public string id { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }
        }

        public class actor
        {
            public int ID { get; set; }
            public int name { get; set; }
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
            public string IMDB_link { get; set; }
            public string IMDB_rating { get; set; }
            public List<actor> cast { get; set; }
        }
    }
}
