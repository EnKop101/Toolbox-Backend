using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Toolbox_API.Extension;
using Toolbox_Library.Utilities.Encryption.Interface;
using Toolbox_Library.Utilities.Response;

namespace Toolbox_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IEncryptionHandler _encryptionHandler;
        public UserController(IEncryptionHandler encryptionHandler)
        {
            _encryptionHandler = encryptionHandler;
        }

        [EnableRateLimiting("login")]
        [HttpPost("login")]
        public async Task<IActionResult> Login()
        {
            try
            {
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
