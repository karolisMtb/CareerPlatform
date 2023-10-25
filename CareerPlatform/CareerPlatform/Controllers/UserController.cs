using CareerPlatform.BusinessLogic.Interfaces;
using CareerPlatform.DataAccess.DTOs;
using CareerPlatform.DataAccess.Entities;
using CareerPlatform.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerPlatform.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Creates new user account
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad request</response>
        /// <response code="409">Conflict error</response>
        /// <response code="500">Server side error</response>
        [HttpPost]
        [Route("user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> User([FromForm] UserSignUpDto userSignUpDto)
        {
            try
            {
                User user = await _userService.SignUpNewUserAsync(userSignUpDto);

                return Ok($"User {user.UserName} was created successfully.");
            }
            catch (ExistingUserFoundException e)
            {
                _logger.LogError($"Client side error occured: {e.Message}");
                return BadRequest(e.Message);
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError($"Server side error occured: {e.Message}");
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Logs in existing user
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad request</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Server side error</response>
        [HttpGet]
        [Route("user/authentication")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // ar cia turi buti 401? 
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Authentication([FromQuery]UserLoginDto userLoginDto) // negali buti from query
        {
            try
            {
                string authenticatedUser = await _userService.AuthenticateUserAsync(userLoginDto);
                if (authenticatedUser == null)
                {
                    return BadRequest("You entered wrong details.");
                }

                return Ok(authenticatedUser);
            }
            catch (PasswordMismatchException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
            catch (UserNotFountException e)
            {
                // cia turi buti 401 status code?
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
            catch(Exception e)
            {
                _logger.LogError($"Server side error occured: {e.Message}");
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Deletes user from database
        /// </summary>
        /// <response code="204">No content</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Server side error</response>
        [HttpDelete]
        [Route("user/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> User([FromQuery]Guid userId)
        {
            try
            {
                await _userService.DeleteUserAsync(userId);

                if (_userService.GetUserByIdAsync(userId) == null)
                {
                    return NoContent();
                }
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError($"Server side error occured: {e.Message}");
                return BadRequest(e.Message);
            }

            return NoContent();
        }

        //update users password when the password is known
        //forgot password https://medium.com/@m.anilkarasah/reset-password-implementation-inside-net-core-web-api-9559dac1d2db


    }
}
