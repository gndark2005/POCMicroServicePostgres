using BuildingLink.ModuleServiceTemplate.Consumer.Consumers;
using BuildingLink.ModuleServiceTemplate.Consumer.Extensions;
using BuildingLink.ModuleServiceTemplate.Consumers;
using BuildingLink.ModuleServiceTemplate.Events;
using BuildingLink.ModuleServiceTemplate.Repositories;
using BuildingLink.ModuleServiceTemplate.Repositories.Seeding;
using BuildingLink.ModuleServiceTemplate.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BuildingLink.ModuleServiceTemplate.Tests.Consumers
{
    public class BookRemovedConsumerTests
    {
        private readonly BookRemovedConsumer _consumer;
        private readonly Mock<ILogger<BookRemovedConsumer>> _mockLogger;

        public BookRemovedConsumerTests()
        {
            _mockLogger = new Mock<ILogger<BookRemovedConsumer>>();
            _mockLogger
                .Setup(l => l.IsEnabled(It.IsAny<LogLevel>()))
                .Returns(true);
            _consumer = new BookRemovedConsumer(_mockLogger.Object);
        }

        [Fact]
        public async Task Consume_WhenMessageArrives_ShouldPrintBookStatusChange()
        {
            // ARRANGE
            var bookId = Guid.NewGuid();
            var bookTitle = "Vue for Dummies";
            var bookAuthor = "Evan you";
            var bookStatus = Enum.GetName(BookStatus.Available) ?? string.Empty;
            var bookStatusChanged = new BookRemoved
            {
                Id = bookId,
                Title = bookTitle,
                Author = bookAuthor,
                Status = bookStatus,
                Active = true
            };
            var expectedLogMessage = $"Book Id: {bookId} removed. Title: {bookTitle}, Author: {bookAuthor}, Status: {bookStatus}";

            // ACT
            await _consumer.Consume(bookStatusChanged);

            // ASSERT
            _mockLogger.AssertLoggerPrinted(expectedLogMessage, LogLevel.Information, Times.Once());
        }
    }
}