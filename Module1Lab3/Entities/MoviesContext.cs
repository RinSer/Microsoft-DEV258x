﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace MovieApp.Entities
{
    public partial class MoviesContext : DbContext
    {
        public virtual DbSet<Actor> Actors { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Film> Films { get; set; }
        public virtual DbSet<FilmActor> FilmActors { get; set; }
        public virtual DbSet<FilmCategory> FilmCategories { get; set; }
        // Added table
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
        // Added table with a composite key
        public virtual DbSet<FilmInfo> FilmInfos { get; set; }
        // Added table for one to one relational entities
        public virtual DbSet<FilmImage> FilmImages { get; set; }
        // Added table for one to many relational entities
        public virtual DbSet<Rating> Ratings { get; set; }

        private static MoviesContext _context;

        //public static readonly LoggerFactory MyLoggerFactory = new LoggerFactory(
            //new[] { new ConsoleLoggerProvider((_, __) => true, true) });

        public static MoviesContext Instance
        {
            get
            {
                if (_context == null)
                {
                    _context = new MoviesContext();
                    //var serviceProvider = _context.GetInfrastructure<IServiceProvider>();
                    //var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                    //loggerFactory.AddProvider(new ConsoleLoggerProvider((_, __) => true, true));
                }
                return _context;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder
                    //.UseLoggerFactory(MyLoggerFactory)
                    .UseMySql("server=localhost;userid=root;pwd=123456;port=3306;database=Movies;sslmode=none;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Actor>(entity =>
            {
                entity.ToTable("actor");

                entity.Property(e => e.ActorId)
                    .HasColumnType("int(11)");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(45);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category");

                entity.Property(e => e.CategoryId)
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name).HasMaxLength(45);
            });

            modelBuilder.Entity<Film>(entity =>
            {
                entity.ToTable("film");

                entity.Property(e => e.FilmId)
                    .HasColumnType("int(11)");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.RatingCode).HasMaxLength(45);

                entity.Property(e => e.ReleaseYear).HasColumnType("int(11)");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasIndex(e => e.RatingCode)
                    .HasName("film_rating_index");
            });

            modelBuilder.Entity<FilmActor>(entity =>
            {
                entity.HasKey(e => new { e.FilmId, e.ActorId });

                entity.ToTable("film_actor");

                entity.HasIndex(e => e.ActorId)
                    .HasName("film_actor_actor_idx");

                entity.Property(e => e.FilmId).HasColumnType("int(11)");

                entity.Property(e => e.ActorId).HasColumnType("int(11)");

                entity.HasOne(d => d.Actor)
                    .WithMany(p => p.FilmActor)
                    .HasForeignKey(d => d.ActorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("film_actor_actor");

                entity.HasOne(d => d.Film)
                    .WithMany(p => p.FilmActor)
                    .HasForeignKey(d => d.FilmId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("film_actor_film");
            });

            modelBuilder.Entity<FilmCategory>(entity =>
            {
                entity.HasKey(e => new { e.FilmId, e.CategoryId });

                entity.ToTable("film_category");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("film_category_category_fk_idx");

                entity.Property(e => e.FilmId).HasColumnType("int(11)");

                entity.Property(e => e.CategoryId).HasColumnType("int(11)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.FilmCategory)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("film_category_category_fk");

                entity.HasOne(d => d.Film)
                    .WithMany(p => p.FilmCategory)
                    .HasForeignKey(d => d.FilmId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("film_category_film_fk");
            });

            modelBuilder.Entity<FilmInfo>(entity =>
            {
                entity.HasKey(e => new { e.Title, e.ReleaseYear });
            });
        }
    }
}
