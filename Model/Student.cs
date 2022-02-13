namespace Model
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public float Gpa { get; set; }
        public School School { get; set; }
        public int? SchoolId { get; set; }
    }
}