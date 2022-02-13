using System.Threading.Tasks;
using Model;

namespace DbAccess.RepositoryInterfaces
{
    public interface IStudentRepository 
    {
        public Task<Student> GetStudentById(int id);
        public Task<Student> AddStudent(Student student);
        public Task<Student> UpdateStudent(Student student);
        public Task<bool> DeleteStudent(int id);
    }
}