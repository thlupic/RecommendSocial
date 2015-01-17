using Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace RS.BLL
{
    public class Facebook
    {
        //private Uri RedirectUri
        //{
            //get
            //{
                //var uriBuilder = new UriBuilder(Request.Url);
                //uriBuilder.Query = null;
                //uriBuilder.Fragment = null;
                //uriBuilder.Path = Url.Action("FacebookCallback");
                //return uriBuilder.Uri;
            //}
        //}

        public void FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = "1538470139728022",
                client_secret = "958bde759598dbac3db3cf0eda526709",
                //redirect_uri = RedirectUri.AbsoluteUri,
                code = code,
                scope = "user_likes,email"
            });

            var accessToken = result.access_token;

            //Session["AccessToken"] = accessToken;
            fb.AccessToken = accessToken;

            // Get the user's information

            dynamic me = fb.Get("me?fields=first_name,last_name,id,likes,email");
            string firstname = me.first_name;
            string lastname = me.last_name;
            string email = me.email;
            var likes = me.likes;
            List<string> movies = new List<string>();
            foreach (var like in likes.data)
            {
                string id = like.id;
                string name = like.name;
            }

            // Set the auth cookie
            System.Web.Security.FormsAuthentication.SetAuthCookie(email, false);

            //get facebook movie data
            dynamic movie = fb.Get("https://graph.facebook.com/?ids=http://www.imdb.com/title/tt0816692/");
        }

        public void SignIn()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = "1538470139728022",
                client_secret = "958bde759598dbac3db3cf0eda526709",
                //redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email"
            });
        }

    }
}
