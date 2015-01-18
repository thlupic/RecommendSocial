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
        //private static string connectionString = "mongodb://localhost";
        private static string connectionString = "mongodb://rcsocial:RCsocial@ds031631.mongolab.com:31631/recommendsocial";
        private static MongoClient client = new MongoClient(connectionString);
        private static MongoServer server = client.GetServer();
        //private static MongoDatabase database = server.GetDatabase("local");
        // private static MongoDatabase database = server.GetDatabase("users");
        //spremanje na moju lokalnu bazu-Nikolina
        private static MongoDatabase database = server.GetDatabase("recommendsocial");

        /* Ove dvije linije iznad koje su zakomentirane su za lokalnu bazu
         * Dvije aktivne linije su za online bazu
         * Ako iz nekog razloga treba prebaciti na lokalnu, zakomentiraju se
         * sadašnje otkomentirane linije i otkomentiraju ove dvije za local */

        #region twitter
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








        #endregion

        #region movies
        public static void storeMovie(moviesCore.movieDBData movie)
        {
            MongoCollection<moviesCore.movieDBData> collection = database.GetCollection<moviesCore.movieDBData>("Movies");
            collection.Insert(movie);
        }

        public static void storeGenre(moviesCore.moviesGenreTMDB genre)
        {
            MongoCollection<moviesCore.moviesGenreTMDB> collection = database.GetCollection<moviesCore.moviesGenreTMDB>("Genres");
            collection.Insert(genre);
        }

        public static List<moviesCore.movieDB> getMovies()
        {
            MongoCollection<moviesCore.movieDB> collection = database.GetCollection<moviesCore.movieDB>("Movies");
            var movieData = collection.AsQueryable<moviesCore.movieDB>().ToList();
            return movieData;
        }

        #endregion


        #region facebook

        public static void storeUserData(UserCore.DBuserData userData)
        {
            MongoCollection<UserCore.DBuserData> collection = database.GetCollection<UserCore.DBuserData>("LoginUserData");
            collection.Insert(userData);
        }

        public static UserCore.DBuserData getUserData(int userID)
        {
            UserCore.DBuserData userData = new UserCore.DBuserData();
            MongoCollection<UserCore.DBuserData> collection = database.GetCollection<UserCore.DBuserData>("LoginUserData");
            userData = collection.AsQueryable<UserCore.DBuserData>().Where(e => e.id == userID).SingleOrDefault();

            return userData;
        }

        public static List<UserCore.DBuserData> getUsers()
        {
            List<UserCore.DBuserData> list = new List<UserCore.DBuserData>();
            MongoCollection<UserCore.DBuserData> collection = database.GetCollection<UserCore.DBuserData>("LoginUserData");
            list = collection.AsQueryable<UserCore.DBuserData>().ToList();
            return list;
        }

        public static int getLastID()
        {
            int lastID = 1;

            MongoCollection<UserCore.userData> collection = database.GetCollection<UserCore.userData>("LoginUserData");
            var userData = collection.AsQueryable<UserCore.DBuserData>().ToList();

            foreach (var item in userData)
            {
                if (item.id > lastID) lastID = item.id;
            }

            return lastID;
        }

        public static int getUserID(string facebookID)
        {
            MongoCollection<UserCore.DBuserData> collection = database.GetCollection<UserCore.DBuserData>("LoginUserData");
            var userData = collection.AsQueryable<UserCore.DBuserData>().Where(e => e.facebookID == facebookID).SingleOrDefault();
            if (userData != null) return userData.id;
            else return 0;
        }

        public static long getTwitterID(int userID)
        {
            MongoCollection<UserCore.DBuserData> collection = database.GetCollection<UserCore.DBuserData>("LoginUserData");
            var userData = collection.AsQueryable<UserCore.DBuserData>().Where(e => e.id == userID).SingleOrDefault();
            if (userData != null & userData.facebookID != "0") return 1;
            else return 0;
        }
        #endregion
    }
}
