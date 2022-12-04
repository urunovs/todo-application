using Microsoft.EntityFrameworkCore;

namespace todo_domain_entities
{
    /// <summary>
    /// AppDbContext class providing database processes with T0D0 lists
    /// </summary>
    public class AppDbContext : DbContext
    {
        public virtual DbSet<ToDoList> ToDoLists { get; set; }
        public virtual DbSet<ToDoEntry> ToDoEntries { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseLazyLoadingProxies();
    }
}
