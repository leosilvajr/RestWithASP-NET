using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestWithASPNET.Business;
using RestWithASPNET.Data.VO;
using RestWithASPNET.Model;
using RestWithASPNET.Repository;

namespace RestWithASPNET.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    [Route("api/[controller]/v{version:apiVersion}")]
    public class AuthController : ControllerBase
    {
        private ILoginBusiness _loginBusiness;
        private readonly IUserRepository _userRepository;

        public AuthController(ILoginBusiness loginBusiness,  IUserRepository userRepository)
        {
            _loginBusiness = loginBusiness;
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("signin")]
        public IActionResult Signin([FromBody] UserVO user)
        {
            if (user == null) return BadRequest("Ivalid client request");
            var token = _loginBusiness.ValidateCredentials(user);
            if (token == null) return Unauthorized();
            return Ok(token);
        }

        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh([FromBody] TokenVO tokenVo)
        {
            if (tokenVo == null) return BadRequest("Ivalid client request");
            var token = _loginBusiness.ValidateCredentials(tokenVo);
            if (token == null) return BadRequest("Ivalid client request");
            return Ok(token);
        }

        [HttpPost]
        [Route("signup")]
        public IActionResult Signup([FromBody] User newUser)
        {
            if (newUser == null) return BadRequest("Invalid client request");
            var user = _userRepository.CreateUser(newUser);
            if (user == null) return BadRequest("User already exists");
            return Ok(user);
        }

        [HttpGet]
        [Route("revoke")] //User revoke no LogOff
        [Authorize("Bearer")] //Adicionando regra para que obriga autenticação
        public IActionResult Revoke() // Não precisa passar parametro porque ja temos o Bearer
        {
            var username = User.Identity.Name;
            var result = _loginBusiness.RevokeToken(username);

            if (!result) return BadRequest("Ivalid client request");
            return Ok("Logoff realizado com sucesso. Faça login novamente para gerar token.");
        }
    }
}
