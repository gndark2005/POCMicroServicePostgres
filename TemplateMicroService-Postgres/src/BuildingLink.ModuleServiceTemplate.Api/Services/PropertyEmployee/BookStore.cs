using BookStatus = BuildingLink.ModuleServiceTemplate.Models.BookStatus;
using BookModel = BuildingLink.ModuleServiceTemplate.Models.Book;
using BuildingLink.ModuleServiceTemplate.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingLink.ModuleServiceTemplate.Repositories;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace BuildingLink.ModuleServiceTemplate.Services.PropertyEmployee
{
    /// <summary>
    /// Book store service
    /// </summary>
    public class BookStore
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor, initialize the books collection
        /// </summary>
        public BookStore(IBookRepository bookRepository, IMapper mapper)
        {
            this._bookRepository = bookRepository;
            this._mapper = mapper;
        }

        /// <summary>
        /// Get all active books. It is just an example.
        /// </summary>
        /// <returns>Returns an IEnumerable of books.</returns>
        public async Task<IEnumerable<Book>> GetAllActiveAsync()
        {
            var books = await this._bookRepository.GetAllActive().ToListAsync();
            return books.Select(b => this._mapper.Map<Book>(b));
        }

        /// <summary>
        /// Get book. It is just an example.
        /// </summary>
        /// <returns>Returns a book.</returns>
        public async Task<Book> GetAsync(Guid bookId)
        {
            var book = await this._bookRepository.GetAsync(bookId);
            return this._mapper.Map<Book>(book);
        }

        /// <summary>
        /// Add new book to the store. It is just an example.
        /// </summary>
        /// <param name="book">New book object.</param>
        /// <returns>Returns the new book added to the store.</returns>
        public async Task<Book> AddBookAsync(Book book)
        {
            var modelBook = this._mapper.Map<BookModel>(book);

            modelBook.Status = BookStatus.AvailableSoon;
            modelBook.Active = true;

            var newBook = await this._bookRepository.CreateAsync(modelBook);

            return this._mapper.Map<Book>(newBook);
        }

        /// <summary>
        /// Change the book status from "AvailableSoon" to "Available". It is just an example.
        /// </summary>
        /// <param name="bookId">The book's Id.</param>
        /// <returns>Returns the book with the new status.</returns>
        public async Task<Book> ChangeToAvailableAsync(Guid bookId)
        {
            var book = await this._bookRepository.GetAsync(bookId);
            if (book is null)
                return null;

            if (book.Status != BookStatus.AvailableSoon)
                throw new InvalidOperationException();

            book.Status = BookStatus.Available;
            await this._bookRepository.UpdateAsync(book);

            return this._mapper.Map<Book>(book);
        }

        /// <summary>
        /// Remove a book from the store. It is just an example.
        /// </summary>
        /// <param name="bookId">The book's Id.</param>
        /// <returns>Returns 'true' if the book was removed, 'false' if the book was not found.</returns>
        public async Task<bool> RemoveAsync(Guid bookId)
        {
            var book = await this._bookRepository.GetAsync(bookId);
            if (book is null)
                return false;

            if (book.Status == BookStatus.InUse)
                throw new InvalidOperationException();

            var affectedRows = await this._bookRepository.RemoveAsync(book);
            return affectedRows > 0;
        }
    }
}
