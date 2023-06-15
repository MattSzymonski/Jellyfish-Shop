using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;
using Backend.Models;
using Backend.Services;
using AutoMapper;

namespace Backend.Controllers
{
    [ApiController]
    [EnableCors]
    [Produces("application/json")]
    [Route("api/[Controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> logger;
        private readonly IMapper mapper;
        private readonly UsersService usersService;
        private readonly SecurityService securityService;

        public UsersController(ILogger<UsersController> logger, IMapper mapper, UsersService userService, SecurityService securityService)
        {
            this.logger = logger;
            this.usersService = userService;
            this.securityService = securityService;
            this.mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<Result<LoginResponseDTO>>> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            if (!ModelState.IsValid)
                return new BadRequestResult();

            // Call service
            var loginResult = await usersService.LoginUser(loginRequestDTO);
    
            if (loginResult.Status == Status.Failure) 
            {
                logger.LogInformation($"User {loginRequestDTO.Login} has logged in successfully");
            }
            
            return new OkObjectResult(loginResult);
        }
    }
}
