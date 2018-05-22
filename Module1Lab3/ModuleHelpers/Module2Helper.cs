using ConsoleTables;
using MovieApp.Entities;
using MovieApp.Extensions;
using MovieApp.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace MovieApp
{
    public static class Module2Helper
    {
        public static void Sort()
        {
            // Sorted by Lastname Actors
            var actors = MoviesContext.Instance.Actors
                .OrderBy(a => a.LastName)
                .Select(a => a.Copy<Actor, ActorModel>());
            ConsoleTable.From(actors).Write();
            // Sorted by Rating Movies
            var films = MoviesContext.Instance.Films
                .OrderBy(f => f.Rating)
                .ThenBy(f => f.ReleaseYear)
                .ThenBy(f => f.Title)
                .Select(f => f.Copy<Film, FilmModel>());
            ConsoleTable.From(films).Write();
        }
        public static void SortDescending()
        {
            // Actors sorted in descending order by the Firstname
            var actors = MoviesContext.Instance.Actors
                .OrderByDescending(a => a.FirstName)
                .Select(a => a.Copy<Actor, ActorModel>());
            ConsoleTable.From(actors).Write();
            // Films sorted in descending order by ReleaseYear and Title
            var films = MoviesContext.Instance.Films
                .OrderByDescending(f => f.ReleaseYear)
                .ThenByDescending(f => f.Title)
                .Select(f => f.Copy<Film, FilmModel>());
            ConsoleTable.From(films).Write();
        }
        public static void Skip()
        {
            // Films ordered by Title without the first five
            var films = MoviesContext.Instance.Films
                .OrderBy(f => f.Title)
                .Skip(5)
                .Select(f => f.Copy<Film, FilmModel>());
            ConsoleTable.From(films).Write();
        }
        public static void Take()
        {
            // The first five ordered by Title films
            var films = MoviesContext.Instance.Films
                .OrderBy(f => f.Title)
                .Take(5)
                .Select(f => f.Copy<Film, FilmModel>());
            ConsoleTable.From(films).Write();
        }
        public static void Paging()
        {
            Console.WriteLine("Enter a page size:");
            var pageSize = Math.Max(1, Console.ReadLine().ToInt());

            Console.WriteLine("Enter a page number:");
            var pageNumber = Math.Max(1, Console.ReadLine().ToInt());

            Console.WriteLine("Enter a sort column:");
            Console.WriteLine("\ti - Film ID");
            Console.WriteLine("\tt - Title");
            Console.WriteLine("\ty - Year");
            Console.WriteLine("\tr - Rating");
            var key = Console.ReadKey();

            Console.WriteLine();

            var films = MoviesContext.Instance.Films
                        .OrderBy(GetSort(key))
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .Select(f => f.Copy<Film, FilmModel>());
            ConsoleTable.From(films).Write();
        }

        private static Expression<Func<Film, object>> GetSort(ConsoleKeyInfo info)
        {
            switch (info.Key)
            {
                case ConsoleKey.I:
                    return f => f.FilmId;
                case ConsoleKey.Y:
                    return f => f.ReleaseYear;
                case ConsoleKey.R:
                    return f => f.Rating;
                default:
                    return f => f.Title;
            }
        }

        public static void LinqBasics()
        {
            var search = "ar";
            var actors = (from a in MoviesContext.Instance.Actors
                          where a.FirstName.Contains(search)
                          orderby a.FirstName descending
                          select a.Copy<Actor, ActorModel>());
            ConsoleTable.From(actors).Write();
        }

        public static void LambdaBasics()
        {
            var search = "g";
            var films = MoviesContext.Instance.Films
                        .Where(f => f.Title.Contains(search))
                        .OrderByDescending(f => f.Title)
                        .Select(f => f.Copy<Film, FilmModel>());
            ConsoleTable.From(films).Write();
        }

        public static void LinqVsLambda()
        {
            // LVL simple ordered query
            var title = "g";
            var rating = "pg-13";
            var years = new[] { 2016, 2015, 2012 };
            Console.WriteLine($"Films with letter {title} in their title, rated {rating}");
            Console.WriteLine($", filmed in years: {string.Join(", ", years.Select(y => y.ToString()))}");
            Console.WriteLine();
            Console.WriteLine("LINQ");
            var films = (from f in MoviesContext.Instance.Films
                         where f.Title.Contains(title) &&
                            f.Rating == rating &&
                            f.ReleaseYear.HasValue &&
                            years.Contains(f.ReleaseYear.Value)
                         orderby f.ReleaseYear descending,
                            f.Title
                         select f.Copy<Film, FilmModel>());
            ConsoleTable.From(films).Write();
            // Results are identical
            Console.WriteLine("Lambda");
            films = MoviesContext.Instance.Films
                        .Where(f => f.Title.Contains(title) &&
                            f.Rating == rating &&
                            f.ReleaseYear.HasValue &&
                            years.Contains(f.ReleaseYear.Value))
                        .OrderByDescending(f => f.ReleaseYear)
                        .ThenBy(f => f.Title)
                        .Select(f => f.Copy<Film, FilmModel>());
            ConsoleTable.From(films).Write();

            // LVL Groupings
            string delimiter = "---------------------------------------------";

            Console.WriteLine();
            Console.WriteLine(new String('~', delimiter.Length));
            Console.WriteLine();

            Console.WriteLine("Films grouped by rating");
            Console.WriteLine();

            Console.WriteLine("LINQ");
            var filmGroups = (from f in MoviesContext.Instance.Films
                              group f by f.Rating into g
                              select g);
            foreach (var filmGroup in filmGroups)
            {
                Console.WriteLine(filmGroup.Key);
                foreach (var film in filmGroup.OrderBy(f => f.Title))
                {
                    Console.WriteLine($"\t{film.Title}");
                }
            }

            Console.WriteLine();
            Console.WriteLine(delimiter);
            Console.WriteLine();

            Console.WriteLine("Lambda");
            filmGroups = MoviesContext.Instance.Films
                            .GroupBy(f => f.Rating);
            foreach (var filmGroup in filmGroups)
            {
                Console.WriteLine(filmGroup.Key);
                foreach (var film in filmGroup.OrderBy(f => f.Title))
                {
                    Console.WriteLine($"\t{film.Title}");
                }
            }

            Console.WriteLine();
            Console.WriteLine(new String('~', delimiter.Length));
            Console.WriteLine();

            // LVL Joins
            var ratings = new[] {
                new { Code = "G", Name = "General Audiences"},
                new { Code = "PG", Name = "Parental Guidance Suggested"},
                new { Code = "PG-13", Name = "Parents Strongly Cautioned"},
                new { Code = "R", Name = "Restricted"},
            };

            Console.WriteLine($"Films joined with rating names:");
            Console.WriteLine($"{string.Join(", ", ratings.Select(r => r.Code + "=" + r.Name))}");
            Console.WriteLine();

            Console.WriteLine("LINQ");
            var joinedFilms = (from f in MoviesContext.Instance.Films
                         join r in ratings on f.Rating equals r.Code
                         select new { f.Title, r.Code, r.Name });
            ConsoleTable.From(joinedFilms).Write();

            Console.WriteLine("Lambda");
            joinedFilms = MoviesContext.Instance.Films.Join(ratings,
                        f => f.Rating,
                        r => r.Code,
                        (f, r) => new { f.Title, r.Code, r.Name });
            ConsoleTable.From(joinedFilms).Write();
        }

        public static void MigrationAddColumn()
        {
            var film = MoviesContext.Instance.Films
                .FirstOrDefault(f => f.Title.Contains("the first avenger"));
            if (film != null)
            {
                Console.WriteLine($"Updating film with id {film.FilmId}");
                film.Runtime = 124;
                MoviesContext.Instance.SaveChanges();
            }

            var films = MoviesContext.Instance.Films
                            .Select(f => f.Copy<Film, FilmModel>());
            ConsoleTable.From(films).Write();
        }

        public static void MigrationAddTable()
        {
            var user = new ApplicationUser
            {
                UserName = "testuser",
                InvalidLoginAttempts = 0
            };

            MoviesContext.Instance.ApplicationUsers.Add(user);
            MoviesContext.Instance.SaveChanges();

            var users = MoviesContext.Instance.ApplicationUsers;
            ConsoleTable.From(users).Write();
        }

        public static void CompositeKeys()
        {
            var data = new[] {
                new FilmInfo { Title = "Thor", ReleaseYear = 2011, Rating = "PG-13" },
                new FilmInfo { Title = "The Avengers", ReleaseYear = 2012, Rating = "PG-13" },
                new FilmInfo { Title = "Rogue One", ReleaseYear = 2016, Rating = "PG-13" }
            };

            //MoviesContext.Instance.FilmInfos.AddRange(data);
            //MoviesContext.Instance.SaveChanges();

            var infos = MoviesContext.Instance.FilmInfos;
            ConsoleTable.From(infos).Write();
        }

        public static void SelfAssessment()
        {
            Console.WriteLine("Enter a page size:");
            var pageSize = Math.Max(1, Console.ReadLine().ToInt());

            Console.WriteLine("Enter a page number:");
            var pageNumber = Math.Max(1, Console.ReadLine().ToInt());

            Console.WriteLine("Enter a sort column:");
            Console.WriteLine("\ti - Actor ID");
            Console.WriteLine("\tf - Firstname");
            Console.WriteLine("\tl - Lastname");
            var column = Console.ReadKey();
            Console.WriteLine();

            Console.WriteLine("Enter a sort order:");
            Console.WriteLine("\ta - Ascending");
            Console.WriteLine("\td - Descending");
            var order = Console.ReadKey();
            Console.WriteLine();

            Console.WriteLine();

            var queryOrder = order.Key == ConsoleKey.D 
                        ? MoviesContext.Instance.Actors
                        .OrderByDescending(GetColumn(column))
                        : MoviesContext.Instance.Actors
                        .OrderBy(GetColumn(column));
            var actors = queryOrder.Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .Select(a => a.Copy<Actor, ActorModel>());
            ConsoleTable.From(actors).Write();
        }

        /// <summary>
        /// SelfAssessment helper method
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal static Expression<Func<Actor, object>> GetColumn(ConsoleKeyInfo info)
        {
            switch (info.Key)
            {
                case ConsoleKey.I:
                    return a => a.ActorId;
                case ConsoleKey.F:
                    return a => a.FirstName;
                case ConsoleKey.L:
                    return a => a.LastName;
                default:
                    return a => a.ActorId;
            }
        }
    }
}