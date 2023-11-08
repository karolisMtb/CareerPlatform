using CareerPlatform.BusinessLogic.Interfaces;
using CareerPlatform.DataAccess.DTOs;
using CareerPlatform.Shared.Exceptions;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace CareerPlatform.API.Controllers
{
    [Route("api/authenticate")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AuthenticateController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IPasswordReminderService _passwordReminderService;
        private readonly IUserService _userService;
        private readonly IAuthenticateService _authenticateService;

        public AuthenticateController(
            UserManager<IdentityUser> userManager,
            ILogger<AuthenticateController> logger,
            IEmailSender emailSender,
            IUserService userService,
            IPasswordReminderService passwordReminderService,
            IAuthenticateService authenticateService)
        {
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _userService = userService;
            _passwordReminderService = passwordReminderService;
            _authenticateService = authenticateService; 
        }

        /// <summary>
        /// Logs in existing user
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">No found</response>
        /// <response code="500">Server side error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                //kodo struktura kitaip galima parasyti. 
                //userManager kelti i servisa visam application
                IdentityUser user = await _userManager.FindByNameAsync(model.Username);
                bool validPassword = await _userManager.CheckPasswordAsync(user, model.Password);

                if (user is not null && validPassword is true)
                {
                    LoginValidationDto token = await _authenticateService.ValidateUserLoginAsync(user);

                    return Ok(token);
                }

                return Unauthorized("Login was unsuccessfull. Focus, dude!");
            }
            catch(ArgumentNullException e)
            {
                _logger.LogError($"Client side error occured: {e.Message}");
                return BadRequest($"Login was unsuccessfull. {e.Message}");
            }
            catch (Exception e) //tai cia 400 status code? Tai kur 500?
            {
                _logger.LogError($"Server side error occured: {e.Message}");
                return BadRequest("Server side error");
            }
        }

        /// <summary>
        /// Registers new user
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Server side error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                IdentityResult result = await _authenticateService.RegisterUserAsync(model);

                if (!result.Succeeded)
                {
                    return BadRequest("User creation failed! Please check user details and try again.");
                }

                return Ok("User created successfully!");
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError($"Client side error occured: {e.Message}");
                return BadRequest(e.Message);
            }
            catch (ExistingUserFoundException e)
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

        //[HttpPost]
        //[Route("register-admin")]
        //public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        //{
        //    var userExists = await _userManager.FindByNameAsync(model.Username);
        //    if (userExists != null)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

        //    IdentityUser user = new()
        //    {
        //        Email = model.Email,
        //        SecurityStamp = Guid.NewGuid().ToString(),
        //        UserName = model.Username
        //    };
        //    var result = await _userManager.CreateAsync(user, model.Password);
        //    if (!result.Succeeded)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

        //    if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
        //        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        //    if (!await _roleManager.RoleExistsAsync(UserRoles.User))
        //        await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

        //    if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
        //    {
        //        await _userManager.AddToRoleAsync(user, UserRoles.Admin);
        //    }
        //    if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
        //    {
        //        await _userManager.AddToRoleAsync(user, UserRoles.User);
        //    }
        //    return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        //}


        /// <summary>
        /// Handles password reset request
        /// </summary>
        /// <response code="202">Accepted</response>> 
        /// <response code="400">Bad request</response>> 
        /// <response code="500">Server side error</response>
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [AllowAnonymous]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            try
            {
                IdentityUser user = await _userManager.FindByEmailAsync(request.Email);
                    
                if (user is not null)
                {
                    await _emailSender.ResetPasswordAsync(user);
                    return Accepted("Email for password recovery was sent successfully");
                }
                return BadRequest("Failed. Please try again.");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest($"Something went wrong: {e.Message}");
            }            
        }

        /// <summary>
        /// Validates password reset request params
        /// </summary>
        /// <response code="200">Success</response>> 
        /// <response code="400">Bad request</response>> 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [AllowAnonymous]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid parameters");
            }

            try
            {
                bool validRequest = await _passwordReminderService.ValidatePasswordResetRequestAsync(email, token);
                //return Redirect(); //param: url i kur bus redirected
                return Ok(validRequest);
            }
            catch (UserNotFoundException e)
            {
                _logger.LogError($"Client side error occured: {e.Message}");
                return NotFound(e.Message);
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError($"Server side error occured: {e.Message}");
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                //return LocalRedirect(gal i password input form?); //pasidometi kur tas redirect nukreipia
                return BadRequest(e.Message);
            }            
        }

        /// <summary>
        /// Resets password from form
        /// </summary>
        /// <response code="202">Accepted</response>> 
        /// <response code="400">Bad request</response>> 
        /// <response code="500">Server side error</response>
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                IdentityUser user = await _userManager.FindByEmailAsync(request.UserEmail);

                if (user == null)
                {
                    return BadRequest($"User with this email {request.UserEmail} was not found");
                }

                IdentityResult result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);

                if (result.Succeeded)
                {
                    return Ok("Password reset successfully");
                }
                else //sito gali ir nereiketi. Reikia patikrinti
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest("Failed to reset password");
            }
        }

        /// <summary>
        /// Changes user password
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad request</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Server side error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpPatch]
        [Route("change-password")]
        //is vending ar companyX rasti kaip gauti useri, kuris yra siuo metu prisilogines ir perduoti i si endpoint
        public async Task<IActionResult> ChangeUserPassword(
            string userEmail, 
            [FromBody] UserPasswordDto userPasswordDto)
        {
            try
            {
                IdentityResult result = await _userService.ChangeUserPasswordAsync(userEmail, userPasswordDto.OldPassword, userPasswordDto.NewPassword);

                if (!result.Succeeded)
                {
                    return BadRequest("Password could not be changed. Something went wrong");
                }
                
                return Ok("Password was changed successfully");
            }
            catch (TargetInvocationException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
        }
    }
}
