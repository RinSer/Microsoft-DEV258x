using ConsoleTables;
using Microsoft.EntityFrameworkCore;
using MovieApp.Entities;
using MovieApp.Extensions;
using MovieApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieApp
{
    public static class Module4Helper
    {
        public static void CommitTransaction()
        {
            var film = new Film
            {
                Title = "Willy Wonka & the Chocolate Factory",
                ReleaseYear = 1971,
                RatingId = 1
            };
            var actor = new Actor
            {
                FirstName = "Gene",
                LastName = "Wilder"
            };

            using (var transaction = MoviesContext.Instance.Database.BeginTransaction())
            {
                MoviesContext.Instance.Films.Add(film);
                MoviesContext.Instance.SaveChanges();

                MoviesContext.Instance.Actors.Add(actor);
                MoviesContext.Instance.SaveChanges();

                MoviesContext.Instance.Database.CommitTransaction();
            }

            var filmFound = MoviesContext.Instance.Films
                                .Any(f => f.Title == film.Title &&
                                        f.ReleaseYear == film.ReleaseYear &&
                                        f.RatingId == film.RatingId) ? "Yes" : "No";
            var actorFound = MoviesContext.Instance.Actors
                                .Any(a => a.FirstName == actor.FirstName &&
                                        a.LastName == actor.LastName) ? "Yes" : "No";

            Console.Write($"Film Found: {filmFound}\nActor Found: {actorFound}\n");
        }

        public static void RollbackTransactionException()
        {
            var count = MoviesContext.Instance.Films.Count();
            Console.WriteLine($"Film Count: {count}");
            using (var transaction = MoviesContext.Instance.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < 5; i++)
                    {
                        MoviesContext.Instance.Films.Add(new Film { Title = $"Temp Film {i}" });
                    }
                    MoviesContext.Instance.SaveChanges();

                    throw new ApplicationException("Simulated exception");

                    transaction.Commit();
                    Console.WriteLine("Transaction committed");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception encountered: {ex.Message}");
                    transaction.Rollback();
                    Console.WriteLine("Transaction rolled back");
                }
            }
            count = MoviesContext.Instance.Films.Count();
            Console.WriteLine($"Film Count: {count}");

            // clean up after ourselves
            var films = MoviesContext.Instance.Films.Where(f => f.Title.StartsWith("temp"));
            MoviesContext.Instance.Films.RemoveRange(films);
            MoviesContext.Instance.SaveChanges();
        }

        public static void RollbackTransactionBusinessRule()
        {
            var count = MoviesContext.Instance.Films.Count();
            Console.WriteLine($"Film Count: {count}");
            using (var transaction = MoviesContext.Instance.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < 5; i++)
                    {
                        MoviesContext.Instance.Films.Add(new Film { Title = $"Temp Film {i}" });
                    }
                    MoviesContext.Instance.SaveChanges();

                    var cancelReceivedFromUser = true;
                    if (cancelReceivedFromUser)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Transaction rolled back due to user cancel");
                    }
                    else
                    {
                        transaction.Commit();
                        Console.WriteLine("Transaction committed");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception encountered: {ex.Message}");
                    transaction.Rollback();
                    Console.WriteLine("Transaction rolled back due to exception");
                }
            }

            count = MoviesContext.Instance.Films.Count();
            Console.WriteLine($"Film Count: {count}");

            // clean up after ourselves
            var films = MoviesContext.Instance.Films.Where(f => f.Title.StartsWith("temp"));
            MoviesContext.Instance.Films.RemoveRange(films);
            MoviesContext.Instance.SaveChanges();
        }

        public static void QueryOptimization()
        {
            /*Console.WriteLine("Loading Data Unoptimized...");

            var films = MoviesContext.Instance.Films;
            foreach (var film in films)
            {
                MoviesContext.Instance.Entry(film).Collection(f => f.FilmActor).Load();
                foreach (var filmActor in film.FilmActor)
                {
                    MoviesContext.Instance.Entry(filmActor).Reference(fa => fa.Actor).Load();
                }
            }

            Console.WriteLine("Done");*/

            Console.WriteLine("Loading Data Optimized...");

            var filmsWithActors = MoviesContext.Instance.Films
                                .Include(f => f.FilmActor)
                                .ThenInclude(fa => fa.Actor)
                                .ToList();

            foreach (var film in filmsWithActors)
            {
                foreach (var filmActor in film.FilmActor)
                {
                    // do nothing
                }
            }

            Console.WriteLine("Done");
        }

        public static void DetachedEntities()
        {
            const int actorId = 1;
            var name = MoviesContext.Instance.Actors.Where(a => a.ActorId == actorId).Select(a => $"{a.FirstName} {a.LastName}").First();
            Console.WriteLine($"Original Name:\t{name}");

            var actorModel = new ActorModel
            {
                ActorId = actorId,
                FirstName = "Luke",
                LastName = "Skywalker"
            };
            var actor = actorModel.Copy<ActorModel, Actor>();

            MoviesContext.Instance.Actors.Attach(actor);
            MoviesContext.Instance.Entry(actor).State = EntityState.Modified;

            MoviesContext.Instance.SaveChanges();

            name = MoviesContext.Instance.Actors.Where(a => a.ActorId == actorId).Select(a => $"{a.FirstName} {a.LastName}").First();
            Console.WriteLine($"Updated Name:\t{name}");

            actor.FirstName = "Mark";
            actor.LastName = "Hammil";

            MoviesContext.Instance.SaveChanges();

            name = MoviesContext.Instance.Actors.Where(a => a.ActorId == actorId).Select(a => $"{a.FirstName} {a.LastName}").First();
            Console.WriteLine($"Reverted Name:\t{name}");

            Console.WriteLine();

            const int filmId = 1;
            var year = MoviesContext.Instance.Films.Where(f => f.FilmId == filmId).Select(f => f.ReleaseYear).First();
            Console.WriteLine($"Original Release Year:\t{year}");

            var model = new FilmUpdateModel
            {
                FilmId = filmId,
                Description = "As Steve Rogers struggles to embrace his role in the modern world, he teams up with a fellow Avenger and S.H.I.E.L.D agent, Black Widow, to battle a new threat from history: an assassin known as the Winter Soldier.",
                Title = "Captain America: The Winter Soldier",
                RatingId = 3,
                ReleaseYear = 2013
            };

            var film = model.Copy<FilmUpdateModel, Film>();

            MoviesContext.Instance.Films.Attach(film);
            MoviesContext.Instance.Entry(film).State = EntityState.Modified;

            MoviesContext.Instance.SaveChanges();

            year = MoviesContext.Instance.Films.Where(f => f.FilmId == filmId).Select(f => f.ReleaseYear).First();
            Console.WriteLine($"Updated Release Year:\t{year}");

            film.ReleaseYear = 2014;
            MoviesContext.Instance.SaveChanges();

            year = MoviesContext.Instance.Films.Where(f => f.FilmId == filmId).Select(f => f.ReleaseYear).First();
            Console.WriteLine($"Reverted Release Year:\t{year}");
        }

        public static void ExecuteRawSql()
        {
            var sql = "select * from actor where actorid = ?";
            var actors = MoviesContext.Instance.Actors
                                    .FromSql(sql, 2)
                                    .Select(a => a.Copy<Actor, ActorModel>());
            ConsoleTable.From(actors).Write();

            sql = "select * from film limit 1";
            var films = MoviesContext.Instance.Films
                                    .FromSql(sql)
                                    .Select(f => f.Copy<Film, FilmModel>());
            ConsoleTable.From(films).Write();
        }

        public static void ExecuteStoredProcedure()
        {
            var sql = "call FilmStartsWithTitle({0})";
            var films = MoviesContext.Instance.Films
                                    .FromSql(sql, "t")
                                    .Select(f => f.Copy<Film, FilmModel>());
            ConsoleTable.From(films).Write();

            sql = "call FindActor({0})";
            var actors = MoviesContext.Instance.Actors
                                    .FromSql(sql, "on")
                                    .Select(a => a.Copy<Actor, ActorModel>());
            ConsoleTable.From(actors).Write();
        }

        public static void SeedDatabase()
        {
            WriteDataCount();
            AddOrUpdateSeedData();
            WriteDataCount();
        }

        public static void WriteDataCount()
        {
            var count = MoviesContext.Instance.Films.Count();
            Console.WriteLine($"films: {count}");
            count = MoviesContext.Instance.Categories.Count();
            Console.WriteLine($"categories: {count}");
            count = MoviesContext.Instance.FilmCategories.Count();
            Console.WriteLine($"film categories: {count}");
        }

        private static List<Film> Films
        {
            get
            {
                var id = 1;
                return new[] {
                    new Film { FilmId = id++, Title="Captain America: The Winter Soldier", Description = "As Steve Rogers struggles to embrace his role in the modern world, he teams up with a fellow Avenger and S.H.I.E.L.D agent, Black Widow, to battle a new threat from history: an assassin known as the Winter Soldier.", ReleaseYear = 2014, RatingCode= "PG-13"},
                    new Film { FilmId = id++, Title = "The Avengers", Description = "Earth's mightiest heroes must come together and learn to fight as a team if they are to stop the mischievous Loki and his alien army from enslaving humanity.",ReleaseYear = 2012, RatingCode= "PG-13"},
                    new Film { FilmId = id++, Title="Thor", Description = "The powerful but arrogant god Thor is cast out of Asgard to live amongst humans in Midgard (Earth), where he soon becomes one of their finest defenders.", ReleaseYear = 2011, RatingCode ="PG-13"},
                    new Film { FilmId = id++, Title="Avengers: Age of Ultron", Description = "When Tony Stark and Bruce Banner try to jump-start a dormant peacekeeping program called Ultron, things go horribly wrong and it's up to Earth's mightiest heroes to stop the villainous Ultron from enacting his terrible plan.", ReleaseYear = 2015, RatingCode = "PG-13"},
                    new Film { FilmId = id++, Title="Captain America: The First Avenger", Description = "Steve Rogers, a rejected military soldier transforms into Captain America after taking a dose of a \"Super-Soldier serum\". But being Captain America comes at a price as he attempts to take down a war monger and a terrorist organization.", ReleaseYear = 2011, RatingCode ="PG-13"},
                    new Film { FilmId = id++, Title="Star Wars: Episode IV - A New Hope", Description = "Luke Skywalker joins forces with a Jedi Knight, a cocky pilot, a Wookiee, and two droids to save the galaxy from the Empire's world-destroying battle-station, while also attempting to rescue Princess Leia from the evil Darth Vader. ", ReleaseYear = 1977, RatingCode ="PG"},
                    new Film { FilmId = id++, Title="Star Wars: Episode V - The Empire Strikes Back", Description = "After the rebels are overpowered by the Empire on their newly established base, Luke Skywalker begins Jedi training with Master Yoda. His friends accept shelter from a questionable ally as Darth Vader hunts them in a plan to capture Luke.", ReleaseYear = 1980, RatingCode ="PG"},
                    new Film { FilmId = id++, Title="Star Wars: Episode VI - Return of the Jedi", Description = "After a daring mission to rescue Han Solo from Jabba the Hutt, the rebels dispatch to Endor to destroy a more powerful Death Star. Meanwhile, Luke struggles to help Vader back from the dark side without falling into the Emperor's trap.", ReleaseYear = 1983, RatingCode = "PG"},
                    new Film { FilmId = id++, Title="Star Wars: Episode I - The Phantom Menace", Description = "Two Jedi Knights escape a hostile blockade to find allies and come across a young boy who may bring balance to the Force, but the long dormant Sith resurface to claim their old glory.", ReleaseYear = 1999, RatingCode = "PG"},
                    new Film { FilmId = id++, Title="Star Wars: Episode II - Attack of the Clones", Description = "Ten years after initially meeting, Anakin Skywalker shares a forbidden romance with Padmé, while Obi-Wan investigates an assassination attempt on the Senator and discovers a secret clone army crafted for the Jedi.", ReleaseYear = 2002, RatingCode = "PG"},
                    new Film { FilmId = id++, Title="Star Wars: Episode III - Revenge of the Sith", Description = "Three years into the Clone Wars, the Jedi rescue Palpatine from Count Dooku. As Obi-Wan pursues a new threat, Anakin acts as a double agent between the Jedi Council and Palpatine and is lured into a sinister plan to rule the galaxy.",ReleaseYear =  2005, RatingCode = "PG-13"},
                    new Film { FilmId = id++, Title="Rogue One", Description = "Three decades after the Empire's defeat, a new threat arises in the militant First Order. Stormtrooper defector Finn and spare parts scavenger Rey are caught up in the Resistance's search for the missing Luke Skywalker.", ReleaseYear = 2016, RatingCode = "PG-13"}
                }.ToList();
            }
        }

        private static List<Category> Categories
        {
            get
            {
                var id = 1;
                return new[] {
                    new Category{ CategoryId = id++, Name="Action"},
                    new Category{ CategoryId = id++, Name="Animation"},
                    new Category{ CategoryId = id++, Name="Children"},
                    new Category{ CategoryId = id++, Name="Classics"},
                    new Category{ CategoryId = id++, Name="Comedy"},
                    new Category{ CategoryId = id++, Name="Documentary"},
                    new Category{ CategoryId = id++, Name="Drama"},
                    new Category{ CategoryId = id++, Name="Family"},
                    new Category{ CategoryId = id++, Name="Foreign"},
                    new Category{ CategoryId = id++, Name="Games"},
                    new Category{ CategoryId = id++, Name="Horror"},
                    new Category{ CategoryId = id++, Name="Music"},
                    new Category{ CategoryId = id++, Name="New"},
                    new Category{ CategoryId = id++, Name="Sci-Fi"},
                    new Category{ CategoryId = id++, Name="Sports"},
                    new Category{ CategoryId = id++, Name="Travel"}
                }.ToList();
            }
        }

        private static List<FilmCategory> FilmCategories
        {
            get
            {
                return new[] {
                    new FilmCategory { FilmId = 1, CategoryId = 1 },
                    new FilmCategory { FilmId = 2, CategoryId = 1 },
                    new FilmCategory { FilmId = 3, CategoryId = 1 },
                    new FilmCategory { FilmId = 4, CategoryId = 1 },
                    new FilmCategory { FilmId = 5, CategoryId = 1 },
                    new FilmCategory { FilmId = 6, CategoryId = 4 },
                    new FilmCategory { FilmId = 7, CategoryId = 4 },
                    new FilmCategory { FilmId = 8, CategoryId = 4 },
                    new FilmCategory { FilmId = 6, CategoryId = 14 },
                    new FilmCategory { FilmId = 7, CategoryId = 14 },
                    new FilmCategory { FilmId = 8, CategoryId = 14 },
                    new FilmCategory { FilmId = 9, CategoryId = 14 },
                    new FilmCategory { FilmId = 10, CategoryId = 14 },
                    new FilmCategory { FilmId = 11, CategoryId = 14 },
                    new FilmCategory { FilmId = 12, CategoryId = 14 }
                }.ToList();
            }
        }

        private static void AddSeedData()
        {
            if (!MoviesContext.Instance.Films.Any())
            {
                Console.WriteLine("Seeding films");
                Films.ForEach(f =>
                {
                    f.FilmId = 0;
                    MoviesContext.Instance.Films.Add(f);
                });
                Console.WriteLine("Done");
            }
            else
            {
                Console.WriteLine("Skipping films");
            }

            if (!MoviesContext.Instance.Categories.Any())
            {
                Console.WriteLine("Seeding categories");
                Categories.ForEach(c =>
                {
                    c.CategoryId = 0;
                    MoviesContext.Instance.Categories.Add(c);
                });
                Console.WriteLine("Done");
            }
            else
            {
                Console.WriteLine("Skipping categories");
            }

            if (!MoviesContext.Instance.FilmCategories.Any())
            {
                Console.WriteLine("Seeding film categories");
                FilmCategories.ForEach(fc => MoviesContext.Instance.FilmCategories.Add(fc));
                Console.WriteLine("Done");
            }
            else
            {
                Console.WriteLine("Skipping film categories");
            }

            MoviesContext.Instance.SaveChanges();
        }

        private static void AddOrUpdateSeedData()
        {
            var updatedCount = 0;
            var addedCount = 0;

            Console.WriteLine("Seeding films");
            Films.ForEach(f =>
            {
                var film = MoviesContext.Instance.Films.SingleOrDefault(e => e.FilmId == f.FilmId);
                if (film == null)
                {
                    MoviesContext.Instance.Films.Attach(f);
                    MoviesContext.Instance.Entry(f).State = EntityState.Added;
                    addedCount++;
                }
                else
                {
                    f.Copy(film);
                    updatedCount++;
                }
            });
            Console.WriteLine($"Done. Updated: {updatedCount}, Added: {addedCount}");

            addedCount = updatedCount = 0;
            Console.WriteLine("Seeding categories");
            Categories.ForEach(c =>
            {
                var category = MoviesContext.Instance.Categories.SingleOrDefault(e => e.CategoryId == c.CategoryId);
                if (category == null)
                {
                    MoviesContext.Instance.Categories.Attach(c);
                    MoviesContext.Instance.Entry(c).State = EntityState.Added;
                    addedCount++;
                }
                else
                {
                    c.Copy(category);
                    updatedCount++;
                }
            });
            Console.WriteLine($"Done. Updated: {updatedCount}, Added: {addedCount}");

            addedCount = updatedCount = 0;
            Console.WriteLine("Seeding film categories");
            FilmCategories.ForEach(fc =>
            {
                var filmCategory = MoviesContext.Instance.FilmCategories
                                    .SingleOrDefault(e => e.FilmId == fc.FilmId && e.CategoryId == fc.CategoryId);
                if (filmCategory == null)
                {
                    MoviesContext.Instance.FilmCategories.Attach(fc);
                    MoviesContext.Instance.Entry(fc).State = EntityState.Added;
                    addedCount++;
                }
            });
            Console.WriteLine($"Done. Updated: {updatedCount}, Added: {addedCount}");

            MoviesContext.Instance.SaveChanges();
        }

        public static void SelfAssessment()
        {
            Console.WriteLine("Input from which row do you want to see the films:");
            int offset = Console.ReadLine().ToInt();
            Console.WriteLine("Input how many films you want to be displayed:");
            int count = Console.ReadLine().ToInt();

            var films = MoviesContext.Instance.Films
                .FromSql("CALL loadFilms({0}, {1})", offset, count)
                .Select(f => f.Copy<Film, FilmModel>());

            ConsoleTable.From<FilmModel>(films).Write();
            Console.ReadKey();
        }
    }
}