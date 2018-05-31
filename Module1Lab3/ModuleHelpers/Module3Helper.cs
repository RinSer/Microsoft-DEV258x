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
            // Ordered films with actors
            var films = MoviesContext.Instance.Films
                .OrderBy(f => f.Title)
                .Include(f => f.FilmActor)
                .ThenInclude(a => a.Actor);

            foreach (var film in films)
            {
                Console.WriteLine(new string('-', 78));
                Console.WriteLine($"{film.FilmId}\t{film.ReleaseYear}\t{film.Title}");
                Console.WriteLine(new string('-', 78));
                var actors = film.FilmActor.Select(a => a.Actor)
                                    .OrderBy(a => a.LastName)
                                    .ThenBy(a => a.FirstName);
                foreach (var filmActor in film.FilmActor)
                {
                    Console.WriteLine($"\t{filmActor.ActorId}\t{filmActor.Actor.LastName}, {filmActor.Actor.FirstName}");
                }
            }
            // Sorted actors with films
            var actors2 = MoviesContext.Instance.Actors
                .OrderBy(a => a.LastName)
                .ThenBy(a => a.FirstName)
                .Include(a => a.FilmActor)
                .ThenInclude(f => f.Film);
            foreach (var actor in actors2)
            {
                Console.WriteLine(new string('-', 78));
                Console.WriteLine($"{actor.ActorId}\t{actor.LastName}, {actor.FirstName}");
                Console.WriteLine(new string('-', 78));
                var films2 = actor.FilmActor.Select(a => a.Film).OrderByDescending(f => f.ReleaseYear);
                foreach (var film in films2)
                {
                    Console.WriteLine($"\t{film.FilmId}\t{film.ReleaseYear}\t{film.Title}");
                }
            }
        }

        public static void ManyToManyInsert()
        {
            // Add actor to a film
            var filmId = 12;
            var film = MoviesContext.Instance.Films
                            .Include(f => f.FilmActor)
                            .ThenInclude(a => a.Actor)
                            .Single(f => f.FilmId == filmId);
            var actor = new Actor { FirstName = "Stan", LastName = "Lee" };

            if (film.FilmActor.All(fa => fa.ActorId != actor.ActorId))
            {
                Console.WriteLine("Adding actor to film.");
                film.FilmActor.Add(new FilmActor
                {
                    Film = film,
                    Actor = actor
                });
            }
            MoviesContext.Instance.SaveChanges();

            film = MoviesContext.Instance.Films
                            .Include(f => f.FilmActor)
                            .ThenInclude(a => a.Actor)
                            .Single(f => f.FilmId == filmId);

            foreach (var filmActor in film.FilmActor)
            {
                Console.WriteLine($"{filmActor.ActorId}\t{filmActor.Actor.FirstName}\t{filmActor.Actor.LastName}");
            }
            // Add film to an actor
            var actorId = 3;
            filmId = 12;

            actor = MoviesContext.Instance.Actors
                            .Include(a => a.FilmActor)
                            .ThenInclude(fa => fa.Film)
                            .Single(a => a.ActorId == actorId);
            if (actor.FilmActor.All(fa => fa.FilmId != filmId))
            {
                Console.WriteLine("Adding film to actor");
                actor.FilmActor.Add(new FilmActor { Actor = actor, FilmId = filmId });
                MoviesContext.Instance.SaveChanges();
            }

            actor = MoviesContext.Instance.Actors
                            .Include(a => a.FilmActor)
                            .ThenInclude(fa => fa.Film)
                            .Single(a => a.ActorId == actorId);
            foreach (var filmWithActor in actor.FilmActor.Select(fa => fa.Film))
            {
                Console.WriteLine($"{filmWithActor.FilmId}\t{filmWithActor.ReleaseYear}\t{filmWithActor.Title}");
            }
        }

        public static void ManyToManyDelete()
        {
            var filmActor = GetRandomFilmActor();
            var filmId = filmActor.FilmId;
            var actorId = filmActor.ActorId;

            Write(filmActor);

            var entity = new FilmActor
            {
                FilmId = filmId,
                ActorId = actorId
            };
            MoviesContext.Instance.FilmActors.Remove(entity);
            MoviesContext.Instance.SaveChanges();

            filmActor = MoviesContext.Instance.FilmActors
                            .SingleOrDefault(fa => fa.FilmId == filmId &&
                                                    fa.ActorId == actorId);

            Write(filmActor);

            MoviesContext.Instance.FilmActors.Add(new FilmActor
            {
                FilmId = filmId,
                ActorId = actorId
            });
            MoviesContext.Instance.SaveChanges();

            filmActor = MoviesContext.Instance.FilmActors
                            .SingleOrDefault(fa => fa.FilmId == filmId &&
                                                    fa.ActorId == actorId);

            Write(filmActor);
        }

        // Helper methods for many to many delete
        private static void Write(FilmActor filmActor)
        {
            if (filmActor == null)
            {
                Console.WriteLine("Film Actor Not Found");
                return;
            }

            var film = filmActor.Film;
            var actor = filmActor.Actor;
            if (film == null)
            {
                film = MoviesContext.Instance.Films
                            .FirstOrDefault(f => f.FilmId == filmActor.FilmId);
            }
            if (actor == null)
            {
                actor = MoviesContext.Instance.Actors
                            .FirstOrDefault(a => a.ActorId == filmActor.ActorId);
            }

            Console.WriteLine($"Film: {film.FilmId}  -  {film.Title}\t Actor: {actor.ActorId}  -  {actor.FirstName} {actor.LastName}");
        }

        private static FilmActor GetRandomFilmActor()
        {
            int count = MoviesContext.Instance.FilmActors.Count();
            var skip = new Random().Next(0, count);
            return MoviesContext.Instance.FilmActors
                        .Skip(skip)
                        .Select(fa => new FilmActor
                        {
                            FilmId = fa.FilmId,
                            ActorId = fa.ActorId
                        })
                        .First();
        }

        /// Loading Lab
        ///

        public static void LazyLoadFilm()
        {
            var filmId = 4;
            var film = MoviesContext.Instance.Films.Single(f => f.FilmId == filmId);
            Console.WriteLine($"{film.FilmId} - {film.Title}");
            MoviesContext.Instance.Entry(film).Collection(f => f.FilmActor).Load();
            foreach (var filmActor in film.FilmActor)
            {
                MoviesContext.Instance.Entry(filmActor).Reference(fa => fa.Actor).Load();
                Console.WriteLine($"\tfilm id: {filmActor.FilmId} actor id: {filmActor.ActorId}");
                Console.WriteLine($"\t\tactor id: {filmActor.Actor.ActorId} - {filmActor.Actor.FirstName} {filmActor.Actor.LastName}");
            }
        }

        public static void LazyLoadCategory()
        {
            var categories = MoviesContext.Instance.Categories;
            foreach (var category in categories)
            {
                Console.WriteLine($"Category: {category.CategoryId} - {category.Name}");
                MoviesContext.Instance.Entry(category).Collection(c => c.FilmCategory).Load();
                if (category.FilmCategory.Any())
                {
                    foreach (var filmCategory in category.FilmCategory)
                    {
                        MoviesContext.Instance.Entry(filmCategory).Reference(fc => fc.Film).Load();
                        Console.WriteLine($"\t{filmCategory.Film.FilmId} - {filmCategory.Film.Title}");
                    }
                }
                else
                {
                    Console.WriteLine("\tNo Films");
                }
            }
        }

        public static void EagerLoadFilm()
        {
            var filmId = 4;
            var film = MoviesContext.Instance.Films
                        .Include(f => f.FilmActor)
                            .ThenInclude(fa => fa.Actor)
                        .Single(f => f.FilmId == filmId);
            Console.WriteLine($"{film.FilmId} - {film.Title}");
            foreach (var filmActor in film.FilmActor)
            {
                Console.WriteLine($"\tfilm id: {filmActor.FilmId} actor id: {filmActor.ActorId}");
                if (filmActor.Actor != null)
                {
                    Console.WriteLine($"\t\tactor id: {filmActor.Actor.ActorId} - {filmActor.Actor.FirstName} {filmActor.Actor.LastName}");
                }
            }
        }

        public static void EagerLoadCategory()
        {
            var categories = MoviesContext.Instance.Categories
                    .Include(c => c.FilmCategory)
                        .ThenInclude(fc => fc.Film);
            foreach (var category in categories)
            {
                Console.WriteLine($"Category: {category.CategoryId} - {category.Name}");
                MoviesContext.Instance.Entry(category).Collection(c => c.FilmCategory);
                if (category.FilmCategory.Any())
                {
                    foreach (var filmCategory in category.FilmCategory)
                    {
                        MoviesContext.Instance.Entry(filmCategory).Reference(fc => fc.Film);
                        Console.WriteLine($"\t{filmCategory.Film.FilmId} - {filmCategory.Film.Title}");
                    }
                }
                else
                {
                    Console.WriteLine("\tNo Films");
                }
            }
        }

        public static void SelfAssessment()
        {
            var ratingCode = "PG-13";
            var rating = MoviesContext.Instance.Ratings
                .Where(r => r.Code.Equals(ratingCode))
                .Include(r => r.Films).FirstOrDefault();

            foreach(var film in rating.Films)
            {
                if (film.ReleaseYear != null)
                {
                    if (film.ReleaseYear % 2 == 0)
                        film.Title = film.Title.Insert(0, "even ");
                    else
                        film.Title = film.Title.Insert(0, "odd ");
                }
            }

            MoviesContext.Instance.SaveChanges();

            WriteFilmsByRatingCode(ratingCode);

            foreach (var film in rating.Films)
            {
                if (film.ReleaseYear != null)
                {
                    if (film.ReleaseYear % 2 == 0)
                        film.Title = film.Title.Replace("even ", "");
                    else
                        film.Title = film.Title.Replace("odd ", "");
                }
            }

            MoviesContext.Instance.SaveChanges();

            WriteFilmsByRatingCode(ratingCode);
        }

        public static void WriteFilmsByRatingCode(string ratingCode)
        {
            var rating = MoviesContext.Instance.Ratings
                .Where(r => r.Code.Equals(ratingCode))
                .Include(r => r.Films).FirstOrDefault();

            Console.WriteLine(new string('~', 25));
            Console.WriteLine($"Rating ID: {rating.RatingId}   Code: {rating.Code}    Name: {rating.Name}");
            Console.WriteLine(new string('~', 25));
            foreach (var film in rating.Films)
            {
                Console.WriteLine($"{film.FilmId}   {film.ReleaseYear}  {film.Title}");
            }
        }
    }
}