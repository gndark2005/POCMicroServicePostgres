using BuildingLink.ModuleServiceTemplate.Models;
using BuildingLink.ModuleServiceTemplate.Models.Builders;
using BuildingLink.ModuleServiceTemplate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingLink.ModuleServiceTemplate.Data.Services
{
    /// <summary>
    /// Set up the database with an initial set of book objects.
    /// </summary>
    public class BookSeedingService: IBookSeedingService
    {
        private readonly IBookRepository _bookRepository;

        /// <summary>
        /// Create new BookSeedingService instance.
        /// </summary>
        /// <param name="bookRepository">Book repository service.</param>
        public BookSeedingService(IBookRepository bookRepository)
        {
            this._bookRepository = bookRepository;
        }

        /// <summary>
        /// Build the books collection.
        /// </summary>
        /// <returns>Books collection.</returns>
        public IEnumerable<Book> SeedBooks()
        {
            var books = new List<Book>()
            {
                BookBuilder.Builder().WithId(Guid.NewGuid())
                    .WithTitle("ABSALOM, ABSALOM!")
                    .WithAuthor("WILLIAM FAULKNER")
                    .WithStatus(BookStatus.Available)
                    .Build(),
                BookBuilder.Builder().WithId(Guid.NewGuid())
                    .WithTitle("A TIME TO KILL")
                    .WithAuthor("JOHN GRISHAM")
                    .WithStatus(BookStatus.Available)
                    .Build(),
                BookBuilder.Builder().WithId(Guid.NewGuid())
                    .WithTitle("THE HOUSE OF MIRTH")
                    .WithAuthor("EDITH WHARTON")
                    .WithStatus(BookStatus.AvailableSoon)
                    .Build(),
                BookBuilder.Builder().WithId(Guid.NewGuid())
                    .WithTitle("EAST OF EDEN")
                    .WithAuthor("JOHN STEINBECK")
                    .WithStatus(BookStatus.InUse)
                    .Build(),
                BookBuilder.Builder().WithId(Guid.NewGuid())
                    .WithTitle("THE SUN ALSO RISES")
                    .WithAuthor("ERNEST HEMINGWAY")
                    .WithStatus(BookStatus.Available)
                    .Build(),
                BookBuilder.Builder().WithId(Guid.NewGuid())
                    .WithTitle("His and Her Summer")
                    .WithAuthor("WILLIAM FAULKNER")
                    .WithStatus(BookStatus.Available)
                    .WithActive(false)
                    .Build(),
            };

            var createTasks = new List<Task<Book>>();
            foreach (var book in books)
            {
                createTasks.Add(this._bookRepository.CreateAsync(book));
            }

            Task.WaitAll(createTasks.ToArray());

            return books;
        }
    }
}
