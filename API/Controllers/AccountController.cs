using API.Controllers;
using API.Dtos;
using Core.Entities;
using Core.Responses;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseApiController
    {
        private readonly UserManager<User> _userManager;
        private readonly TokenService _tokenService;

        public AccountController(UserManager<User> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto command)
        {

            // See is there any user already registered with the command username
            var response = await _userManager.FindByNameAsync(command.Username);

            if (response != null)
            {
                // Create an api response for error case
                return Ok(new ControllerResponse
                {
                    ResponseCode = -300,
                    Errors = new List<string> { "There is user already with the given username" },
                });
            }

            // Create user 
            var user = new User
            {
                UserName = command.Username,
                Email = command.Username,
                Firstname = command.Firstname,
                Lastname = command.Lastname,
                GdprApprove = command.GdprApprove,
            };

            // Create request for new user
            var result = await _userManager.CreateAsync(user, command.Password);

            // Check is it succeeded
            if (!result.Succeeded)
            {
                // Add errors to model state and return validation problem
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return ValidationProblem();
            }

            // Add user role to current user
            await _userManager.AddToRoleAsync(user, "User");

            // Create an api response
            return Ok(new UserDto
            {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                UserName = user.UserName,
                Token = "Bearer " + await _tokenService.GenerateToken(user),
            });

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto command)
        {

            // See there is an user exists
            var user = await _userManager.FindByNameAsync(command.Username);

            // check if user is not exists and return 404 status code
            if (user == null)
            {
                return StatusCode(404, "User is not found");
            }

            // check user write correct password
            if (!await _userManager.CheckPasswordAsync(user, command.Password))
            {
                return Unauthorized();
            }

            // Create an api response
            return new UserDto
            {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                Token = "Bearer " + await _tokenService.GenerateToken(user),
            };
        }
    }
}
