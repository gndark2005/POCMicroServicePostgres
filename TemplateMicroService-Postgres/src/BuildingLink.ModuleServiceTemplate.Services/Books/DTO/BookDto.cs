using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingLink.ModuleServiceTemplate.Repositories;

namespace BuildingLink.ModuleServiceTemplate.Services.Books.DTO
{
    /// <summary>
    /// Book view model is using to send to the client-side in some of our Endpoints.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class BookDto
    {
        /// <summary>
        /// Gets or sets book Id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets book Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets book Author.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets book Status.
        /// </summary>
        public BookStatus Status { get; set; }

        /// <summary>
        /// Get hash code.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        ///  Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is BookDto))
            {
                return false;
            }

            var compareObj = obj as BookDto;
            return Id == compareObj.Id
                && Title == compareObj.Title
                && Author == compareObj.Author
                && Status == compareObj.Status;
        }
    }
}
