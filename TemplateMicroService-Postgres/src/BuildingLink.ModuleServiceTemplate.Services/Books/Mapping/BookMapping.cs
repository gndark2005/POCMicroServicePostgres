using AutoMapper;
using BuildingLink.ModuleServiceTemplate.Repositories;
using BuildingLink.ModuleServiceTemplate.Services.Books.DTO;

namespace BuildingLink.ModuleServiceTemplate.Services.Books.Mapping
{
    /// <summary>
    /// Book automapper profile.
    /// </summary>
    public class BookMapping : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BookMapping"/> class.
        /// Create a new Book automapper profile instance.
        /// </summary>
        public BookMapping()
        {
            CreateMap<BookDto, Book>()
                .ReverseMap();
            CreateMap<CreateBookDto, Book>();
        }
    }
}
