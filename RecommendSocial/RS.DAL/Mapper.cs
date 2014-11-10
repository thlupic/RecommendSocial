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

    }
}
