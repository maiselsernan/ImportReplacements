using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportReplacement.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImportReplacement.Api.Controllers
{
    [Route(("api/[controller]/[action]"))]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public readonly IAuthenticationRepository _authenticationRepository;

        public AuthenticationController(IAuthenticationRepository authenticationRepository)
        {
            _authenticationRepository = authenticationRepository;
        }

        [HttpPost]
        //[HttpGet("{password}")]
        public ActionResult Authenticate([FromBody]string password)
        {
            try
            {
                if (string.IsNullOrEmpty(password))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Wrong Password!!");
                }
                return Ok(_authenticationRepository.Authenticate(password));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Wrong Password!!");
            }
        }

    }


 

}
