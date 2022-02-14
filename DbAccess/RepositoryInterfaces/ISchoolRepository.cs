using System.Collections.Generic;
using System.Threading.Tasks;
using Model;

namespace DbAccess.RepositoryInterfaces
{
    public interface ISchoolRepository
    {
        public Task<School> GetSchoolByNameAndAddress(string schoolName, string schoolAddress);
    }
}