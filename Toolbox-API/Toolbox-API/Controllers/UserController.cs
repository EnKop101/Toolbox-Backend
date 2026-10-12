using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Toolbox_API.Extension;
using Toolbox_Library.Utilities.Encryption.Interface;

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


    }
}
