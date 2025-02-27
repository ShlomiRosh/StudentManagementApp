﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using DbAccess.RepositoryInterfaces;
using Dto;
using Microsoft.Extensions.Caching.Distributed;
using Model;
using RedisCacheManagement;


namespace StudentManagement.API.Controllers
{
    /// <summary>
    /// Management Controller responsible to all CRUD operations using API calls 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IStudentRepository _studentRepository;
        private readonly IRedisCacheService _distributedCache;
        
        public ManagementController(ILogger<ManagementController> logger, IStudentRepository studentRepository, IRedisCacheService distributedCache)
        {
            _logger = logger;
            _studentRepository = studentRepository;
            _distributedCache = distributedCache;
        }
        
        /// <summary>
        /// Get StudentDto object by student id
        /// </summary>
        /// <param name="studentId">Student id</param>
        /// <response code="200">StudentDto object contains all of the student details</response>
        /// <response code="400">BadRequest - invalid values (null or empty class code)</response>
        /// <response code="404">NotFound - cannot find the student in DB</response>
        /// <response code="500">InternalServerError - for any error occurred in server</response>
        [ProducesResponseType(typeof(StudentDto), 200)]
        [ProducesResponseType(typeof(BadRequestResult), 400)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        [ProducesResponseType(500)]
        [HttpGet("{studentId}")]
        public async Task<ActionResult<StudentDto>> Get(int studentId)
        {
            // validate request
            if (studentId < 0)
            {
                string msg = $"student id: {studentId} must be positive";
                _logger.LogError(msg);
                return BadRequest(msg);
            }
            try
            {
                var s = JsonConvert.SerializeObject(studentId) + "GET";
                // Get data from cache
                var cacheData = _distributedCache.Get<StudentDto>(s);
                if (cacheData != null) Ok(cacheData);
                // If data not found in cache, get data from DB
                var student = await _studentRepository.GetStudentById(studentId);
                if (student != null)
                {
                    _distributedCache.Set(s, student.ToDto());
                    return Ok(student.ToDto());
                }
                string msg = $"student with student id: {studentId} not found in DB";
                _logger.LogError(msg);
                return NotFound(msg);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        /// <summary>
        /// Add a new student to DB
        /// </summary>
        /// <param name="studentDto">StudentDto object contains all of the student's details which will be added to DB</param>
        /// <response code="200">StudentDto object contains all of the details from DB</response>
        /// <response code="400">BadRequest - invalid values</response>
        /// <response code="500">InternalServerError - for any error occurred in server</response>
        [HttpPost]
        [ProducesResponseType(typeof(StudentDto), 200)]
        [ProducesResponseType(typeof(BadRequestResult), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<StudentDto>> Post([FromBody] StudentDto studentDto)
        {
            //validate request
            if (!CorrectDto(studentDto))
            {
                string msg = $"studentDto has invalid values";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            try
            {
                string s = JsonConvert.SerializeObject(studentDto) + "POST";
                // Get data from cache
                var cacheData = _distributedCache.Get<StudentDto>(s);
                if (cacheData != null) Ok(cacheData);
                //add a new student to DB
                var student = await _studentRepository.AddStudent(studentDto.ToModel());
                if (student != null) 
                {
                    _distributedCache.Set(s, student.ToDto());
                    return Ok(student.ToDto());
                }
                string msg = $"cannot add student with student id: {studentDto.Id} to DB";
                _logger.LogError(msg);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        /// <summary>
        /// Change student in the DB
        /// </summary>
        /// <param name="studentDto">StudentDto object contains all of the new student's details</param>
        /// <response code="200">StudentDto object contains all of the details from DB</response>
        /// <response code="400">BadRequest - invalid values</response>
        /// <response code="500">InternalServerError - for any error occurred in server</response>
        [HttpPut]
        [ProducesResponseType(typeof(StudentDto), 200)]
        [ProducesResponseType(typeof(BadRequestResult), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<StudentDto>> Put([FromBody] StudentDto studentDto)
        {
            if (!CorrectDto(studentDto))
            {
                string msg = $"studentDto has invalid values";
                _logger.LogError(msg);
                return BadRequest(msg);
            }
            
            try
            {
                string s = JsonConvert.SerializeObject(studentDto) + "UPDATE";
                // Get data from cache
                var cachedData = _distributedCache.Get<StudentDto>(s);
                if (cachedData != null) return Ok(cachedData);
                // update student in DB
                var student = await _studentRepository.UpdateStudent(studentDto.ToModel());
                if (student != null)
                {
                    _distributedCache.Set(s, student.ToDto());
                    return Ok(student.ToDto());
                }
                string msg = $"cannot change the student with student id: {studentDto.Id}";
                _logger.LogError(msg);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);

            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }
        
        /// <summary>
        /// A private method to check if the studentDto has valid values
        /// </summary>
        /// <param name="studentDto">StudentDto object contains all of the new student's details</param>
        /// <returns>true if the studentDto has valid values, false otherwise</returns>
        private bool CorrectDto(StudentDto studentDto)
        {
            if (studentDto == null || studentDto.Id < 0 || studentDto.SchoolId < 0 || studentDto.Age > 18 ||
                String.IsNullOrEmpty(studentDto.FirstName) || String.IsNullOrEmpty(studentDto.LastName) || 
                studentDto.Gpa < 0 || String.IsNullOrEmpty(studentDto.School.Name) || 
                String.IsNullOrEmpty(studentDto.School.Address))
            {
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Delete studentDto object by student id
        /// </summary>
        /// <param name="studentId">The lesson we want to delete </param>
        /// <response code="200">bool true</response>
        /// <response code="400">BadRequest - invalid values (lower than 1)</response>
        /// <response code="404">NotFound - cannot find the student in DB</response>
        /// <response code="500">InternalServerError - for any error occurred in server</response>
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(BadRequestResult), 400)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        [ProducesResponseType(500)]
        [HttpDelete("{studentId}")]
        public async Task<ActionResult<bool>> Delete(int studentId)
        {
            if (studentId < 0)
            {
                string msg = $"student id: {studentId} must be positive";
                _logger.LogError(msg);
                return BadRequest(msg);
            }
            try
            {
                string s = JsonConvert.SerializeObject(studentId) + "DELETE";
                // Get data from cache
                var cachedData = _distributedCache.Get<bool>(s);
                if (cachedData) return Ok(cachedData);
                //remove student from DB
                var student = await _studentRepository.DeleteStudent(studentId);
                if (student)
                {
                    _distributedCache.Set(s, student);
                    return Ok(true);
                }
                string msg = $"cannot remove the student with the id: {studentId} from DB";
                _logger.LogError(msg);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);

            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

    }
}



