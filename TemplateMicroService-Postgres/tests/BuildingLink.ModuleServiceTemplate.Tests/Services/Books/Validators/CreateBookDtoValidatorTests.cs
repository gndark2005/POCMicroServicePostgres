using BuildingLink.ModuleServiceTemplate.Repositories;
using BuildingLink.ModuleServiceTemplate.Repositories.Seeding;
using BuildingLink.ModuleServiceTemplate.Services.Books.DTO;
using BuildingLink.ModuleServiceTemplate.Services.Books.Validators;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BuildingLink.ModuleServiceTemplate.Tests.Services.Books.Validators
{
    [Trait("Feature", "ModuleServiceTemplate")]
    [Trait("Layer", "Services")]
    [Trait("Category", "Unit")]
    public class CreateBookDtoValidatorTests
    {
        private readonly Mock<IBookRepository> _mockBookRepository;
        private readonly CreateBookDtoValidator _validator;

        public CreateBookDtoValidatorTests()
        {
            _mockBookRepository = new Mock<IBookRepository>();
            _validator = new CreateBookDtoValidator(_mockBookRepository.Object);
        }

        [Fact]
        public async Task Validate_WhenTitleIsEmpty_ShouldReturnErrorForTitle()
        {
            // ARRANGE
            var createBook = new CreateBookDto
            {
                Title = string.Empty,
                Author = "Test"
            };

            _mockBookRepository
                .Setup(r => r.DoesBookExistWith(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            // ACT
            var result = await _validator.ValidateAsync(createBook);

            // ASSERT
            result.Errors.Should().Contain(error => error.PropertyName == nameof(createBook.Title));
        }

        [Fact]
        public async Task Validate_WhenAuthorIsEmpty_ShouldReturnErrorForAuthor()
        {
            // ARRANGE
            var createBook = new CreateBookDto
            {
                Title = "Test",
                Author = string.Empty
            };

            _mockBookRepository
                .Setup(r => r.DoesBookExistWith(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            // ACT
            var result = await _validator.ValidateAsync(createBook);

            // ASSERT
            result.Errors.Should().Contain(error => error.PropertyName == nameof(createBook.Author));
        }

        [Fact]
        public async Task Validate_WhenAuthorAndTitleIsEmpty_ShouldReturnErrorForBothProperties()
        {
            // ARRANGE
            var createBook = new CreateBookDto
            {
                Title = string.Empty,
                Author = string.Empty
            };

            _mockBookRepository
                .Setup(r => r.DoesBookExistWith(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            // ACT
            var result = await _validator.ValidateAsync(createBook);

            // ASSERT
            result.Errors.Should().Contain(error => error.PropertyName == nameof(createBook.Title));
            result.Errors.Should().Contain(error => error.PropertyName == nameof(createBook.Author));
        }

        [Fact]
        public async Task Validate_WhenAuthorAndTitleAlreadyExists_ShouldReturnBookAlreadyExistsError()
        {
            // ARRANGE
            var createBook = new CreateBookDto
            {
                Title = "Vue for dummies",
                Author = "Evan You"
            };
            var errorMessage = "There is a book with the same title and author already registered.";

            _mockBookRepository
                .Setup(r => r.DoesBookExistWith(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // ACT
            var result = await _validator.ValidateAsync(createBook);

            // ASSERT
            result.Errors.First().ErrorMessage.Should().BeEquivalentTo(errorMessage);
        }
    }
}
