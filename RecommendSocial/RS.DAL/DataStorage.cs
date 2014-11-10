﻿using System;
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

        public static int getUserID(string username, string password)
        {
            MongoCollection<UserCore.DBuserData> collection = database.GetCollection<UserCore.DBuserData>("LoginUserData");
            var userData = collection.AsQueryable<UserCore.DBuserData>().Where(e => e.username == username && e.password == password).SingleOrDefault();
            if (userData!=null) return userData.id;
            else return 0;
        }

        public static long getTwitterID(int userID)
        {
            MongoCollection<UserCore.DBuserData> collection = database.GetCollection<UserCore.DBuserData>("LoginUserData");
            var userData = collection.AsQueryable<UserCore.DBuserData>().Where(e => e.id == userID).SingleOrDefault();
            if (userData != null & userData.twitterID != 0) return userData.twitterID;
            else return 0;
        }
    }
}
