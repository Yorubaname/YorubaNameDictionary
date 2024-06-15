using Api.Utilities;
using Application.Services;
using Core.Dto.Request;
using Core.Dto.Response;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  //  [Authorize(Policy = "AdminAndLexicographers")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IValidator<CreateUserDto> _userValidator;
        private readonly IValidator<UpdateUserDto> _updateUserValidator;

        public AuthController(
            UserService userService, 
            IValidator<CreateUserDto> userValidator,
            IValidator<UpdateUserDto> updateUserValidator)
        {
            _userService = userService;
            _userValidator = userValidator;
            _updateUserValidator = updateUserValidator;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Login()
        {
            var theUser = await _userService.GetUserByEmail(User.Identity!.Name!);
            var userDetails = new UserDto
            {
                Roles = theUser.Roles.ToArray(),
                Username = theUser.Email,
            };

            return Ok(userDetails);
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(Dictionary<string, string>), (int)HttpStatusCode.Created)]
      //  [Authorize(Policy = "AdminOnly")]
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
                return BadRequest(ResponseHelper.GetResponseDict("This user already exists"));
            }

            _ = await _userService.CreateUser(createUserDto);

            return Ok(ResponseHelper.GetResponseDict("success"));
        }

        [HttpDelete("users/{email}")]
        [ProducesResponseType(typeof(Dictionary<string, string>), (int)HttpStatusCode.OK)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(string email)
        {
            bool isDeleted = await _userService.DeleteBy(email);

            if (isDeleted)
            {
                return Ok(ResponseHelper.GetResponseDict("Name deleted successfully."));
            }

            return BadRequest(ResponseHelper.GetResponseDict("Delete failed: User not found."));
        }

        [HttpPatch("users/{email}")]
        [ProducesResponseType(typeof(Dictionary<string, string>), (int)HttpStatusCode.OK)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(string email, [FromBody] UpdateUserDto update)
        {
            var result = await _updateUserValidator.ValidateAsync(update);
            if (!result.IsValid)
            {
                result.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }

            bool isUpdated = await _userService.Update(email, update);

            if (isUpdated)
            {
                return Ok(ResponseHelper.GetResponseDict("Name updated successfully."));
            }

            return BadRequest(ResponseHelper.GetResponseDict("Update failed: User not found."));
        }

        [HttpGet("users")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            IEnumerable<UserDto> allUsers = await _userService.List();
            return Ok(allUsers);
        }

        [HttpGet("users/{email}")]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(string email)
        {
            var theUser = await _userService.GetUserByEmail(email);
            return theUser == null ?
                NotFound(ResponseHelper.GetResponseDict("User was not found.")) :
                Ok(new UserDto
                {
                    Email = theUser.Email!,
                    Roles = theUser.Roles.ToArray(),
                    Username = theUser.Username,
                });
        }
    }
}
