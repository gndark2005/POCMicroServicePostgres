namespace BuildingLink.ModuleServiceTemplate.Services.Users.PropertyEmpolyee.DTO
{
    public class UserResponseDto
    {
        public int Id { get; set; }

        public int? FacilityId { get; set; }

        public int? UnitId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }
        public bool IsActive { get; set; }

        public bool? ReadOnly { get; set; }

        public int? CreatedByUserId { get; set; }

        public int? LastChangedByUserId { get; set; }

        public UserResponseDto CreatedBy { get; set; }

        public UserResponseDto LastChanged { get; set; }
    }
}
