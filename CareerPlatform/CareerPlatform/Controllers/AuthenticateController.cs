using CareerPlatform.BusinessLogic.Interfaces;
using CareerPlatform.DataAccess.DTOs;
using CareerPlatform.Shared.Exceptions;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace CareerPlatform.API.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/authenticate")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticateController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IPasswordReminderService _passwordReminderService;
        private readonly IUserService _userService;

        public AuthenticateController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ILogger<AuthenticateController> logger,
            IEmailSender emailSender,
            IUserService userService,
            IPasswordReminderService passwordReminderService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
            _emailSender = emailSender;
            _userService = userService;
            _passwordReminderService = passwordReminderService;
        }

        /// <summary>
        /// Logs in existing user
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Server side error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Username); //kodel ne email?
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var userRoles = await _userManager.GetRolesAsync(user); //neturetu buti _roleManager?

                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var token = GetToken(authClaims);

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });

                }

                return Unauthorized(); //tikrai unauthorized turi buti?
            }
            catch(ArgumentNullException e)
            {
                _logger.LogError($"Client side error occured: {e.Message}");
                return BadRequest($"Login was unsuccessfull. {e.Message}");
            }
            catch (Exception e)
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
                var userByName = await _userManager.FindByNameAsync(model.Username);
                var userByEmail = await _userManager.FindByEmailAsync(model.Email);

                if(userByName != null || userByEmail !=null)
                {
                    return BadRequest("User already exists");
                }
            
                IdentityUser user = new()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                var nonIdentityUser = await _userService.CreateNonIdentityUserAsync(user);

                if (!result.Succeeded && nonIdentityUser is not null)
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
            catch (Exception e)
            {
                _logger.LogError($"Server side error occured: {e.Message}");
                return BadRequest(e.Message);
            }
           
        }




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
                var user = await _userManager.FindByEmailAsync(request.Email);
                    
                if (user is not null)
                {
                    string baseUrl = _configuration["Application:BaseHost"];
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                    var encodedEmail = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(user.Email));
                    var callbackUrl = $"{baseUrl}/api/authenticate/reset-password?email={encodedEmail}&token={encodedToken}";
                    await _emailSender.SendEmailAsync(request.Email, "Reset your password",
                        $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest($"Something went wrong: {e.Message}");
            }
            
            return Accepted("Email for password recovery was sent successfully");
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
                else
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
            [FromBody] string oldPassword, 
            [FromBody] string newPassword)
        {
            try
            {
                IdentityResult result = await _userService.ChangeUserPasswordAsync(userEmail, oldPassword, newPassword);

                if (!result.Succeeded)
                {
                    return BadRequest("Password could not be changed. Something went wrong");
                }
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
            return Ok("Password was changed successfully");
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
