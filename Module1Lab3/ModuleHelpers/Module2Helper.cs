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
    }
}