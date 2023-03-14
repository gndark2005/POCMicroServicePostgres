using BuildingLink.ModuleServiceTemplate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingLink.ModuleServiceTemplate.Repositories
{
    /// <summary>
    /// Manages the data access book objects. 
    /// </summary>
    public interface IBookRepository
    {
        /// <summary>
        /// Get all active books query.
        /// </summary>
        /// <returns>Book query.</returns>
        IQueryable<Book> GetAllActive();

        /// <summary>
        /// Get the Book.
        /// </summary>
        /// <param name="id">Book Id.</param>
        /// <returns>A Book</returns>
        Task<Book> GetAsync(Guid id);

        /// <summary>
        /// Add new book to the database.
        /// </summary>
        /// <param name="book">Book</param>
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
    }
}
