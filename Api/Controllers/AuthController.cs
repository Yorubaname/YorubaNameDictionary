using Api.Utilities;
using Application.Services;
using Core.Dto.Response;
using Core.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IValidator<CreateUserDto> _userValidator;

        public AuthController(UserService userService, IValidator<CreateUserDto> userValidator)
        {
            _userService = userService;
            _userValidator = userValidator;
        }

        [Authorize]
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Login()
        {
            var theUser = await _userService.GetUserByEmail(User.Identity!.Name!);
            var userDetails = new UserDto
            {
                Id = theUser.Id,
                Roles = theUser.Roles.ToArray(),
                Username = theUser.Email
            };

            return Ok(userDetails);
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(Dictionary<string, string>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create([FromBody] CreateUserDto createUserDto)
        {
            var result = await _userValidator.ValidateAsync(createUserDto);
            if (!result.IsValid)
            {
                result.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }

            var existingUser = await _userService.GetUserByEmail(createUserDto.Email!);

            if (existingUser != null)
            {
                return BadRequest("This user already exists");
            }

            _ = await _userService.CreateUser(createUserDto);

            return Ok(ResponseHelper.GetResponseDict("success"));
        }
    }
}
