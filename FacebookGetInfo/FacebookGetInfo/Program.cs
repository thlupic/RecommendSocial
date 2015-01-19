﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Facebook;
using System.Net;


namespace FacebookGetInfo
{

    class Program
    {
        static void readMovies()
        {
            WebClient webClient = new WebClient();
            MongoServer mongoServer = MongoServer.Create();
            mongoServer.Connect();
            var db = mongoServer.GetDatabase("mydb");
            using (mongoServer.RequestStart(db))
            {
                string downloadString;
                var collection = db.GetCollection("MovieData");
                for (int i = 1; i <= 9; i++)
                {
                    downloadString = webClient.DownloadString("http://www.omdbapi.com/?i=tt000000" + i + "&plot=short&r=json");
                    BsonDocument doc = BsonDocument.Parse(downloadString);
                    collection.Insert(doc);
                    //Console.WriteLine(downloadString);
                }
            }
            mongoServer.Disconnect();
        }


        static void readFacebookData(string AccessToken)
        {
            var fb = new FacebookClient(AccessToken);
            dynamic me = fb.Get("me");
            string firstName = me.first_name;
            string lastName = me.last_name;
            MongoServer mongoServer = MongoServer.Create();
            mongoServer.Connect();
            var db = mongoServer.GetDatabase("mydb");


            using (mongoServer.RequestStart(db))
            {
                var collection = db.GetCollection<BsonDocument>("UserData");
                //collection.Insert(me);
                BsonDocument doc = new BsonDocument(me);
                collection.Insert(doc);

            }

            mongoServer.Disconnect();
        }

        static void Main(string[] args)
        {
            /*
            MongoServer mongoServer = MongoServer.Create();
            var db = mongoServer.GetDatabase("mydb");
            string AccessToken = "CAACEdEose0cBAMOfMkvWdmDYU7N4oDcZATEbqdRkFfQWYcMKEZCKOWVZBlc2foDbviybHgTX7cgAwjHVnHlX0uI5forZCe0sn5GY51z6QNMRkyRPAlXY7ZCUdythjNl7ufZBc9oCcVUtgxiDJ0B09q6ls8eHWO1OqxYrYW6GkRpvcfeFeNAs6is61mP8mJAutZBC9BlRBXMVIPpGK0I5fPcs71YLbzVew0ZD";
            readFacebookData(AccessToken);
            readMovies();
            using (mongoServer.RequestStart(db))
            {
                var userCollection = db.GetCollection<BsonDocument>("UserData");
                Console.WriteLine();
                foreach (BsonDocument doc in userCollection.FindAll())
                {
                    Console.WriteLine(doc.ToJson());
                }
                Console.WriteLine();
                var movieCollection = db.GetCollection("MovieData");
                foreach (BsonDocument doc in movieCollection.FindAll())
                {
                    Console.WriteLine(doc.ToJson());
                    Console.WriteLine();
                }
            }

            mongoServer.Disconnect();*/
            //RS.BLL.RecommendMovies.recommend(10);
        }
    }

}
