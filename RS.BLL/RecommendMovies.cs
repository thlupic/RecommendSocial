using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using RS.Core;
using RS.DAL;
using MongoDB.Bson.Serialization;

namespace RS.BLL
{
    public class RecommendMovies
    {
        //numOfMovies označava koliko filmova funkcija vraća
        public static List<moviesCore.movieDBData> recommend(UserCore.userData user)
        {
            BsonClassMap.RegisterClassMap<moviesCore.movieDB>();

            //*************** testni podaci *****************
            /*UserCore.userData profile = new UserCore.userData();
            profile.firstName = "Matija";
            profile.lastName = "Močilac";
            profile.likes = new List<UserCore.LikeData>();
            profile.facebookID = "12345";

            UserCore.LikeData like1 = new UserCore.LikeData();
            like1.likeID = "1";
            like1.name = "Big Hero 6";
            profile.likes.Add(like1);

            UserCore.LikeData like2 = new UserCore.LikeData();
            like2.likeID = "2";
            like2.name = "Furious 7";
            profile.likes.Add(like2);*/
            //************************************************

            string connectionString = "mongodb://rcsocial2:RCsocial2@ds031631.mongolab.com:31631/recommendsocial";
            MongoClient client = new MongoClient(connectionString);
            MongoServer server = client.GetServer();
            server.Connect();
            //private static MongoDatabase database = server.GetDatabase("local");
            MongoDatabase database = server.GetDatabase("recommendsocial");

            
            MongoCollection<moviesCore.movieDBDB> moviesCollection = database.GetCollection<moviesCore.movieDBDB>("Movies");
            //UserCore.DBuserData profile = DataStorage.getUserData(1);  //dohvati korisnika s id-jem 1
            
            //Console.WriteLine("Name: {0}", profile.firstName);
            //Console.WriteLine("Number of likes: {0}", profile.likes.Count);

            var qResult = (from m in moviesCollection.AsQueryable<moviesCore.movieDBDB>() select m).Count();
            Console.WriteLine("Movies count={0}", qResult);
            
      
            List<moviesCore.actorRT> actors = new List<moviesCore.actorRT>();   //skup glumaca koji se nalaze u svim filmovima koje je korisnik lajkao
            List<moviesCore.moviesGenreTMDB> genres = new List<moviesCore.moviesGenreTMDB>();   //skup id-jeva žanrova koje je korisnik lajkao
            List<string> directors = new List<string>();  //skup redatelja koji su režirali sve filmove koje je korisnik lajkao
            List<int> actorsMentioning = new List<int>();   //broj pojavljivanja pojedinog glumca u lajkovima
            List<int> genresMentioning = new List<int>();   //broj pojavljivanja pojedinog žanra u lajkovima
            List<int> directorsMentioning = new List<int>();    //broj pojavljivanja pojedinog redatelja u lajkovima


            //******* testni ispis *********
            /*var queryResult2 = moviesCollection.FindAll();

            foreach (var v in queryResult2)
                Console.WriteLine(v.IMDBID);*/
            //******************************


            //za svaki lajk izdvoji glumce, žanrove i redatelje
            foreach (UserCore.LikeData like in user.likes)
            {
                //nađi film u bazi
                var queryResult = (from m in moviesCollection.AsQueryable<moviesCore.movieDBDB>() where m.title.Equals(like.name) select m);
                if (queryResult.Count() == 0)   //upit nije pronašao niti jedan film 
                    continue;

                var singleQuery = queryResult.First();  //u slučaju da je za neki film pronađeno više rezultata, vrati prvi

                //ako redatelj ne postoji u skupu, dodaj ga
                if (!directors.Contains(singleQuery.director))
                {
                    directors.Add(singleQuery.director);
                    directorsMentioning.Add(1);
                }
                else    //ako redatelj već postoji u skupu, povećaj broj pojavljivanja za 1
                {
                    int index = directors.FindIndex(a=>a.Equals(singleQuery.director));
                    directorsMentioning[index] += 1;
                }

                foreach (var actor in singleQuery.cast)
                {
                    //ako se glumac ne nalazi u skupu, dodaj ga
                    if (!actors.Contains(actor))
                    {
                        actors.Add(actor);
                        actorsMentioning.Add(1);
                    }
                    else    //ako se glumac nalazi u skupu, povećaj broj pojavljivanja za 1
                    {
                        int index = actors.FindIndex(a => a.name.Equals(actor.name));
                        actorsMentioning[index] += 1;
                    }
                }

                foreach (var g in singleQuery.genres.genreList)
                {
                    //ako se žanr ne nalazi u skupu, dodaj ga
                    if (!genres.Contains(g))
                    {
                        genres.Add(g);
                        genresMentioning.Add(1);
                    }
                    else    //ako se žanr nalazi u skupu, povećaj broj pojavljivanja za 1
                    {
                        int index = genres.FindIndex(a => a.genreID == g.genreID);
                        genresMentioning[index] += 1;
                    }
                }

            }


            //*************************** TESTNI ISPIS **************************************
            Console.WriteLine("Genres: {0}, Directors: {1}, Actors: {2}", genres.Count, directors.Count, actors.Count);
            Console.Write("Genres mentioning: ");
            foreach (var v in genresMentioning)
                Console.Write("{0} ",v);
            Console.WriteLine();
            Console.Write("Directors mentioning: ");
            foreach (int i in directorsMentioning)
                Console.Write("{0} ", i);
            Console.WriteLine();
            Console.Write("Actors mentioning: ");
            foreach (int i in actorsMentioning)
                Console.Write("{0} ", i);
            Console.WriteLine();
            Console.WriteLine("Press any key to continue.");
            Console.ReadLine();
            //*****************************************************************************************


            //dohvati sve filmove koji imaju zajednička dva od tri parametra koja korisnik preferira: žanr i neki od glumaca,
            //žanr i redatelj ili redatelj i neki od glumaca
            var queryResult1 = (from m in moviesCollection.AsQueryable<moviesCore.movieDBDB>()
                                where (m.director.In(directors) && m.genres.genreList.ContainsAny(genres)
                                || m.genres.genreList.ContainsAny(genres) && m.cast.ContainsAny(actors)
                                || m.director.In(directors) && m.cast.ContainsAny(actors))
                                select m);

            Console.WriteLine("Broj pronađenih filmova: {0}", queryResult1.Count());

            //sumiraj ukupni broj spominjanja za glumce, redatelje i žanrove
            int totalDirectorMentionings = 0;
            foreach (int i in directorsMentioning)
                totalDirectorMentionings += i;

            int totalGenreMentionings = 0;
            foreach (int i in genresMentioning)
                totalGenreMentionings += i;

            int totalActorMentionings = 0;
            foreach (int i in actorsMentioning)
                totalActorMentionings+=i;

            //iduća polja označavaju koliko je pojedini glumac, redatelj i žanr značajan za korisnika, tj.
            //koliko često se pojavljuje u korisnikovim lajkovima
            double[] directorSignificance = new double[directorsMentioning.Count];
            double[] actorSignificance = new double[actorsMentioning.Count];
            double[] genreSignificance = new double[genresMentioning.Count];

            for (int i = 0; i < directorSignificance.Count(); i++)
                directorSignificance[i] =(double) directorsMentioning[i] / totalDirectorMentionings;

            for (int i = 0; i < actorSignificance.Count(); i++)
                actorSignificance[i] = (double)actorsMentioning[i] / totalActorMentionings;

            for (int i = 0; i < genreSignificance.Count(); i++)
                genreSignificance[i] = (double)genresMentioning[i] / totalGenreMentionings;

            
            //sadrži listu filmova koji će se rangirati po kriteriju važnosti s obzirom na glumce, redatelje i žanrove
            List<MovieRank> rankedMovies = new List<MovieRank>();
                
            //za svaki pronađeni film
            foreach (var v in queryResult1)
            {
                MovieRank movieRank = new MovieRank(v);
                int index;
                foreach (var g in v.genres.genreList)
                {
                    if ((index = genres.FindIndex(a => a.genreID == g.genreID))!=-1)
                    {
                        movieRank.rank += genreSignificance[index]*queryResult1.Count();
                    }
                }

                foreach (var a in v.cast)
                {
                    if ((index = actors.FindIndex(o => o.name.Equals(a.name))) != -1)
                    {
                        movieRank.rank += actorSignificance[index] * queryResult1.Count();
                    }
                }

                if ((index = directors.FindIndex(d=>d.Equals(v.director))) != -1)
                {
                    movieRank.rank += directorSignificance[index] * queryResult1.Count();                      
                }

                rankedMovies.Add(movieRank);      
            }

            //poredaj filmove po rangu
            rankedMovies = rankedMovies.OrderByDescending(o => o.rank).ToList();
            //foreach (var v in rankedMovies)
            //    Console.WriteLine("{0}\t{1}", v.movie.title, v.rank);

            //lista koja sadrži filmove koji će biti prikazani korisniku
            List<moviesCore.movieDBData> returnMovies = new List<moviesCore.movieDBData>();

            //ako je rezultat vratio manje filmova od zadanog
            //if (rankedMovies.Count < numOfMovies)
            //    numOfMovies = rankedMovies.Count;
 

            for (int i = 0; i < rankedMovies.Count(); i++)
            {
                returnMovies.Add(rankedMovies.ElementAt(i).movie);
                Console.WriteLine("{0}\t{1}", rankedMovies.ElementAt(i).movie.title, rankedMovies.ElementAt(i).rank);
            }

            Console.WriteLine("Press any key to continue.");
            Console.ReadLine();
            server.Disconnect();
            return returnMovies;
        }
    }

    //pomoćna klasa za rangiranje filmova
    public class MovieRank
    {
        public moviesCore.movieDBDB movie;
        public double rank;

        public MovieRank(moviesCore.movieDBDB m)
        {
            movie = m;
            rank = m.imdbScore;
        }

    }
}
