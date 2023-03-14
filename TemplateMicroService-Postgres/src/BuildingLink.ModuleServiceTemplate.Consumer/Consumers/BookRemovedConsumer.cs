using BuildingLink.Messaging.MassTransit;
using BuildingLink.ModuleServiceTemplate.Consumer.Extensions;
using BuildingLink.ModuleServiceTemplate.Events;
using System.Diagnostics;

namespace BuildingLink.ModuleServiceTemplate.Consumer.Consumers
{
    public class BookRemovedConsumer : Consumer<BookRemoved>
    {
        private readonly ILogger<BookRemovedConsumer> _logger;

        public BookRemovedConsumer(ILogger<BookRemovedConsumer> logger)
        {
            _logger = logger;
        }
        public override Task Consume(BookRemoved bookEvent)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            _logger.MessageRecieved(bookEvent.Id.ToString(), DateTime.Now);

            _logger.BookRemoved(bookEvent.Id, bookEvent.Title, bookEvent.Author, bookEvent.Status);
            stopWatch.Stop();

            _logger.MessageConsumed(bookEvent.Id, stopWatch.Elapsed.TotalMilliseconds);

            return Task.CompletedTask;
        }
    }
}
