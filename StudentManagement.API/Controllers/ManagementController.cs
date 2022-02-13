using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using DbAccess.RepositoryInterfaces;
using Dto;


namespace StudentManagement.API.Controllers
{
    /// <summary>
    /// Management Controller responsible to all CRUD operations using API calls 
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IStudentRepository _studentRepository;


        public ManagementController(ILogger<ManagementController> logger, IStudentRepository studentRepository)
        {
            _logger = logger;
            _studentRepository = studentRepository;
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
            if (studentId < 1)
            {
                string msg = $"student id: {studentId} must be positive";
                _logger.LogError(msg);
                return BadRequest(msg);
            }
            try
            {
                // get the student from DB
                var student = await _studentRepository.GetStudentById(studentId);
                if (student != null) return Ok(student.ToDto());
                
                string msg = $"student with student id: {studentId} not found in DB";
                _logger.LogError(msg);
                return NotFound(msg);
            }
            catch (Exception e)
            {
                string msg = $"cannot get student with student id: {studentId}. due to: {e}";
                _logger.LogError(msg);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
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
            if (studentDto == null)
            {
                string msg = $"studentDto is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }
            if (studentDto.Id < 0 || studentDto.SchoolId < 1)
            {
                string msg = $"studentDto.id: {studentDto.Id} or " +
                    $"studentDto.SchoolId: {studentDto.SchoolId} are invalid";
                _logger.LogError(msg);
                return BadRequest(msg);
            }
            try
            {
                //add a new student to DB
                var student = await _studentRepository.AddStudent(studentDto.ToModel());
                if (student != null) return Ok(student.ToDto());
                
                string msg = $"cannot add student with student id: {studentDto.Id} to DB";
                _logger.LogError(msg);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
            catch (Exception e)
            {
                string msg = $"cannot add student with student id: {studentDto.Id} to DB. due to: {e}";
                _logger.LogError(msg);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
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
            // validate request
            if (studentDto == null)
            {
                string msg = $"studentDto is null";
                _logger.LogError(msg);
                return BadRequest(msg);
            }
            if (studentDto.Id < 0 || studentDto.SchoolId < 1)
            {
                string msg = $"studentDto.id: {studentDto.Id} or " +
                    $"studentDto.SchoolId: {studentDto.SchoolId} are invalid";
                    _logger.LogError(msg);
                return BadRequest(msg);
            }
            try
            {
                // update student in DB
                var student = await _studentRepository.UpdateStudent(studentDto.ToModel());
                if (student != null) return Ok(student.ToDto());
                
                string msg = $"cannot change the student with student id: {studentDto.Id}";
                _logger.LogError(msg);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);

            }
            catch (Exception e)
            {
                string msg = $"cannot change the student with student id: {studentDto.Id}. due to: {e}";
                _logger.LogError(msg);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
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
                //remove student from DB
                var student = await _studentRepository.DeleteStudent(studentId);
                if (student != false) return Ok(true);
                
                string msg = $"cannot remove the student with the id: {studentId} from DB";
                _logger.LogError(msg);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);

            }
            catch (Exception e)
            {
                string msg = $"cannot remove the student with student id: {studentId} from DB. Due to {e}";
                _logger.LogError(msg);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
        }

    }
}



