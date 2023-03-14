using System;
using BuildingLink.ModuleServiceTemplate.Repositories;

namespace BuildingLink.ModuleServiceTemplate.Repositories.Seeding
{
    /// <summary>
    /// Represents a Book to be created.
    /// </summary>
    public class BookBuilder
    {
        private Guid _id;

        private string _title;

        private string _author;

        private BookStatus _status;

        private bool _active;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookBuilder"/> class.
        /// Create a new BookBuilder instance.
        /// </summary>
        public BookBuilder()
        {
            _active = true;
        }

        /// <summary>
        /// Create a new BookBuilder instance.
        /// </summary>
        /// <returns>BookBuilder object.</returns>
        public static BookBuilder Builder()
        {
            return new BookBuilder();
        }

        /// <summary>
        /// Build a Book instance.
        /// </summary>
        /// <returns>Book object.</returns>
        public Book Build()
        {
            return new Book()
            {
                Id = this._id,
                Title = this._title,
                Author = this._author,
                Status = this._status,
                Active = this._active
            };
        }

        /// <summary>
        /// Set the "id" book builder field.
        /// </summary>
        /// <param name="id">Book Id.</param>
        /// <returns>BookBuilder object.</returns>
        public BookBuilder WithId(Guid id)
        {
            this._id = id;

            return this;
        }

        /// <summary>
        /// Set the "title" book builder field.
        /// </summary>
        /// <param name="title">Book title.</param>
        /// <returns>BookBuilder object.</returns>
        public BookBuilder WithTitle(string title)
        {
            this._title = title;

            return this;
        }

        /// <summary>
        /// Set the "author" book builder field.
        /// </summary>
        /// <param name="author">Book Author.</param>
        /// <returns>BookBuilder object.</returns>
        public BookBuilder WithAuthor(string author)
        {
            this._author = author;

            return this;
        }

        /// <summary>
        /// Set the id book builder field.
        /// </summary>
        /// <param name="status">Book status.</param>
        /// <returns>BookBuilder object.</returns>
        public BookBuilder WithStatus(BookStatus status)
        {
            this._status = status;

            return this;
        }

        /// <summary>
        /// Set the "active" book builder field.
        /// </summary>
        /// <param name="active">Book active.</param>
        /// <returns>BookBuilder object.</returns>
        public BookBuilder WithActive(bool active)
        {
            this._active = active;

            return this;
        }
    }
}
