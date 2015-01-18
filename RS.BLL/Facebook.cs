using Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using RS.Core;
using RS.DAL;


namespace RS.BLL
{
    public class Facebook
    {

        public static void CreateUser(dynamic me, List<UserCore.userData> users)
        {
            UserCore.userData user = new UserCore.userData();
            user.firstName = me.first_name;
            user.lastName = me.last_name;
            user.facebookID = me.id;
            var likes = me.likes;
            var friends = me.friends;


            //  napravi listu korisnikovih lajkova
            try
            {
                foreach (var like in likes.data)
                {
                    if (like.category.Equals("Movie"))
                    {
                        UserCore.LikeData movieLike = new UserCore.LikeData();
                        movieLike.likeID = like.id;
                        movieLike.name = like.name;
                        user.likes.Add(movieLike);
                    }
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

            //napuni listu prijatelja od korisnika

            foreach (var friend in friends.data)
            {

                if (users != null)
                {
                    UserCore.userData userFriend = users.Where(n => n.facebookID == friend.id).FirstOrDefault();
                    if (userFriend != null)
                    {
                        user.friends.Add(userFriend);
                        userFriend.friends.Add(user);
                    }
                }


            }

            // Set the auth cookie
            System.Web.Security.FormsAuthentication.SetAuthCookie(user.facebookID, false);


            if (RS.BLL.Facebook.getUserID(user.facebookID) == 0)
            {
                RS.BLL.Facebook.saveUser(user.firstName, user.lastName, user.facebookID, user.likes);
            }
        }
        public static int getUserID(string facebookID)
        {
            return Mapper.getUserID(facebookID);
        }


        public static void saveUser(string firstName, string lastName, string facebookID, List<UserCore.LikeData> likes)
        {
            int id = DataStorage.getLastID();
            Mapper.storeUserData(id, firstName, lastName, facebookID, likes);
        }
        public static List<UserCore.userData> getUsers()
        {
            List<UserCore.DBuserData> List = DataStorage.getUsers();
            return mappFromCore(List);
        }

        public static List<UserCore.userData> mappFromCore(List<UserCore.DBuserData> listCore)
        {
            List<UserCore.userData> list = new List<UserCore.userData>();
            foreach (UserCore.DBuserData userCore in listCore)
            {
                UserCore.userData user = new UserCore.userData();
                user.firstName = userCore.firstName;
                user.lastName = userCore.lastName;
                user.facebookID = userCore.facebookID;
                user.likes = userCore.likes;
                user.friends = userCore.friends;
                list.Add(user);
            }
            return list;
        }



    }
}
