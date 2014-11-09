using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Facebook;
using System.Net;

class ReadMovieDatabase
{
    public void readMovies()
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
};

class ReadFacebookData
{
    
   public void readFacebookData(){
       string AccessToken = "CAACEdEose0cBAETtn3rPTQBpBjdu18rZBIE9oYbLL6EH0DnxjyXT24AwVQTe2WKghSvlNS86XyVPxTFsGzZCLFKC88QER6O3bKllYO1pE50fnr6bydWKEercPscPnxiMZBM1WZBkZBcxI5GDgGAd9XdWAWuN9hPb4brW4cneyUawpgpSndssow74fhZCDzsHsq84Q15U5EZCSqWGQSJYK2ZBZCIlrEnAUFG8ZD";
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
};

namespace FacebookGetInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            ReadFacebookData dat1 = new ReadFacebookData();
            dat1.readFacebookData();
            ReadMovieDatabase dat2 = new ReadMovieDatabase();
            dat2.readMovies();
        }
    }
}
