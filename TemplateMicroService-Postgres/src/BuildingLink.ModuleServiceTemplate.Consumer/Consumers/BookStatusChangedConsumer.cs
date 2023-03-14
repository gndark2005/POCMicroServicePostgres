using BuildingLink.Messaging.MassTransit;
using BuildingLink.ModuleServiceTemplate.Consumer.Extensions;
using BuildingLink.ModuleServiceTemplate.Events;
using System.Diagnostics;

namespace BuildingLink.ModuleServiceTemplate.Consumer.Consumers
{
    public class BookStatusChangedConsumer : Consumer<BookStatusChanged>
    {
        private readonly ILogger<BookStatusChangedConsumer> _logger;

        public BookStatusChangedConsumer(ILogger<BookStatusChangedConsumer> logger)
        {
            _logger = logger;
        }
        public override Task Consume(BookStatusChanged bookEvent)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            _logger.MessageRecieved(bookEvent.Id.ToString(), DateTime.Now);

            _logger.BookStatusUpdated(bookEvent.Id, bookEvent.PreviousStatus, bookEvent.NewStatus);
            stopWatch.Stop();

            _logger.MessageConsumed(bookEvent.Id, stopWatch.Elapsed.TotalMilliseconds);
            return Task.CompletedTask;
        }
    }
}
