using BuildingLink.ModuleServiceTemplate.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BuildingLink.ModuleServiceTemplate.Data
{
    /// <summary>
    /// Bd context is the object to represents the database integration.
    /// </summary>
    public class CodeFirstDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFirstDbContext"/> class.
        /// <see cref="CodeFirstDbContext"/> Constructor.
        /// </summary>
        /// <param name="options">Dependency injection options to set up the possibles configurations.</param>
        public CodeFirstDbContext(DbContextOptions<CodeFirstDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets query to Books table.
        /// </summary>
        public DbSet<Book> Books { get; set; }
    }
}
