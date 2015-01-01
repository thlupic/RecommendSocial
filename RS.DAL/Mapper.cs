using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RS.Core;

namespace RS.DAL
{
    public static class Mapper 
    {
        public static void setTokens(long userID, TwitterCore.Tokens tokens)
        {
            TwitterCore.User userData = new TwitterCore.User();
            userData.ID = userID;
            userData.AccessToken = tokens.AccessToken;
            userData.AccessTokenSecret = tokens.AccessTokenSecret;
            DataStorage.setTokens(userData);
        }

        public static TwitterCore.Tokens getTokens(long userID)
        {
            TwitterCore.Tokens tokens = new TwitterCore.Tokens();
            TwitterCore.User user = DataStorage.getTokens(userID);

            tokens.AccessToken = user.AccessToken;
            tokens.AccessTokenSecret = user.AccessTokenSecret;

            return tokens;
        }

        public static bool checkUserValid(string username, string hashPassword, int userID)
        {
            UserCore.userData userData = getUserData(userID);
            if (userData.username != username || userData.password != hashPassword) return false;
            return true;
        }

        public static UserCore.userData getUserData(int userID)
        {
            UserCore.userData userData = new UserCore.userData();

            var data = DataStorage.getUserData(userID);

            userData.id = data.id;
            userData.username = data.username;
            userData.password = data.password;
            userData.twitterID = data.twitterID;

            return userData;
        }

        public static void storeUserData(string username, string password, long twitterID)
        {
            UserCore.DBuserData userData = new UserCore.DBuserData();
            userData.username = username;
            userData.password = password;
            userData.twitterID = twitterID;

            userData.id = DataStorage.getLastID();

            DataStorage.storeUserData(userData);
        }

        public static int getUserID(string username, string password)
        {
            return DataStorage.getUserID(username, password);
        }


        public static long getTwitterID(int userID)
        {
            return DataStorage.getTwitterID(userID);
        }
    }
}
