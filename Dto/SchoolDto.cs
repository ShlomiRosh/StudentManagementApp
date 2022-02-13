using Model;

namespace Dto
{
    public class SchoolDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
    
    public static class SchoolDtoExtensions
    {
        public static SchoolDto ToDto(this School school)
        {
            return new SchoolDto
            {
                Id = school.Id,
                Name = school.Name,
                Address = school.Address
            };
        }
    }
    
    public static class SchoolExtensions
    {
        public static School ToModel(this SchoolDto schoolDto)
        {
            return new School
            {
                Id = schoolDto.Id,
                Name = schoolDto.Name,
                Address = schoolDto.Address
            };
        }
    }
}