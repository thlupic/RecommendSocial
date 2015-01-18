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
        public static List<moviesCore.movieDBData> recommend(int numOfMovies)
        {
            BsonClassMap.RegisterClassMap<moviesCore.movieDB>();

            //*************** testni podaci *****************
            UserProfile profile = new UserProfile();
            profile.firstName = "Matija";
            profile.lastName = "Močilac";
            profile.likes = new List<UserLike>();
            profile.facebookID = "12345";

            UserLike like1 = new UserLike();
            like1.id = "1";
            like1.name = "Interstellar";
            profile.likes.Add(like1);

            UserLike like2 = new UserLike();
            like2.id = "2";
            like2.name = "Furious 7";
            profile.likes.Add(like2);
            //************************************************

            string connectionString = "mongodb://rcsocial2:RCsocial2@ds031631.mongolab.com:31631/recommendsocial";
            MongoClient client = new MongoClient(connectionString);
            MongoServer server = client.GetServer();
            server.Connect();
            //private static MongoDatabase database = server.GetDatabase("local");
            MongoDatabase database = server.GetDatabase("recommendsocial");

            
            MongoCollection<moviesCore.movieDB> moviesCollection = database.GetCollection<moviesCore.movieDB>("Movies");
            var qResult = (from m in moviesCollection.AsQueryable<moviesCore.movieDB>() select m).Count();
            Console.WriteLine("Movies count={0}", qResult);
            

            HashSet<moviesCore.actorRT> actors = new HashSet<moviesCore.actorRT>();   //skup glumaca koji se nalaze u svim filmovima koje je korisnik lajkao
            HashSet<moviesCore.moviesGenreTMDB> genres = new HashSet<moviesCore.moviesGenreTMDB>();   //skup id-jeva žanrova koje je korisnik lajkao
            HashSet<string> directors = new HashSet<string>();  //skup redatelja koji su režirali sve filmove koje je korisnik lajkao
            
            //******* testni ispis *********
            var queryResult2 = moviesCollection.FindAll();

            foreach (var v in queryResult2)
                Console.WriteLine(v.data.IMDBID);
            //******************************

            //za svaki lajk izdvoji glumce, žanrove i redatelje
            foreach (UserLike like in profile.likes)
            {
                //nađi film u bazi
                var queryResult = (from m in moviesCollection.AsQueryable<moviesCore.movieDB>() where m.data.title.Equals(like.name) select m);
                if (queryResult.Count() == 0)   //upit nije pronašao niti jedan film 
                    continue;

                //Console.WriteLine("Pronađen rezultat");
                var singleQuery = queryResult.First();  //u slučaju da je za neki film pronađeno više rezultata, vrati prvi

                //ako redatelj ne postoji u skupu, dodaj ga
                if (!directors.Contains(singleQuery.data.director))
                    directors.Add(singleQuery.data.director);

                foreach (var actor in singleQuery.data.cast)
                {
                    //ako se glumac ne nalazi u skupu, dodaj ga
                    if (!actors.Contains(actor))
                        actors.Add(actor);
                }

                foreach (var g in singleQuery.data.genres.genreList)
                {
                    //ako se žanr ne nalazi u skupu, dodaj ga
                    if (!genres.Contains(g))
                        genres.Add(g);
                }

            }

            Console.WriteLine("Genres: {0}, Directors: {1}, Actors: {2}", genres.Count, directors.Count, actors.Count);
            Console.ReadLine();

            //dohvati sve filmove koji imaju zajednička dva od tri parametra koja korisnik preferira: žanr i neki od glumaca,
            //žanr i redatelj ili redatelj i neki od glumaca
            var queryResult1=(from m in moviesCollection.AsQueryable<moviesCore.movieDB>()
                             where (genres.ContainsAny(m.data.genres.genreList) && directors.Contains(m.data.director)
                                    || genres.ContainsAny(m.data.genres.genreList) && actors.ContainsAny(m.data.cast)
                                    || directors.Contains(m.data.director) && actors.ContainsAny(m.data.cast))
                             select m);

            //sadrži filmove sortirane prema imdb ocjeni
            List<moviesCore.movieDBData> sortedMovies = new List<moviesCore.movieDBData>();

            foreach (var v in queryResult1)
                sortedMovies.Add(v.data);

            sortedMovies = sortedMovies.OrderByDescending(o => o.imdbScore).ToList();

            //lista koja sadrži filmove koji će biti prikazani korisniku
            List<moviesCore.movieDBData> returnMovies = new List<moviesCore.movieDBData>();

            //ako je rezultat vratio manje filmova od zadanog
            if (sortedMovies.Count < numOfMovies)
                numOfMovies = sortedMovies.Count;

            for (int i = 0; i < numOfMovies; i++)
            {
                returnMovies.Add(sortedMovies.ElementAt(i));
            }

            server.Disconnect();
            return returnMovies;
        }
    }
}
