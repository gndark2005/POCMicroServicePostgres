using BuildingLink.ModuleServiceTemplate.Repositories;
using BuildingLink.ModuleServiceTemplate.Services.Books.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingLink.ModuleServiceTemplate.Services.Books
{
    public interface IBookStore
    {
        /// <summary>
        /// Get all active books. It is just an example.
        /// </summary>
        /// <returns>Returns an IEnumerable of books.</returns>
        Task<IEnumerable<BookDto>> GetAllActiveAsync();

        /// <summary>
        /// Get book. It is just an example.
        /// </summary>
        /// <param name="bookId">bookId.</param>
        /// <returns>Returns a book.</returns>
        Task<BookDto> GetAsync(Guid bookId);

        /// <summary>
        /// Add new book to the store. It is just an example.
        /// </summary>
        /// <param name="createBookDto">Create Book object.</param>
        /// <returns>Returns the new book added to the store.</returns>
        Task<BookDto> AddBookAsync(CreateBookDto createBookDto);

        /// <summary>
        /// Change the book status from "AvailableSoon" to "Available". It is just an example.
        /// </summary>
        /// <param name="bookId">The book's Id.</param>
        /// <returns>Returns the book with the new status.</returns>
        Task<BookDto> ChangeToAvailableAsync(Guid bookId);

        /// <summary>
        /// Remove a book from the store. It is just an example.
        /// </summary>
        /// <param name="bookId">The book's Id.</param>
        /// <returns>Returns 'true' if the book was removed, 'false' if the book was not found.</returns>
        Task<bool> RemoveAsync(Guid bookId);
    }
}
