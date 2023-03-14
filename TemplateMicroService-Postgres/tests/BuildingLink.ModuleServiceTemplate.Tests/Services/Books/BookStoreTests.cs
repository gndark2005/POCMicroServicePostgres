using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BuildingLink.Messaging.Publisher;
using BuildingLink.ModuleServiceTemplate.Events;
using BuildingLink.ModuleServiceTemplate.Repositories;
using BuildingLink.ModuleServiceTemplate.Repositories.Seeding;
using BuildingLink.ModuleServiceTemplate.Services.Books;
using BuildingLink.ModuleServiceTemplate.Services.Books.DTO;
using BuildingLink.ModuleServiceTemplate.Services.Books.Mapping;
using BuildingLink.ModuleServiceTemplate.Tests.Helpers;
using FluentAssertions;
using FluentValidation;
using Moq;
using Xunit;

namespace BuildingLink.ModuleServiceTemplate.Tests.Services.Books
{
    [Trait("Feature", "ModuleServiceTemplate")]
    [Trait("Layer", "Services")]
    [Trait("Category", "Unit")]
    public class BookStoreTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IBookRepository> _mockBookRepository;
        private readonly Mock<IPublisherService> _mockPublisherService;
        private readonly Mock<IValidator<CreateBookDto>> _mockCreateBookValidator;
        private readonly BookStore _bookStore;

        public BookStoreTests()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new BookMapping());
            });
            this._mapper = mapperConfiguration.CreateMapper();

            this._mockBookRepository = new Mock<IBookRepository>();
            _mockPublisherService = new Mock<IPublisherService>();
            _mockCreateBookValidator = new Mock<IValidator<CreateBookDto>>();
            this._bookStore = new BookStore(this._mockBookRepository.Object, this._mapper, _mockPublisherService.Object, _mockCreateBookValidator.Object);
        }

        [Fact]
        public async Task GetAllActiveAsync_ShouldReturnAllTheBooks()
        {
            // ARRANGE
            var book1 = BookBuilder.Builder()
                .WithTitle("Test")
                .WithAuthor("Test author")
                .WithStatus(BookStatus.Available)
                .Build();
            var book2 = BookBuilder.Builder()
                .WithTitle("Test 2")
                .WithAuthor("Test author 2")
                .WithStatus(BookStatus.Available)
                .Build();

            var expectedBooks = new List<Book> { book1, book2 };
            _mockBookRepository
                .Setup(r => r.GetAllActiveAsync())
                .ReturnsAsync(expectedBooks);

            // ACT
            var activeBooks = await _bookStore.GetAllActiveAsync();

            // ASSERT
            expectedBooks.Select(b => this._mapper.Map<BookDto>(b))
                .Should().Equal(activeBooks);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnTheRequestedBook()
        {
            // ARRANGE
            var expectedBook = BookBuilder.Builder().WithId(Guid.NewGuid())
                    .WithStatus(BookStatus.Available)
                    .Build();
            var expectedBookDto = _mapper.Map<BookDto>(expectedBook);
            this._mockBookRepository.Setup(repository => repository.GetAsync(expectedBook.Id))
                .ReturnsAsync(expectedBook);

            // ACT
            var book = await this._bookStore.GetAsync(expectedBook.Id);

            // ASSERT
            expectedBookDto.Should().BeEquivalentTo(book);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNullWhenCantFindBook()
        {
            // ARRANGE
            var bookId = Guid.NewGuid();
            this._mockBookRepository.Setup(repository => repository.GetAsync(bookId))
                .ReturnsAsync(default(Book));

            // ACT
            var book = await this._bookStore.GetAsync(bookId);

            // ASSERT
            Assert.Null(book);
        }

        [Fact]
        public async Task ChangeToAvailableAsync_ShouldReturnNullWhenCantFindBook()
        {
            // ARRANGE
            var bookId = Guid.NewGuid();
            this._mockBookRepository.Setup(repository => repository.GetAsync(bookId))
                .ReturnsAsync(default(Book));

            // ACT
            var book = await this._bookStore.ChangeToAvailableAsync(bookId);

            // ASSERT
            Assert.Null(book);
        }

        [Fact]
        public async Task ChangeToAvailableAsync_ShouldThrowInvalidOperationExceptionWhenTheStatusIsntAvailableSoon()
        {
            // ARRANGE
            var bookId = Guid.NewGuid();
            this._mockBookRepository.Setup(repository => repository.GetAsync(bookId))
                .ReturnsAsync(
                    BookBuilder.Builder().WithId(bookId)
                    .WithStatus(BookStatus.Available)
                    .Build());

            // ACT
            // ASSERT
            await Assert.ThrowsAsync<InvalidOperationException>(() => this._bookStore.ChangeToAvailableAsync(bookId));
        }

        [Fact]
        public async Task ChangeToAvailableAsync_ShouldChangeTheStatusToAvailableFromAvailableSoon()
        {
            // ARRANGE
            var book = BookBuilder.Builder().WithId(Guid.NewGuid())
                    .WithStatus(BookStatus.AvailableSoon)
                    .Build();
            this._mockBookRepository.Setup(repository => repository.GetAsync(book.Id))
                .ReturnsAsync(book);
            this._mockBookRepository.Setup(repository => repository.UpdateAsync(book))
                .ReturnsAsync(1);

            // ACT
            var bookResult = await this._bookStore.ChangeToAvailableAsync(book.Id);

            // ASSERT
            Assert.Equal(BookStatus.Available, bookResult.Status);
        }

        [Fact]
        public async Task RemoveAsync_ShouldReturnFalseIfTheBookDoesntExist()
        {
            // ARRANGE
            var bookId = Guid.NewGuid();
            this._mockBookRepository.Setup(repository => repository.GetAsync(bookId))
                .ReturnsAsync(default(Book));

            // ACT
            var success = await this._bookStore.RemoveAsync(bookId);

            // ASSERT
            Assert.False(success);
        }

        [Fact]
        public async Task RemoveAsync_ShouldReturnTrueIfTheBookWasRemoved()
        {
            // ARRANGE
            var book = BookBuilder.Builder().WithId(Guid.NewGuid())
                    .WithStatus(BookStatus.AvailableSoon)
                    .Build();
            this._mockBookRepository.Setup(repository => repository.GetAsync(book.Id))
                .ReturnsAsync(book);
            this._mockBookRepository.Setup(repository => repository.RemoveAsync(book))
                .ReturnsAsync(1);

            // ACT
            var success = await this._bookStore.RemoveAsync(book.Id);

            // ASSERT
            Assert.True(success);
        }

        [Fact]
        public async Task RemoveAsync_ShouldThrowInvalidOperationExceptionIfTheBookIsInUnse()
        {
            // ARRANGE
            var bookId = Guid.NewGuid();
            this._mockBookRepository.Setup(repository => repository.GetAsync(bookId))
                .ReturnsAsync(
                    BookBuilder.Builder().WithId(bookId)
                    .WithStatus(BookStatus.InUse)
                    .Build());

            // ACT
            // ASSERT
            await Assert.ThrowsAsync<InvalidOperationException>(() => this._bookStore.RemoveAsync(bookId));
        }

        [Fact]
        public async Task AddBookAsync_WhenCreateBookIsOk_ShouldCreateAndReturnBookWithId()
        {
            // ARRANGE
            var createBookDto = new CreateBookDto
            {
                Title = "Vue for Dummies",
                Author = "Evan You"
            };
            _mockBookRepository
                .Setup(repository => repository.CreateAsync(It.IsAny<Book>()))
                .ReturnsAsync((Book methodInput) =>
                {
                    return BookBuilder.Builder()
                    .WithId(Guid.NewGuid())
                    .WithTitle(methodInput.Title)
                    .WithAuthor(methodInput.Author)
                    .WithStatus(methodInput.Status)
                    .WithActive(methodInput.Active)
                    .Build();
                });

            // ACT
            var returnedBook = await _bookStore.AddBookAsync(createBookDto);

            // ASSERT
            Assert.NotNull(returnedBook);
            returnedBook.Id.Should().NotBe(Guid.Empty);
            returnedBook.Title.Should().Be(createBookDto.Title);
            returnedBook.Author.Should().Be(createBookDto.Author);
            returnedBook.Status.Should().Be(BookStatus.AvailableSoon);
        }

        [Fact]
        public async Task AddBookAsync_WhenCreateBookDtoIsOk_ShouldCreateBookAndPublishAllPropertiesToMessagingService()
        {
            // ARRANGE
            var createBookDto = new CreateBookDto
            {
                Title = "Demo title",
                Author = "Demo author"
            };
            var createdBookId = Guid.NewGuid();
            var expectedPublishedEvent = new BookCreated
            {
                Id = createdBookId,
                Title = createBookDto.Title,
                Author = createBookDto.Author,
                Status = Enum.GetName(BookStatus.AvailableSoon)
            };
            _mockBookRepository
                .Setup(repository => repository.CreateAsync(It.IsAny<Book>()))
                .ReturnsAsync((Book methodInput) =>
                {
                    return BookBuilder.Builder()
                    .WithId(createdBookId)
                    .WithTitle(methodInput.Title)
                    .WithAuthor(methodInput.Author)
                    .WithStatus(methodInput.Status)
                    .Build();
                });

            // ACT
            var returnedBook = await _bookStore.AddBookAsync(createBookDto);

            // ASSERT
            Assert.NotNull(returnedBook);
            _mockPublisherService.Verify(
                    publishService => publishService
                                        .PublishAsync(It.Is<BookCreated>(bc => bc.Id == expectedPublishedEvent.Id
                                                                            && bc.Title == expectedPublishedEvent.Title
                                                                            && bc.Author == expectedPublishedEvent.Author
                                                                            && bc.Status == expectedPublishedEvent.Status)),
                    Times.Once());
        }
    }
}
