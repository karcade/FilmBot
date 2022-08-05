﻿using Microsoft.EntityFrameworkCore;
using TelegramBot.Model.DatabaseModels;

namespace TelegtramBot.Model
{
    public class ApplicationContext: DbContext
    {
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        public DbSet<Film> Films { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=FilmDatabase;Trusted_Connection=True;");
        }
    }
}