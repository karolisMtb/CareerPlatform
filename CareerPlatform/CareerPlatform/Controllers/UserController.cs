using CareerPlatform.BusinessLogic.Interfaces;
using CareerPlatform.DataAccess.Models;
using CareerPlatform.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CareerPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService _userService,
            ILogger<UserController> _logger) : ControllerBase
    {

        /// <summary>
        /// Deletes logged in user from database
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="204">No content</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Server side error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromBody]DeleteModel deleteModel)
        {
            try
            {
                if(!string.IsNullOrEmpty(deleteModel.Email) && !string.IsNullOrEmpty(deleteModel.Password))
                {
                    var result = await _userService.DeleteUserByEmailAsync(deleteModel.Email, deleteModel.Password);

                    if (!result.Succeeded)
                    {
                        return BadRequest("User could not be deleted. Please try again");
                    }

                    return Ok("User deleted successfully");
                }
            }
            catch(UserNotFoundException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return StatusCode(500, $"Internal server side error occured: {e.Message}");
            }

            return NoContent();
        }
    }
}
