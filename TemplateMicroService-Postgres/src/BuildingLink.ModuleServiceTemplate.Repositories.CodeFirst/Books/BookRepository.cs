using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BuildingLink.ModuleServiceTemplate.Data;
using Microsoft.EntityFrameworkCore;

namespace BuildingLink.ModuleServiceTemplate.Repositories
{
    /// <summary>
    /// Manages the data access book objects.
    /// </summary>
    public class BookRepository : IBookRepository
    {
        private readonly CodeFirstDbContext _moduleServiceTemplateDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookRepository"/> class.
        /// Create new BookRepository instance.
        /// </summary>
        /// <param name="moduleServiceTemplateDbContext">ModuleServiceTemplateDbContext.</param>
        public BookRepository(CodeFirstDbContext moduleServiceTemplateDbContext)
        {
            _moduleServiceTemplateDbContext = moduleServiceTemplateDbContext;
        }

        /// <summary>
        /// Get all active books.
        /// </summary>
        /// <returns>Book query.</returns>
        public Task<List<Book>> GetAllActiveAsync()
        {
            return this._moduleServiceTemplateDbContext.Books.Where(b => b.Active).ToListAsync();
        }

        /// <summary>
        /// Get the Book.
        /// </summary>
        /// <param name="id">Book Id.</param>
        /// <returns>A Book.</returns>
        public Task<Book> GetAsync(Guid id)
        {
            return _moduleServiceTemplateDbContext.Books
                .Where(b => b.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Add new book to the database.
        /// </summary>
        /// <param name="book">Book.</param>
        /// <returns>The Book.</returns>
        public async Task<Book> CreateAsync(Book book)
        {
            await _moduleServiceTemplateDbContext.Books.AddAsync(book);
            await _moduleServiceTemplateDbContext.SaveChangesAsync();

            return book;
        }

        /// <summary>
        /// Update an existent book.
        /// </summary>
        /// <param name="book">New book status.</param>
        /// <returns>The number of affected rows in the database.</returns>
        public Task<int> UpdateAsync(Book book)
        {
            this._moduleServiceTemplateDbContext.Update(book);

            return this._moduleServiceTemplateDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Remove a book from the database.
        /// </summary>
        /// <param name="book">The Book.</param>
        /// <returns>The number of affected rows in the database.</returns>
        public Task<int> RemoveAsync(Book book)
        {
            this._moduleServiceTemplateDbContext.Books.Remove(book);

            return this._moduleServiceTemplateDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Check if there is any book in the database with the title and author specified in the parameters
        /// </summary>
        /// <param name="title">The Book Title.</param>
        /// <param name="author">The Book Author.</param>
        /// <returns>true if the book exists, false otherwise.</returns>
        public Task<bool> DoesBookExistWith(string title, string author)
        {
            return _moduleServiceTemplateDbContext.Books
                    .AnyAsync(b => b.Active
                                && b.Title.ToLower() == title.ToLower()
                                && b.Author.ToLower() == author.ToLower());
        }
    }
}
