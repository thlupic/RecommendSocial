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
using WebService;
using Facebook;

namespace BootstrapMvcSample.Controllers
{
    public class HomeController : BootstrapBaseController
    {
        private static List<HomeInputModel> _models = ModelIntializer.CreateHomeInputModels();

        public ActionResult Index()
        {
            //var homeInputModels = _models;
            //var modelMovies = new RecommendSocial.Models.MovieVm();
            //var genresList = MovieDB.getAllGenres();
            //var JsonResult = MovieDB.getSearch(model.searchName);
            //modelMovies.searchName = model.searchName;
            //modelMovies.movies = MovieDB.MappToCore(JsonResult);
           // modelMovies.numberOfMovies = 0;

            //MovieDB.storeMovies(movieList);
            //MovieDB.storeGenres(genresList);
            return RedirectToAction("Login");
        }

        //  pocetna funkcija koja logira usera pomocu facebook accounta
        [HttpGet]
        public ActionResult Login()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = "1538470139728022",
                client_secret = "958bde759598dbac3db3cf0eda526709",
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "user_likes,user_friends"
            });
            return Redirect(loginUrl.AbsoluteUri);
        }

        //pomocna funkcija koja se koristi kod logiranja s Facebook accountom
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = "1538470139728022",
                client_secret = "958bde759598dbac3db3cf0eda526709",
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code,
                scope = "user_likes,user_friends"
            });

            // kada si dobio access token dohvati informacije o korisniku 
            var accessToken = result.access_token;
            fb.AccessToken = accessToken;

            //dohvati list svih korisnika iz baze
            List<UserCore.userData> users = RS.BLL.Facebook.getUsers();

            dynamic me = fb.Get("me?fields=first_name,last_name,id,likes,friends");
            //pronadi korisnika u bazi ili ako ne postoji spremi novog 
            RS.BLL.Facebook.CreateUser(me, users);

            return RedirectToAction("GetMovies");
        }

        public ActionResult GetMovies()
        {
            //var movies = MovieDB.getAllMovies();
            var scrapper = new WebService.IMDBScrape();
            scrapper.getFacebookID("0816692");
            return View("LogedIn");
        }

       // [HttpPost]
       // public ActionResult Login(RecommendSocial.Models.UserVm model)
       // {
            //var movieList = MovieDB.getMoviesByGenres();
            //int userID = Twitter.getUserID(model.username, model.password);
            //Session["username"] = model.username;
            //Session["password"] = model.password;
            //if (userID != 0)
            //{
            //    Session["UserID"] = userID;
            //    long twitterID = Twitter.getTwitterID(userID);
            //    Session["TwitterID"] = twitterID;
            //}
            //else
            //{
            //    Session["username"] = model.username;
            //    Session["password"] = model.password;
            //}
         //   return RedirectToAction("IndexTwitter");
       // }

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
        //public ActionResult TwitterIndex(RecommendSocial.Models.TwitterVm model)
        //{
        //    if (!model.isAuth)
        //    {
        //        TwitterCore.TwitterAuth twitterAuth = new TwitterCore.TwitterAuth();
        //        TwitterCore.Tokens token = new TwitterCore.Tokens();
        //        twitterAuth.AuthKey = Session["AuthKey"].ToString();
        //        twitterAuth.AuthSecret = Session["AuthSecret"].ToString();
        //        twitterAuth.captcha = model.captcha;
        //        token = Twitter.Authenticate2(twitterAuth);
        //        Session["AccessToken"] = token.AccessToken;
        //        Session["AccessTokenSecret"] = token.AccessTokenSecret;
        //        TwitterCredentials.SetCredentials(token.AccessToken, token.AccessTokenSecret, twitterAuth.oAuthConsumerKey, twitterAuth.oAuthConsumerSecret);
        //        var user = Tweetinvi.User.GetLoggedUser();
        //        if (Session["TwitterID"].ToString() != "")
        //        {
        //            Twitter.saveUser(Session["username"].ToString(), Session["password"].ToString(), user.Id);
        //        }
        //        var credentials = TwitterCredentials.CreateCredentials(token.AccessToken, token.AccessTokenSecret, twitterAuth.oAuthConsumerKey, twitterAuth.oAuthConsumerSecret);
        //        Twitter.setTokens(user.Id, token);
        //        model.description = user.Description;
        //        model.name = user.Name;
        //        model.profileImageLink = user.ProfileImageUrl;
        //        Session["userName"] = model.name;
        //        Session["description"] = model.description;
        //        Session["profileImageLink"] = model.profileImageLink;
        //        return View(model);
        //    }
        //    else
        //    {
        //        var modelMovies = new RecommendSocial.Models.MovieVm();
        //        var movieList = MovieDB.getMoviesByGenres();
        //        var genresList = MovieDB.getAllGenres();
        //        //var JsonResult = MovieDB.getSearch(model.searchName);
        //        //modelMovies.searchName = model.searchName;
        //        //modelMovies.movies = MovieDB.MappToCore(JsonResult);
        //        //modelMovies.numberOfMovies = movieList.Count();
        //        modelMovies.numberOfMovies = 0;

        //        MovieDB.storeMovies(movieList);
        //        //MovieDB.storeGenres(genresList);
                
        //        return View("MoviesIndex", modelMovies);
        //    }
        //}

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
            //var JsonResult = MovieDB.getSearch(model.searchName);
            //model.searchName = model.searchName;
            //model.movies = MovieDB.MappToCore(JsonResult);
            return View("MoviesIndex", model);
        }
    }
}
