using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BuildingLink.ModuleServiceTemplate.Repositories
{
    /// <summary>
    /// Manages the data access book objects.
    /// </summary>
    public interface IBookRepository
    {
        /// <summary>
        /// Get all active books.
        /// </summary>
        /// <returns>Book query.</returns>
        Task<List<Book>> GetAllActiveAsync();

        /// <summary>
        /// Get the Book.
        /// </summary>
        /// <param name="id">Book Id.</param>
        /// <returns>A Book.</returns>
        Task<Book> GetAsync(Guid id);

        /// <summary>
        /// Add new book to the database.
        /// </summary>
        /// <param name="book">Book.</param>
        /// <returns>The Book.</returns>
        Task<Book> CreateAsync(Book book);

        /// <summary>
        /// Update an existent book.
        /// </summary>
        /// <param name="book">New book status.</param>
        /// <returns>The number of affected rows in the database.</returns>
        Task<int> UpdateAsync(Book book);

        /// <summary>
        /// Remove a book from the database.
        /// </summary>
        /// <param name="book">The Book.</param>
        /// <returns>The number of affected rows in the database.</returns>
        Task<int> RemoveAsync(Book book);

        /// <summary>
        /// Check if there is any book in the database with the title and author specified in the parameters
        /// </summary>
        /// <param name="title">The Book Title.</param>
        /// <param name="author">The Book Author.</param>
        /// <returns>true if the book exists, false otherwise.</returns>
        Task<bool> DoesBookExistWith(string title, string author);
    }
}
