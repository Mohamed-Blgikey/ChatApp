using ChatApp.API.AuthServices;
using ChatApp.API.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region fields
        private readonly IAuthServices authservice;
        #endregion

        #region ctor
        public AuthController(IAuthServices authservice)
        {
            this.authservice = authservice;
        }
        #endregion

        #region Login
        [HttpPost]
        [Route("~/Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await authservice.Login(loginDTO);
            if (!result.IsAuthencated)
            {
                return Ok(new { message = result.Message });
            }

            return Ok(new { message = result.Message, token = result.Token, expiresOn = result.ExpiresOn });
        }
        #endregion

        #region Register
        [HttpPost]
        [Route("~/Register")]
        public async Task<IActionResult> Register(RegisterDto registerDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authservice.Register(registerDTO);
            if (!result.IsAuthencated)
                return Ok(new { message = result.Message });

            return Ok(new { message = result.Message, token = result.Token, expiresOn = result.ExpiresOn });
        }
        #endregion

    }
}
