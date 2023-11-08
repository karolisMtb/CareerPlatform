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
        //private readonly UserManager<IdentityUser> _userManager;
        public UserController(
            IUserService userService, 
            ILogger<UserController> logger
            //UserManager<IdentityUser> userManager
            )
        {
            _userService = userService;
            _logger = logger;
            //_userManager = userManager;
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
        //[Authorize]
        [HttpDelete]
        public async Task<IActionResult> User(string email)
        {
            try //sumazinti kontroleri
            {
                if(!(email is null) || !(email == string.Empty))
                {
                    IdentityResult identityResult = await _userService.DeleteIdentityUserAsync(email);
                    IdentityResult result = await _userService.DeleteNonIdentityUserAsync(email);

                    if(!identityResult.Succeeded || !result.Succeeded)
                    {
                        return BadRequest("User could not be deleted. Please try again");
                    }

                    return Ok("User deleted successfully"); //turetu buti redirect to home page? Issiaiskinti
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


        //delete profile
        //delete Cv
        //delete address


        //change user name
        //change phone number

        ///// <summary>
        ///// Comment
        ///// </summary>
        ///// <response code="200">Success</response>
        ///// <response code="400">Bad request</response>
        ///// <response code="409">Conflict error</response>
        ///// <response code="500">Server side error</response>
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status409Conflict)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[HttpPost]
        //[Route("user")]
        //public async Task<IActionResult> SomeMethod()
        //{

        //}



        ///// <summary>
        ///// Comment
        ///// </summary>
        ///// <response code="200">Success</response>
        ///// <response code="400">Bad request</response>
        ///// <response code="404">Not found</response>
        ///// <response code="500">Server side error</response>
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)] // ar cia turi buti 401? 
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[HttpGet]
        //[Route("user/authentication")]
        //public async Task<IActionResult> SomeMethod()
        //{

        //}



        //forgot password https://medium.com/@m.anilkarasah/reset-password-implementation-inside-net-core-web-api-9559dac1d2db
    }
}
