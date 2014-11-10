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
            var twitterAuth = Twitter.Authenticate1();

            Process.Start(twitterAuth.url);

            var credentials = TwitterCredentials.Credentials;
            Session["AuthKey"] = twitterAuth.AuthKey;
            Session["AuthSecret"] = twitterAuth.AuthSecret;

            return RedirectToAction("TwitterIndex");
        }

        public ActionResult TwitterIndex()
        {
            return View();
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
                var credentials = TwitterCredentials.CreateCredentials(token.AccessToken, token.AccessTokenSecret, twitterAuth.oAuthConsumerKey, twitterAuth.oAuthConsumerSecret);
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

        [HttpPost]
        public ActionResult Create(HomeInputModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = _models.Count==0?1:_models.Select(x => x.Id).Max() + 1;
                _models.Add(model);
                Success("Your information was saved!");
                return RedirectToAction("Index");
            }
            Error("there were some errors in your form.");
            return View(model);
        }

        public ActionResult Create()
        {
            return View(new HomeInputModel());
        }

        public ActionResult Delete(int id)
        {
            _models.Remove(_models.Get(id));
            Information("Your widget was deleted");
            if(_models.Count==0)
            {
                Attention("You have deleted all the models! Create a new one to continue the demo.");
            }
            return RedirectToAction("index");
        }
        public ActionResult Edit(int id)
        {
            var model = _models.Get(id);
            return View("Create", model);
        }
        [HttpPost]        
        public ActionResult Edit(HomeInputModel model,int id)
        {
            if(ModelState.IsValid)
            {
                _models.Remove(_models.Get(id));
                model.Id = id;
                _models.Add(model);
                Success("The model was updated!");
                return RedirectToAction("index");
            }
            return View("Create", model);
        }

		public ActionResult Details(int id)
        {
            var model = _models.Get(id);
            return View(model);
        }

    }
}
