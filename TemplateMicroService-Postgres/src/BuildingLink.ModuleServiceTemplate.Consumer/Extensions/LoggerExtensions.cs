namespace BuildingLink.ModuleServiceTemplate.Consumer.Extensions
{
    /// <summary>
    /// Partial class extends ILogger.
    /// </summary>
    public static partial class LoggerExtensions
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "New message arrived with {messageId} at {StartTime}")]
        public static partial void MessageRecieved(this ILogger logger, string messageId, DateTime StartTime);

        [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "The message with id {messageId} was successfully consumed, total elapsed miliseconds: {elapsedMiliseconds}")]
        public static partial void MessageConsumed(this ILogger logger,
            Guid messageId,
            double elapsedMiliseconds);

        [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "This is the book information Id: {id}, Title: {title}, Author: {author}, Status: {status}")]
        public static partial void BookCreatedInformation(this ILogger logger, Guid id, string title, string author, string status);

        [LoggerMessage(EventId = 4, Level = LogLevel.Warning, Message = "Book Id: {bookId} was not found in the database.")]
        public static partial void BookNotFound(this ILogger logger, Guid bookId);
        [LoggerMessage(EventId = 5, Level = LogLevel.Information, Message = "Book Id: {bookId} changed status from {previousStatus} to {newStatus}.")]
        public static partial void BookStatusUpdated(this ILogger logger, Guid bookId, string previousStatus, string newStatus);
        [LoggerMessage(EventId = 6, Level = LogLevel.Information, Message = "Book Id: {bookId} removed. Title: {title}, Author: {author}, Status: {status}")]
        public static partial void BookRemoved(this ILogger logger, Guid bookId, string title, string author, string status);

        /// <summary>
        /// Add a property or properties (as ValueTuple) to the logging context.
        /// ** Surround (Key, Value) pairs in parens.
        /// ** Using is required
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="properties"></param>
        /// <returns>ILogger.BeginScope</returns>
        public static IDisposable AddScopeProperties(
            this ILogger logger,
            params ValueTuple<string, object>[] properties)
        {
            var dictionary = properties.ToDictionary(p => p.Item1, p => p.Item2);
            return logger.BeginScope(dictionary);
        }
    }
}