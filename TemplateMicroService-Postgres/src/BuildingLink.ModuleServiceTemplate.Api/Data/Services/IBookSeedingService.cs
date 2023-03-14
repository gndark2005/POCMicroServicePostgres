using BuildingLink.ModuleServiceTemplate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingLink.ModuleServiceTemplate.Data.Services
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
