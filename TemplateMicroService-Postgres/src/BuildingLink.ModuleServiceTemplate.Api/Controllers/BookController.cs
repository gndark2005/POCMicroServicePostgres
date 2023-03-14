using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using BuildingLink.ModuleServiceTemplate.Authentication;
using BuildingLink.ModuleServiceTemplate.Services.Books;
using BuildingLink.ModuleServiceTemplate.Services.Books.DTO;
using BuildingLink.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuildingLink.ModuleServiceTemplate.Controllers
{
    /// <summary>
    /// Book controller example with authentication.
    /// </summary>
    [ApiController]
    [Route("book")]
    [Authorize]
    public class BookController : ControllerBase
    {
        private readonly IBookStore _bookStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookController"/> class.
        /// Constructor. Create the BookController instance and it injects all of the necessary services.
        /// </summary>
        /// <param name="bookStore">BookStore.</param>
        public BookController(IBookStore bookStore)
        {
            _bookStore = bookStore;
        }

        /// <summary>Returns all books.</summary>
        /// <returns>Returns all the active books.</returns>
        /// <response code ="200">Returns books.</response>
        /// <response code ="401">Returns an Unauthorized Error.</response>
        /// <response code ="403">Returns a Forbidden Error.</response>
        /// <response code ="500">Returns a server error.</response>
        /// <remarks>Books.</remarks>
        [Produces("application/json")]
        [ProducesResponseType(typeof(IAsyncEnumerable<BookDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ApiExplorerSettings(GroupName = "PropertyEmployee-v1")]
        [HttpGet]
        [AuthorizedRoles(typeof(Roles), Roles.Reader, Roles.Creator)]
        public async Task<IActionResult> GetAll()
        {
            var result = await this._bookStore.GetAllActiveAsync();
            return Ok(result);
        }

        /// <summary>Returns a book.</summary>
        /// <param name="bookId">Book Id.</param>
        /// <returns>Returns a book.</returns>
        /// <response code ="200">Returns book.</response>
        /// <response code ="401">Returns an Unauthorized Error.</response>
        /// <response code ="403">Returns a Forbidden Error.</response>
        /// <response code ="404">Returns a Not Found.</response>
        /// <response code ="500">Returns a server error.</response>
        /// <remarks>Books.</remarks>
        [Produces("application/json")]
        [ProducesResponseType(typeof(BookDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ApiExplorerSettings(GroupName = "PropertyEmployee-v1")]
        [HttpGet("{bookId}")]
        [AuthorizedRoles(typeof(Roles), Roles.Reader, Roles.Creator)]
        public async Task<IActionResult> GetBook([FromRoute] Guid bookId)
        {
            var book = await this._bookStore.GetAsync(bookId);
            if (book is null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        /// <summary>Add new book to the store.</summary>
        /// <param name="createBookDto">New book creation request object.</param>
        /// <returns>Returns the new book added to the store.</returns>
        /// <response code ="201">Returns the new book added to the store.</response>
        /// <response code ="401">Returns an Unauthorized Error.</response>
        /// <response code ="403">Returns a Forbidden Error.</response>
        /// <response code ="409">Returns an Invalid Operation Error.</response>
        /// <response code ="500">Returns a server error.</response>
        /// <remarks>Books.</remarks>
        [Produces("application/json")]
        [ProducesResponseType(typeof(BookDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ApiExplorerSettings(GroupName = "PropertyEmployee-v1")]
        [HttpPost]
        [AuthorizedRoles(typeof(Roles), Roles.Creator)]
        public async Task<IActionResult> Add(CreateBookDto createBookDto)
        {
            var newBook = await this._bookStore.AddBookAsync(createBookDto);
            if (newBook is null)
            {
                return Conflict();
            }

            return Created("Create", newBook);
        }

        /// <summary>Change the book status from "AvailableSoon" to "Available".</summary>
        /// <param name="bookId">Book Id.</param>
        /// <returns>Returns the book with the new status.</returns>
        /// <response code ="200">Returns the book with the new status.</response>
        /// <response code ="401">Returns an Unauthorized Error.</response>
        /// <response code ="403">Returns a Forbidden Error.</response>
        /// <response code ="404">Returns a Not Found.</response>
        /// <response code ="409">Returns an Invalid Operation Error.</response>
        /// <response code ="500">Returns a server error.</response>
        /// <remarks>Books.</remarks>
        [Produces("application/json")]
        [ProducesResponseType(typeof(BookDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ApiExplorerSettings(GroupName = "PropertyEmployee-v1")]
        [HttpPatch("{bookId}")]
        [AuthorizedRoles(typeof(Roles), Roles.Creator)]
        public async Task<IActionResult> ChangeBookStatus([FromRoute] Guid bookId)
        {
            try
            {
                var book = await this._bookStore.ChangeToAvailableAsync(bookId);
                if (book is null)
                {
                    return NotFound();
                }

                return Ok(book);
            }
            catch (InvalidOperationException)
            {
                return Conflict();
            }
        }

        /// <summary>Remove the book from the store.</summary>
        /// <param name="bookId">Book Id.</param>
        /// <returns>Returns 'true' if the book was removed, 'false' if the book was not found.</returns>
        /// <response code ="200">Returns 'true' if the book was removed, 'false' if the book was not found.</response>
        /// <response code ="401">Returns an Unauthorized Error.</response>
        /// <response code ="403">Returns a Forbidden Error.</response>
        /// <response code ="404">Returns a Not Found.</response>
        /// <response code ="409">Returns an Invalid Operation Error.</response>
        /// <response code ="500">Returns a server error.</response>
        /// <remarks>Books.</remarks>
        [Produces("application/json")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ApiExplorerSettings(GroupName = "PropertyEmployee-v1")]
        [HttpDelete("{bookId}")]
        [AuthorizedRoles(typeof(Roles), Roles.Creator)]
        public async Task<IActionResult> RemoveBook([FromRoute] Guid bookId)
        {
            try
            {
                var success = await this._bookStore.RemoveAsync(bookId);
                if (!success)
                {
                    return NotFound();
                }

                return Ok(success);
            }
            catch (InvalidOperationException)
            {
                return Conflict();
            }
        }
    }
}
