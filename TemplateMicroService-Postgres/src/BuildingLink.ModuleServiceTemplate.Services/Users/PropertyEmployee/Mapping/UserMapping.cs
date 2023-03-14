using AutoMapper;
using BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst.Users;
using BuildingLink.ModuleServiceTemplate.Services.Users.PropertyEmpolyee.DTO;

namespace BuildingLink.ModuleServiceTemplate.Services.Users.PropertyEmpolyee.Mapping
{
    /// <summary>
    /// Book automapper profile.
    /// </summary>
    public class UserMapping : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserMapping"/> class.
        /// Create a new Book automapper profile instance.
        /// </summary>
        public UserMapping()
        {
            CreateMap<User, UserResponseDto>();
        }
    }
}
