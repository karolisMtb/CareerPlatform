using CareerPlatform.BusinessLogic.Interfaces;
using CareerPlatform.DataAccess.DTOs;
using CareerPlatform.DataAccess.Entities;
using CareerPlatform.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CareerPlatform.API.Controllers
{
    //user controller bus viskas, kas keis user ir dependencies state(CV, email ir panasiai)

    [Route("api/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(IUserService userService, 
            ILogger<UserController> logger,
            UserManager<IdentityUser> userManager)
        {
            _userService = userService;
            _logger = logger;
            _userManager = userManager;
        }

        /// <summary>
        /// Creates new user account
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad request</response>
        /// <response code="409">Conflict error</response>
        /// <response code="500">Server side error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("user")]
        public async Task<IActionResult> User([FromForm] UserSignUpDto userSignUpDto)
        {
            // validation reikia del input username, password(min 12 simboliu) ir email, kad butu email. Regex
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


        //sita endpointa trinti arba iskomentuoti

        /// <summary>
        /// Logs in existing user
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad request</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Server side error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // ar cia turi buti 401? 
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("user/authentication")]
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete]
        [Route("user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> User(Guid userId)
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

        //forgot password https://medium.com/@m.anilkarasah/reset-password-implementation-inside-net-core-web-api-9559dac1d2db
    }
}
