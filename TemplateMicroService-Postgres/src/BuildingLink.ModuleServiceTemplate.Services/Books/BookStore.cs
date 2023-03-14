using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BuildingLink.Messaging.Publisher;
using BuildingLink.ModuleServiceTemplate.Events;
using BuildingLink.ModuleServiceTemplate.Repositories;
using BuildingLink.ModuleServiceTemplate.Services.Books.DTO;
using FluentValidation;
using BookStatus = BuildingLink.ModuleServiceTemplate.Repositories.BookStatus;

namespace BuildingLink.ModuleServiceTemplate.Services.Books
{
    /// <summary>
    /// Book store service.
    /// </summary>
    public class BookStore : IBookStore
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly IPublisherService _publisherService;
        private readonly IValidator<CreateBookDto> _createBookValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookStore"/> class.
        /// Constructor, initialize the books collection.
        /// </summary>
        /// <param name="bookRepository">IBookRepository.</param>
        /// <param name="mapper">IMapper.</param>
        /// <param name="publisherService">IPublisherService.</param>
        /// <param name="createBookValidator">IValidator<CreateBookDto>.</param>
        public BookStore(
                        IBookRepository bookRepository,
                        IMapper mapper,
                        IPublisherService publisherService,
                        IValidator<CreateBookDto> createBookValidator)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _publisherService = publisherService;
            _createBookValidator = createBookValidator;
        }

        /// <summary>
        /// Get all active books. It is just an example.
        /// </summary>
        /// <returns>Returns an IEnumerable of books.</returns>
        public async Task<IEnumerable<BookDto>> GetAllActiveAsync()
        {
            var books = await _bookRepository.GetAllActiveAsync();
            return books.Select(b => _mapper.Map<BookDto>(b));
        }

        /// <summary>
        /// Get book. It is just an example.
        /// </summary>
        /// <param name="bookId">bookId.</param>
        /// <returns>Returns a book.</returns>
        public async Task<BookDto> GetAsync(Guid bookId)
        {
            var book = await _bookRepository.GetAsync(bookId);
            return _mapper.Map<BookDto>(book);
        }

        /// <summary>
        /// Add new book to the store. It is just an example.
        /// </summary>
        /// <param name="createBookDto">Create Book object.</param>
        /// <returns>Returns the new book added to the store.</returns>
        public async Task<BookDto> AddBookAsync(CreateBookDto createBookDto)
        {
            await _createBookValidator.ValidateAndThrowAsync(createBookDto);

            var modelBook = _mapper.Map<Book>(createBookDto);

            modelBook.Status = BookStatus.AvailableSoon;
            modelBook.Active = true;

            var newBook = await _bookRepository.CreateAsync(modelBook);

            await _publisherService.PublishAsync(new BookCreated
            {
                Id = newBook.Id,
                Title = newBook.Title,
                Author = newBook.Author,
                Status = Enum.GetName(newBook.Status)
            });

            return _mapper.Map<BookDto>(newBook);
        }

        /// <summary>
        /// Change the book status from "AvailableSoon" to "Available". It is just an example.
        /// </summary>
        /// <param name="bookId">The book's Id.</param>
        /// <returns>Returns the book with the new status.</returns>
        public async Task<BookDto> ChangeToAvailableAsync(Guid bookId)
        {
            var book = await _bookRepository.GetAsync(bookId);
            if (book is null)
            {
                return await Task.FromResult<BookDto>(null);
            }

            if (book.Status != BookStatus.AvailableSoon)
            {
                throw new InvalidOperationException();
            }

            var originalStatus = book.Status;
            book.Status = BookStatus.Available;
            await _bookRepository.UpdateAsync(book);

            await _publisherService.PublishAsync(new BookStatusChanged
            {
                Id = book.Id,
                PreviousStatus = Enum.GetName(originalStatus),
                NewStatus = Enum.GetName(book.Status)
            });
            return _mapper.Map<BookDto>(book);
        }

        /// <summary>
        /// Remove a book from the store. It is just an example.
        /// </summary>
        /// <param name="bookId">The book's Id.</param>
        /// <returns>Returns 'true' if the book was removed, 'false' if the book was not found.</returns>
        public async Task<bool> RemoveAsync(Guid bookId)
        {
            var book = await _bookRepository.GetAsync(bookId);
            if (book is null)
            {
                return false;
            }

            if (book.Status == BookStatus.InUse)
            {
                throw new InvalidOperationException();
            }

            var affectedRows = await _bookRepository.RemoveAsync(book);

            await _publisherService.PublishAsync(new BookRemoved
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Status = Enum.GetName(book.Status),
                Active = book.Active
            });

            return affectedRows > 0;
        }
    }
}
