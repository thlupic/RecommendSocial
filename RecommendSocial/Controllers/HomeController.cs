using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using RS.BLL;
using RS.Core;
using System.Diagnostics;
using Tweetinvi;

namespace BootstrapMvcSample.Controllers
{
    public class HomeController : BootstrapBaseController
    {
        private static List<HomeInputModel> _models = ModelIntializer.CreateHomeInputModels();
        public ActionResult Index()
        {
            var homeInputModels = _models;
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(RecommendSocial.Models.UserVm model)
        {
            int userID = Twitter.getUserID(model.username, model.password);
            Session["username"] = model.username;
            Session["password"] = model.password;
            if (userID != 0)
            {
                Session["UserID"] = userID;
                long twitterID = Twitter.getTwitterID(userID);
                Session["TwitterID"] = twitterID;
            }
            else
            {
                Session["username"] = model.username;
                Session["password"] = model.password;
            }
            return RedirectToAction("IndexTwitter");
        }

        public ActionResult IndexTwitter()
        {
            string twitterIDString = "";
            try
            {
                twitterIDString = Session["TwitterID"].ToString();
            }
            catch
            {
            }

            long twitterID = 0;
            if (twitterIDString!="") 
            {
                twitterID = Convert.ToInt64(twitterIDString);
                var tokens = Twitter.getTokens(twitterID);
                var authData = new TwitterCore.TwitterAuth();

                TwitterCredentials.SetCredentials(tokens.AccessToken, tokens.AccessTokenSecret, authData.oAuthConsumerKey, authData.oAuthConsumerSecret);
            }
            var user = Tweetinvi.User.GetLoggedUser();

            if (user == null)
            {
                var twitterAuth = Twitter.Authenticate1();

                Process.Start(twitterAuth.url);

                var credentials = TwitterCredentials.Credentials;
                Session["AuthKey"] = twitterAuth.AuthKey;
                Session["AuthSecret"] = twitterAuth.AuthSecret;

                return RedirectToAction("TwitterIndex");
            }
            else
            {
                RecommendSocial.Models.TwitterVm model = new RecommendSocial.Models.TwitterVm();
                model.description = user.Description;
                model.name = user.Name;
                model.profileImageLink = user.ProfileImageUrl;
                model.isAuth = true;
                Session["userName"] = model.name;
                Session["description"] = model.description;
                Session["profileImageLink"] = model.profileImageLink;
                return RedirectToAction("TwitterIndexLogged");
            }
        }

        public ActionResult TwitterIndex()
        {
            return View();
        }

        public ActionResult TwitterIndexLogged(RecommendSocial.Models.TwitterVm model)
        {
            return View("TwitterIndex",model);
        }

        [HttpPost]
        public ActionResult TwitterIndex(RecommendSocial.Models.TwitterVm model)
        {
            if (!model.isAuth)
            {
                TwitterCore.TwitterAuth twitterAuth = new TwitterCore.TwitterAuth();
                TwitterCore.Tokens token = new TwitterCore.Tokens();
                twitterAuth.AuthKey = Session["AuthKey"].ToString();
                twitterAuth.AuthSecret = Session["AuthSecret"].ToString();
                twitterAuth.captcha = model.captcha;
                token = Twitter.Authenticate2(twitterAuth);
                Session["AccessToken"] = token.AccessToken;
                Session["AccessTokenSecret"] = token.AccessTokenSecret;
                TwitterCredentials.SetCredentials(token.AccessToken, token.AccessTokenSecret, twitterAuth.oAuthConsumerKey, twitterAuth.oAuthConsumerSecret);
                var user = Tweetinvi.User.GetLoggedUser();
                if (Session["TwitterID"].ToString() != "")
                {
                    Twitter.saveUser(Session["username"].ToString(), Session["password"].ToString(), user.Id);
                }
                var credentials = TwitterCredentials.CreateCredentials(token.AccessToken, token.AccessTokenSecret, twitterAuth.oAuthConsumerKey, twitterAuth.oAuthConsumerSecret);
                Twitter.setTokens(user.Id, token);
                model.description = user.Description;
                model.name = user.Name;
                model.profileImageLink = user.ProfileImageUrl;
                Session["userName"] = model.name;
                Session["description"] = model.description;
                Session["profileImageLink"] = model.profileImageLink;
                return View(model);
            }
            else
            {
                var modelMovies = new RecommendSocial.Models.MovieVm();
                var JsonResult = MovieDB.getSearch(model.searchName);
                modelMovies.searchName = model.searchName;
                modelMovies.movies = MovieDB.MappToCore(JsonResult);
                return View("MoviesIndex", modelMovies);
            }
        }

        public ActionResult MovieSearch(RecommendSocial.Models.MovieVm model)
        {
            return RedirectToAction("MoviesIndex", model);
        }

        public ActionResult MoviesIndex()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MoviesIndex(RecommendSocial.Models.MovieVm model)
        {
            var JsonResult = MovieDB.getSearch(model.searchName);
            model.searchName = model.searchName;
            model.movies = MovieDB.MappToCore(JsonResult);
            return View("MoviesIndex", model);
        }
    }
}
