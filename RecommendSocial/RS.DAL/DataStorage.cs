using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using RS.Core;

namespace RS.DAL
{
    public class DataStorage
    {
        private static string connectionString = "mongodb://localhost";
        private static MongoClient client = new MongoClient(connectionString);
        private static MongoServer server = client.GetServer();
        private static MongoDatabase database = server.GetDatabase("local");

        public static TwitterCore.Tokens getTokens(string userName)
        {
            TwitterCore.Tokens userTokens = new TwitterCore.Tokens();

            return userTokens;
        }

        public static void setTokens(TwitterCore.User user)
        {
            MongoCollection<TwitterCore.User> collection = database.GetCollection<TwitterCore.User>("UserData");
            collection.Insert(user);
        }

        public static TwitterCore.User getTokens(long userID)
        {
            TwitterCore.User user = new TwitterCore.User();
            MongoCollection<TwitterCore.User> collection = database.GetCollection<TwitterCore.User>("UserData");
            user = collection.AsQueryable<TwitterCore.User>().Where(e => e.ID == userID).SingleOrDefault();

            return user;
        }
    }
}
