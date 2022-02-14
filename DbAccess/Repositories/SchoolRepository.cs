using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbAccess.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model;

namespace DbAccess.Repositories
{
    public class SchoolRepository : ISchoolRepository
    {
        private readonly StudentManagementContext _context;
        private readonly ILogger _logger;

        public SchoolRepository(StudentManagementContext context, ILogger<SchoolRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        /// <summary>
        /// Get school by Name and Address
        /// </summary>
        /// <param name="schoolName"></param>
        /// <param name="schoolAddress"></param>
        /// <returns>School</returns>
        public async Task<School> GetSchoolByNameAndAddress(string schoolName, string schoolAddress)
        {
            try
            {
                return await _context.Schools.Where(x => x.Name == schoolName && x.Address == schoolAddress).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                _logger.LogError($"Cannot get school from DB. With school name: {schoolName} and address: {schoolAddress}. due to: {e}");
                return null;
            }
        }
        
    }
}