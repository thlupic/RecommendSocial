using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RS.Core
{
    public class UserCore
    {
        public class userData
        {
            public ObjectId _id { get; set; }
            public int id { get; set; }
            public string username { get; set; }
            public string password { get; set; }

            public long twitterID { get; set; }
        }

        public class DBuserData
        {
            public int id { get; set; }
            public string username { get; set; }
            public string password { get; set; }

            public long twitterID { get; set; }
        }
    }
}
