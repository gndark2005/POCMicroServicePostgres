using BuildingLink.Messaging.MassTransit;
using BuildingLink.ModuleServiceTemplate.Consumer.Extensions;
using BuildingLink.ModuleServiceTemplate.Events;
using System.Diagnostics;

namespace BuildingLink.ModuleServiceTemplate.Consumers
{
    public class BookCreatedConsumer : Consumer<BookCreated>
    {
        private readonly ILogger<BookCreatedConsumer> _logger;

        public BookCreatedConsumer(ILogger<BookCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public override Task Consume(BookCreated bookEvent)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            using (_logger.AddScopeProperties(("Book.Id", bookEvent.Id)))
            {
                _logger.MessageRecieved(bookEvent.Id.ToString(), DateTime.Now);

                _logger.BookCreatedInformation(bookEvent.Id, bookEvent.Title, bookEvent.Author, bookEvent.Status);

                stopWatch.Stop();

                _logger.MessageConsumed(bookEvent.Id, stopWatch.Elapsed.TotalMilliseconds);
            }

            return Task.CompletedTask;
        }
    }
}
