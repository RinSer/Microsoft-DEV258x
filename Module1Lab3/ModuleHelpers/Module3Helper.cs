using ConsoleTables;
using Microsoft.EntityFrameworkCore;
using MovieApp.Entities;
using MovieApp.Extensions;
using System;
using System.Linq;

namespace MovieApp
{
    public static class Module3Helper
    {
        // Helper method
        private static FilmDetailModel CreateFilmDetailModel(Film film)
        {
            var model = film.Copy<Film, FilmDetailModel>();

            if (film.FilmImage != null)
            {
                model.FilmImageId = film.FilmImage.FilmImageId;
            }

            return model;
        }

        // Helper method overload
        private static FilmDetailModel CreateFilmDetailModel(FilmImage image)
        {
            var model = image.Film.Copy<Film, FilmDetailModel>();

            model.FilmImageId = image.FilmImageId;

            return model;
        }

        public static void OneToOne()
        {
            var films = MoviesContext.Instance.Films.Include(f => f.FilmImage)
                .Where(f => f.FilmImage == null)
                .Select(CreateFilmDetailModel);
            ConsoleTable.From(films).Write();

            films = MoviesContext.Instance.FilmImages.Include(i => i.Film)
                .Select(CreateFilmDetailModel);
            ConsoleTable.From(films).Write();
        }

        public static void OneToMany()
        {
            // Films by random rating
            var ratingsQuery = MoviesContext.Instance.Ratings;
            int skip = new Random().Next(0, ratingsQuery.Count());
            var ratings = ratingsQuery.Skip(skip).Take(1);

            var ratingId = ratings.First().RatingId;

            var rating = MoviesContext.Instance.Ratings.First(r => r.RatingId == ratingId);

            Console.WriteLine(new string('-', 78));
            Console.WriteLine($"{rating.Code}\t{rating.Name}");
            Console.WriteLine(new string('-', 78));
            var films = MoviesContext.Instance.Films.Where(f => f.RatingId == rating.RatingId)
                            .OrderBy(f => f.ReleaseYear);
            if (films.Any())
            {
                Console.WriteLine($"\tID\tYear\tTitle");
                Console.WriteLine($"\t{new string('-', 70)}");
                foreach (var film in films.OrderByDescending(f => f.ReleaseYear))
                {
                    Console.WriteLine($"\t{film.FilmId}\t{film.ReleaseYear}\t{film.Title}");
                }
            }
            else
            {
                Console.WriteLine("\tNo Films");
            }
            // Rating by random film
            Console.WriteLine();
            Console.WriteLine(new string('-', 78));
            var filmsQuery = MoviesContext.Instance.Films;
            skip = new Random().Next(0, filmsQuery.Count());

            var filmId = filmsQuery.Skip(skip).First().FilmId;

            var film2 = MoviesContext.Instance.Films.First(f => f.FilmId == filmId);
            var rating2 = MoviesContext.Instance.Ratings.FirstOrDefault(r => r.RatingId == film2.RatingId);
            Console.WriteLine($"{film2.FilmId}\t{film2.Title}\t{rating2.Code}\t{rating2.Name}");
        }

        public static void ManyToManySelect()
        {
            Console.WriteLine(nameof(ManyToManySelect));
        }

        public static void ManyToManyInsert()
        {
            Console.WriteLine(nameof(ManyToManyInsert));
        }

        public static void ManyToManyDelete()
        {
            Console.WriteLine(nameof(ManyToManyDelete));
        }
    }
}