using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingLink.ModuleServiceTemplate.Services.Books.DTO
{
    /// <summary>
    /// Create book DTO is used to recieve the body from the client-side in some of our Endpoints.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CreateBookDto
    {
        /// <summary>
        /// Gets or sets book Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets book Author.
        /// </summary>
        public string Author { get; set; }
    }
}
