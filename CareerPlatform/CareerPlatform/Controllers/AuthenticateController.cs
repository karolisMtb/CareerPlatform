using CareerPlatform.BusinessLogic.Interfaces;
using CareerPlatform.DataAccess.DTOs;
using CareerPlatform.DataAccess.Entities;
using CareerPlatform.DataAccess.Validators;
using CareerPlatform.Shared.Exceptions;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerPlatform.API.Controllers
{
    [Route("api/authenticate")]
    [ApiController]
    
    //TODO  
    //correct logErrors and return messages to clear and understandable

    public class AuthenticateController(ILogger<AuthenticateController> _logger,
            IUserService _userService,
            IPasswordReminderService _passwordReminderService,
            IAuthenticateService _authenticateService) : ControllerBase
    {
      
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

            var validator = new RegisterValidator();
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                return BadRequest($"Validation failed: {errorMessages}");
            }

            try
            {
                IdentityResult result = await _authenticateService.RegisterUserAsync(model);
                    
                if (!result.Succeeded)
                {
                    _logger.LogError("User was not able to be registered.");
                    return BadRequest("User registration failed! Please check user details and try again.");
                }

                return Ok("User created successfully.");
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError($"An error occured while registering new account: {e.Message}");
                return BadRequest($"An error occured while registering new account: {e.Message}");
            }
            catch (ExistingUserFoundException e)
            {
                _logger.LogError($"User with these credentials already exists: {e.Message}");
                return BadRequest($"User with these credentials already exists: {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Server side error occured: {e.Message}");
                return StatusCode(500, "Server side error occured.");
            }
        }

        /// <summary>
        /// Logs in existing user
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad request</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Server side error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {

            var validator = new LoginValidator();
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                return BadRequest($"Validation failed: {errorMessages}");
            }

            try
            {
                User user = await _userService.GetUserByNameAsync(model);
                bool valid = await _authenticateService.ValidateUserPasswordAsync(model);


                if (user is not null && valid is true)
                {
                    LoginValidationDto token = await _authenticateService.ValidateUserLoginAsync(user);

                    return Ok(token);
                }

                return Unauthorized("Login was unsuccessfull. Focus, dude!");
            }
            catch (UserNotFoundException e)
            {
                _logger.LogError($"User was not found. Check and try again: {e.Message}");
                return BadRequest($"User was not found. Check and try again: {e.Message}");
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError($"Login was unsuccessfull: {e.Message}");
                return BadRequest($"Login was unsuccessfull: {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Server side error occured: {e.Message}");
                return StatusCode(500, $"Server side error occured: {e.Message}");
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
        /// Initiates password reset service
        /// </summary>
        /// <response code="202">Accepted</response>
        /// <response code="400">Bad request</response>
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
                var user = await _userService.GetUserByEmailAddressAsync(request.Email);

                if (user is not null)
                {
                    await _authenticateService.SendPasswordResetLinkAsync(user);
                    return Accepted("Email for password recovery was sent successfully");
                }
                return BadRequest("Failed. Please try again.");
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e.Message);
                return BadRequest($"Client side error: {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Server side error occured: {e.Message}");
                return StatusCode(500, $"Server side error occured: {e.Message}");
            }
        }

        /// <summary>
        /// Validates password reset request params
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Server side error</response>
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

                if (validRequest is true)
                {
                    return Ok(validRequest);
                }

                return BadRequest("Failed to reset the password. Please, try again.");
            }
            catch (UserNotFoundException e)
            {
                _logger.LogError($"Client side error occured: {e.Message}");
                return NotFound(e.Message);
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError($"Could not process input: {e.Message}");
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return StatusCode(500, "Server side error occured.");
            }
        }

        /// <summary>
        /// Resets password from form
        /// </summary>
        /// <response code="202">Accepted</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Server side error</response>
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                User user = await _userService.GetUserByEmailAddressAsync(request.UserEmail);

                if (user is null)
                {
                    _logger.LogError($"User with the email {user?.Email} was not found");
                    return BadRequest($"User with the email {user?.Email} was not found");
                }

                if (request.Password == null)
                {
                    _logger.LogError("Your must enter new password in order to reset it.");
                    return BadRequest("Your must enter new password in order to reset it.");
                }

                IdentityResult result = await _authenticateService.ResetPasswordAsync(user, request.Token, request.Password);

                if (result.Succeeded)
                {
                    return Ok("Password updated successfully");
                }
                else
                {
                    _logger.LogError("Password reset operation was not successfull.");
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Server side error occured: {e.Message}");
                return StatusCode(500, $"Server side error occured: {e.Message}");
            }
        }

        /// <summary>
        /// Changes user password
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Server side error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpPatch]
        [Route("change-password")]
        public async Task<IActionResult> ChangeUserPassword([FromBody] UserPasswordDto userPasswordDto)
        {
            try
            {
                IdentityResult result = await _userService.ChangeUserPasswordAsync(userPasswordDto.Email, userPasswordDto.OldPassword, userPasswordDto.NewPassword);

                if (!result.Succeeded)
                {
                    _logger.LogError("An attempt to change user's password was unsuccessfull.");
                    return BadRequest("An attempt to change user's password was unsuccessfull.");
                }

                return Ok("Password was changed successfully");
            }
            catch (PasswordChangeFailedException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError($"Server side error occured: {e.Message}");
                return StatusCode(500, $"Server side error occured: {e.Message}");
            }
        }

        /// <summary>
        /// Validates account registration confirmation from email
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad request</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Server side error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch]
        [Route("confirm-registration")]
        public async Task<IActionResult> ConfirmAccountRegistration([FromQuery]RegistrationConfirmationRequestDto registrationConfirmationRequestDto)
        {
            try
            {
                if(string.IsNullOrEmpty(registrationConfirmationRequestDto.Email) || string.IsNullOrEmpty(registrationConfirmationRequestDto.Token))
                {
                    return BadRequest("Registration could not be confirmed.");
                }

                string decryptedEmail = await _authenticateService.DecryptConfirmationEmailAsync(registrationConfirmationRequestDto.Email);
                string decrypteToken = await _authenticateService.DecryptConfirmationTokenAsync(registrationConfirmationRequestDto.Token);

                var user = await _userService.GetUserByEmailAddressAsync(decryptedEmail);

                if (user is null)
                {
                    return BadRequest("Invalid email or user not found.");
                }

                IdentityResult result = await _userService.ConfirmUserRegistrationAsync(user, decrypteToken);

                if (!result.Succeeded)
                {
                    return BadRequest("Registration confirmation failed.");
                }
        
                return Ok("Account registration is successfully confirmed.");
            }
            catch(ArgumentNullException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError($"Server side error occured: {e.Message}");
                return StatusCode(500, $"Server side error occured: {e.Message}");
            }
        }


        private async Task<User> GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            
            if (identity != null)
            {
                var userClaims = identity.Claims;
                return new User
                {
                    UserName = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                    //Role = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value
                };
            }
            return null;
        }
    }
}
