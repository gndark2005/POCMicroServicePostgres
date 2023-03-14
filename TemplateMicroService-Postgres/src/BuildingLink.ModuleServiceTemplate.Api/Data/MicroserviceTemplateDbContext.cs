using BuildingLink.ModuleServiceTemplate.Models;
using Microsoft.EntityFrameworkCore;

namespace BuildingLink.ModuleServiceTemplate.Data
{
    /// <summary>
    /// Bd context is the object to represents the database integration.
    /// </summary>
    public class ModuleServiceTemplateDbContext: DbContext
    {
        /// <summary>
        /// ModuleServiceTemplateDbContext Constructor 
        /// </summary>
        /// <param name="options">Dependency injection options to set up the possibles configurations.</param>
        public ModuleServiceTemplateDbContext(DbContextOptions<ModuleServiceTemplateDbContext> options)
            :base(options)
        {

        }

        /// <summary>
        /// Query to Books table
        /// </summary>
        public DbSet<Book> Books { get; set; }
    }
}
