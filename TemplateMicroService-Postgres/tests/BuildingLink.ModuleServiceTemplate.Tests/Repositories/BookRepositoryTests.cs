using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BuildingLink.ModuleServiceTemplate.Repositories;
using BuildingLink.ModuleServiceTemplate.Repositories.Seeding;
using BuildingLink.ModuleServiceTemplate.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace BuildingLink.ModuleServiceTemplate.Tests.Repositories
{
    [Trait("Feature", "ModuleServiceTemplate")]
    [Trait("Layer", "Repositories")]
    [Trait("Category", "Unit")]
    public class BookRepositoryTests
    {
        [Fact]
        public async Task GetAllActive_JustReturnActiveBooks()
        {
            // ARRANGE
            var book = BookBuilder.Builder()
                .WithTitle("Test")
                .WithAuthor("Test author")
                .WithStatus(BookStatus.Available)
                .WithActive(false)
                .Build();

            using (var dbContext = await SeededModuleServiceTemplateDbContext
                .BuildModuleServiceTemplateDbContextAsync(new Book[] { book }))
            {
                var repository = new BookRepository(dbContext);

                // ACT
                var activeBooks = await repository.GetAllActiveAsync();
                var expectedActiveBooks = new List<Book>();

                // ASSERT
                expectedActiveBooks.Should().Equal(activeBooks);
            }
        }

        [Fact]
        public async Task GetAsync_JustReturnNullIfTheIdDoesntMatchWithAnyBook()
        {
            // ARRANGE
            var expextedBook = BookBuilder.Builder()
                .WithTitle("Test")
                .WithAuthor("Test author")
                .WithStatus(BookStatus.Available)
                .Build();

            using (var dbContext = await SeededModuleServiceTemplateDbContext
                .BuildModuleServiceTemplateDbContextAsync(new Book[] { expextedBook }))
            {
                var repository = new BookRepository(dbContext);

                // ACT
                var book = await repository.GetAsync(Guid.NewGuid());

                // ASSERT
                Assert.Null(book);
            }
        }

        [Fact]
        public async Task GetAsync_JustReturnTheBook()
        {
            // ARRANGE
            var expextedBook = BookBuilder.Builder()
                .WithTitle("Test")
                .WithAuthor("Test author")
                .WithStatus(BookStatus.Available)
                .Build();

            using (var dbContext = await SeededModuleServiceTemplateDbContext
                .BuildModuleServiceTemplateDbContextAsync(new Book[] { expextedBook }))
            {
                var repository = new BookRepository(dbContext);

                // ACT
                var book = await repository.GetAsync(expextedBook.Id);

                // ASSERT
                Assert.Equal(expextedBook, book);
            }
        }

        [Fact]
        public async Task CreateAsync_SaveBook()
        {
            // ARRANGE
            var book = BookBuilder.Builder()
                .WithTitle("Test")
                .WithAuthor("Test author")
                .WithStatus(BookStatus.Available)
                .Build();

            using (var dbContext = SeededModuleServiceTemplateDbContext
                .BuildModuleServiceTemplateDbContext())
            {
                var repository = new BookRepository(dbContext);

                // ACT
                await repository.CreateAsync(book);

                // ASSERT
                dbContext.Set<Book>().Should().Equal(new List<Book> { book });
            }
        }

        [Fact]
        public async Task UpdateAsync_UpdateTheBookStateInTheDatabase()
        {
            // ARRANGE
            var book = BookBuilder.Builder()
                .WithTitle("Test")
                .WithAuthor("Test author")
                .WithStatus(BookStatus.Available)
                .Build();

            using (var dbContext = await SeededModuleServiceTemplateDbContext
                .BuildModuleServiceTemplateDbContextAsync(new Book[] { book }))
            {
                var repository = new BookRepository(dbContext);

                // ACT
                book.Title = "Test 1";
                book.Author = "Test author 1";
                book.Status = BookStatus.AvailableSoon;

                var affectedRows = await repository.UpdateAsync(book);

                // ASSERT
                Assert.Equal(1, affectedRows);
                dbContext.Set<Book>().Should().Equal(new Book[] { book });
            }
        }

        [Fact]
        public async Task RemoveAsync_RemoveTheBookFromTheDatabase()
        {
            // ARRANGE
            var book = BookBuilder.Builder()
                .WithTitle("Test")
                .WithAuthor("Test author")
                .WithStatus(BookStatus.Available)
                .Build();

            using (var dbContext = await SeededModuleServiceTemplateDbContext
                .BuildModuleServiceTemplateDbContextAsync(new Book[] { book }))
            {
                var repository = new BookRepository(dbContext);

                // ACT
                var affectedRows = await repository.RemoveAsync(book);

                // ASSERT
                Assert.Equal(1, affectedRows);
                dbContext.Set<Book>().Should().Equal(Array.Empty<Book>());
            }
        }

        [Fact]
        public async Task DoesBookExistWith_WhenActiveBooksFromEvanYouExists_ShouldReturnTrue()
        {
            // ARRANGE
            var authorName = "Evan You";
            var activeBookTitle = "Vue for dummies";
            var inactiveBookTitle = "VueJs";
            var bookVue = BookBuilder.Builder()
                .WithTitle(activeBookTitle)
                .WithAuthor(authorName)
                .WithStatus(BookStatus.Available)
                .WithActive(true)
                .Build();

            var anotherVueBook = BookBuilder.Builder()
                .WithTitle(inactiveBookTitle)
                .WithAuthor(authorName)
                .WithStatus(BookStatus.Available)
                .WithActive(false)
                .Build();

            using (var dbContext = await SeededModuleServiceTemplateDbContext
                .BuildModuleServiceTemplateDbContextAsync(new Book[] { bookVue, anotherVueBook }))
            {
                var repository = new BookRepository(dbContext);

                // ACT
                var bookExists = await repository.DoesBookExistWith(activeBookTitle, authorName);

                // ASSERT
                bookExists.Should().BeTrue();
            }
        }
    }
}
