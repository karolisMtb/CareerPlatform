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

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(
            IUserService userService, 
            ILogger<UserController> logger,
            UserManager<IdentityUser> userManager)
        {
            _userService = userService;
            _logger = logger;
            _userManager = userManager;
        }

        //delete user
        /// <summary>
        /// Deletes user from database
        /// </summary>
        /// <response code="204">No content</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Server side error</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> User(string email)
        {
            try
            {
                if(!(email is null) || !(email == string.Empty))
                {
                    IdentityResult identityResult = await _userService.DeleteIdentityUserAsync(email);
                    IdentityResult result = await _userService.DeleteUserAsync(email);

                    if(!identityResult.Succeeded || !result.Succeeded)
                    {
                        return BadRequest("User could not be deleted. Please try again");
                    }

                    return Ok(); //turetu buti redirect to home page? Issiaiskinti
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


    }
}
