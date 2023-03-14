using System;
using System.Threading.Tasks;
using BuildingLink.ModuleServiceTemplate.Consumer.Consumers;
using BuildingLink.ModuleServiceTemplate.Events;
using BuildingLink.ModuleServiceTemplate.Repositories;
using BuildingLink.ModuleServiceTemplate.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BuildingLink.ModuleServiceTemplate.Tests.Consumers
{
    public class BookStatusChangedConsumerTests
    {
        private readonly BookStatusChangedConsumer _consumer;
        private readonly Mock<ILogger<BookStatusChangedConsumer>> _mockLogger;

        public BookStatusChangedConsumerTests()
        {
            _mockLogger = new Mock<ILogger<BookStatusChangedConsumer>>();
            _mockLogger
                .Setup(l => l.IsEnabled(It.IsAny<LogLevel>()))
                .Returns(true);
            _consumer = new BookStatusChangedConsumer(_mockLogger.Object);
        }

        [Fact]
        public async Task Consume_WhenMessageArrives_ShouldPrintBookStatusChange()
        {
            // ARRANGE
            var bookId = Guid.NewGuid();
            var previousStatus = Enum.GetName(BookStatus.AvailableSoon);
            var newStatus = Enum.GetName(BookStatus.Available);
            var bookStatusChanged = new BookStatusChanged
            {
                Id = bookId,
                PreviousStatus = previousStatus ?? string.Empty,
                NewStatus = newStatus ?? string.Empty
            };
            var expectedLogMessage = $"Book Id: {bookId} changed status from {previousStatus} to {newStatus}";

            // ACT
            await _consumer.Consume(bookStatusChanged);

            // ASSERT
            _mockLogger.AssertLoggerPrinted(expectedLogMessage, LogLevel.Information, Times.Once());
        }
    }
}