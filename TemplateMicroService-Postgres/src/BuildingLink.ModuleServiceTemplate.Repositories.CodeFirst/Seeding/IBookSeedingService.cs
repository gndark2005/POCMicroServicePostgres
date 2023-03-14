using System.Collections.Generic;

namespace BuildingLink.ModuleServiceTemplate.Repositories.Seeding
{
    /// <summary>
    /// Set up the database with an initial set of book objects.
    /// </summary>
    public interface IBookSeedingService
    {
        /// <summary>
        /// Build the books collection.
        /// </summary>
        /// <returns>Books collection.</returns>
        IEnumerable<Book> SeedBooks();
    }
}
