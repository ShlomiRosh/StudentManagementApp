using Model;

namespace Dto
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public float Gpa { get; set; }
        public School School { get; set; }
        public int? SchoolId { get; set; }
    }
    
    public static class StudentDtoExtensions
    {
        public static StudentDto ToDto(this Student student)
        {
            return new StudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Age = student.Age,
                Gpa = student.Gpa,
                School = student.School,
                SchoolId = student.SchoolId
            };
        }
    }
    
    public static class StudentExtensions
    {
        public static Student ToModel(this StudentDto studentDto)
        {
            return new Student
            {
                Id = studentDto.Id,
                FirstName = studentDto.FirstName,
                LastName = studentDto.LastName,
                Age = studentDto.Age,
                Gpa = studentDto.Gpa,
                School = studentDto.School,
                SchoolId = studentDto.SchoolId
            };
        }
    }
}