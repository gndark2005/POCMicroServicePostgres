using BuildingLink.ModuleServiceTemplate.Data;
using BuildingLink.ModuleServiceTemplate.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingLink.ModuleServiceTemplate.Repositories
{
    /// <summary>
    /// Manages the data access book objects. 
    /// </summary>
    public class BookRepository: IBookRepository
    {
        private readonly ModuleServiceTemplateDbContext _moduleServiceTemplateDbContext;

        /// <summary>
        /// Create new BookRepository instance.
        /// </summary>
        /// <param name="moduleServiceTemplateDbContext">ModuleServiceTemplateDbContext</param>
        public BookRepository(ModuleServiceTemplateDbContext moduleServiceTemplateDbContext)
        {
            this._moduleServiceTemplateDbContext = moduleServiceTemplateDbContext;
        }

        /// <summary>
        /// Get all active books query.
        /// </summary>
        /// <returns>Book query.</returns>
        public IQueryable<Book> GetAllActive()
        {
            return this._moduleServiceTemplateDbContext.Books.Where(b => b.Active);
        }

        /// <summary>
        /// Get the Book.
        /// </summary>
        /// <param name="id">Book Id.</param>
        /// <returns>A Book</returns>
        public Task<Book> GetAsync(Guid id)
        {
            return this._moduleServiceTemplateDbContext.Books
                .Where(b => b.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Add new book to the database.
        /// </summary>
        /// <param name="book">Book</param>
        /// <returns>The Book.</returns>
        public async Task<Book> CreateAsync(Book book)
        {
            await this._moduleServiceTemplateDbContext.Books.AddAsync(book);
            await this._moduleServiceTemplateDbContext.SaveChangesAsync();

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
    }
}
