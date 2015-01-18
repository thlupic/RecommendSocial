using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RS.BLL
{
    public class UserProfile
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string facebookID { get; set; }
        public List<UserLike> likes { get; set; }

       
    }
}