using System;
using System.Threading.Tasks;
using BuildingLink.ModuleServiceTemplate.Consumer.Extensions;
using BuildingLink.ModuleServiceTemplate.Consumers;
using BuildingLink.ModuleServiceTemplate.Events;
using BuildingLink.ModuleServiceTemplate.Repositories;
using BuildingLink.ModuleServiceTemplate.Repositories.Seeding;
using BuildingLink.ModuleServiceTemplate.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BuildingLink.ModuleServiceTemplate.Tests.Consumers
{
    public class BookCreatedConsumerTests
    {
        private readonly BookCreatedConsumer _consumer;
        private readonly Mock<ILogger<BookCreatedConsumer>> _mockLogger;

        public BookCreatedConsumerTests()
        {
            _mockLogger = new Mock<ILogger<BookCreatedConsumer>>();
            _mockLogger
                .Setup(l => l.IsEnabled(It.IsAny<LogLevel>()))
                .Returns(true);
            _consumer = new BookCreatedConsumer(_mockLogger.Object);
        }

        [Fact]
        public async Task Consume_WhenBookIdExists_ShouldPrintBookInformation()
        {
            // ARRANGE
            var bookId = Guid.NewGuid();
            var bookCreated = new BookCreated
            {
                Id = bookId,
                Title = "Vue for Dummies",
                Author = "Even You",
                Status = "Available"
            };
            var loggedInformation = $"This is the book information Id: {bookId}, Title: {bookCreated.Title}, Author: {bookCreated.Author}, Status: {bookCreated.Status}";

            // ACT
            await _consumer.Consume(bookCreated);

            // ASSERT
            _mockLogger.AssertLoggerPrinted(loggedInformation, LogLevel.Information, Times.Once());
        }
    }
}