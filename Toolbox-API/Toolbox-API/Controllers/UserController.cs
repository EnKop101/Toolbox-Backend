using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Toolbox_API.Extension;
using Toolbox_Library.DTO;
using Toolbox_Library.Enums;
using Toolbox_Library.Services.Interfaces;
using Toolbox_Library.Utilities.Encryption.Interface;
using Toolbox_Library.Utilities.Response;

namespace Toolbox_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [EnableRateLimiting("login")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                return Ok(await _userService.Login(dto));
            }
            catch (HttpResponseException hre)
            {
                return StatusCode((int)hre.StatusCode, hre.ResponseMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //[Authorize(UserRole.Admin, UserRole.Owner)]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO dto)
        {
            try
            {
                await _userService.Add(dto);
                return Ok();
            }
            catch (HttpResponseException hre)
            {
                return StatusCode((int)hre.StatusCode, hre.ResponseMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
