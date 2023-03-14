using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using BuildingLink.ModuleServiceTemplate.Repositories.Seeding;
using BuildingLink.ModuleServiceTemplate.Services.Books.DTO;
using FluentAssertions;
using Xunit;
using Book = BuildingLink.ModuleServiceTemplate.Repositories.Book;

namespace BuildingLink.ModuleServiceTemplate.Tests.Controllers
{
    [Trait("Feature", "ModuleServiceTemplate")]
    [Trait("Layer", "Controllers")]
    [Trait("Category", "Integration")]
    public class BookControllerIntegrationTests : IntegrationTestsCodeFirstBase
    {
        [Fact]
        public async Task GetAll_WhenBooksExist_ShouldReturnOnlyActiveBooks()
        {
            // ARRANGE
            var activeBook = BookBuilder.Builder()
                .WithTitle("Active book")
                .WithAuthor("Active author")
                .Build();
            var noActiveBook = BookBuilder.Builder()
                .WithTitle("Inactive book")
                .WithAuthor("Active author")
                .WithActive(false)
                .Build();

            await DbContext.Books.AddRangeAsync(new Book[] { activeBook, noActiveBook });
            await DbContext.SaveChangesAsync();

            // ACT
            var response = await Client.GetAsync("book");
            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IEnumerable<BookDto>>(jsonResult, JsonSerializerOptions);

            // ASSERT
            response.IsSuccessStatusCode.Should().BeTrue();
            result.Should().BeEquivalentTo(new BookDto[] { Mapper.Map<BookDto>(activeBook) });
        }

        [Fact]
        public async Task GetBook_WhenBookDoesntExist_ShouldReturn404HTTPStatus()
        {
            // ACT
            var response = await Client.GetAsync($"book/{Guid.NewGuid()}");

            // ASSERT
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetBook_WhenBookExist_ShouldReturnTheBook()
        {
            // ARRANGE
            var activeBook = BookBuilder.Builder()
                .WithTitle("Active book")
                .WithAuthor("Active author")
                .Build();

            await DbContext.Books.AddRangeAsync(new Book[] { activeBook });
            await DbContext.SaveChangesAsync();

            // ACT
            var response = await Client.GetAsync($"book/{activeBook.Id}");
            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<BookDto>(jsonResult, JsonSerializerOptions);

            // ASSERT
            response.IsSuccessStatusCode.Should().BeTrue();
            result.Should().BeEquivalentTo(Mapper.Map<BookDto>(activeBook));
        }
    }
}
