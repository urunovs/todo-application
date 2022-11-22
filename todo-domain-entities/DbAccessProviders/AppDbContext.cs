using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace todo_domain_entities
{
    /// <summary>
    /// AppDbContext class providing database processes with T0D0 lists
    /// </summary>
    public class AppDbContext : DbContext
    {
        public DbSet<ToDoList> ToDoLists { get; set; }
        public DbSet<ToDoEntry> ToDoEntries { get; set; }

        public AppDbContext(DbContextOptions<DbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseLazyLoadingProxies();
    }
}
