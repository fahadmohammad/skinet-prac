using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Extensions;
using API.MailService;
using API.ThirdPartyAuth;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Google.Apis.Auth;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IGoogleAuth _googleAuth;
        private readonly IMailService _mailService;
        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager,
            SignInManager<AppUser> signInManager, ITokenService tokenService,
            IMapper mapper, IGoogleAuth googleAuth, IMailService mailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _googleAuth = googleAuth;
            _mailService = mailService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUserAsync()
        {
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            return new UserDto
            {
                Email = user.Email,
                Token = await _tokenService.CreateToken(user),
                DisplayName = user.DisplayName
            };
        }

        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExistAsync([FromQuery] string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var user = await _userManager.FindUserByClaimPrincipleWithAddressAsync(HttpContext.User);

            return _mapper.Map<Address, AddressDto>(user.Address);
        }

        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateAddess(AddressDto addressDto)
        {
            var user = await _userManager.FindUserByClaimPrincipleWithAddressAsync(HttpContext.User);
            user.Address = _mapper.Map<AddressDto, Address>(addressDto);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded) return Ok(_mapper.Map<Address, AddressDto>(user.Address));

            return BadRequest("Problem occurs while updating the user");
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null) return Unauthorized(new ApiResponse(401));

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return Unauthorized(new ApiResponse(401, "Email is not confirmed"));

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password
                , false);

            if (!signInResult.Succeeded) return Unauthorized(new ApiResponse(401));

            var roleList = await _userManager.GetRolesAsync(user);

            return new UserDto
            {
                Email = user.Email,
                Token = await _tokenService.CreateToken(user),
                DisplayName = user.DisplayName,
                Roles = roleList.ToList()
            };
        }

        [HttpPost("ExternalLogin")]
        public async Task<ActionResult<UserDto>> ExternalLogin([FromBody] ExternalAuthDto externalAuth)
        {
            GoogleJsonWebSignature.Payload payload = await _googleAuth.VerifyGoogleToken(externalAuth);

            if (payload == null)
                return BadRequest("Invalid External Authentication.");

            var info = new UserLoginInfo(externalAuth.Provider, payload.Subject, externalAuth.Provider);

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);

                if (user == null)
                {
                    user = new AppUser { Email = payload.Email, UserName = payload.Email, DisplayName = payload.Name };
                    await _userManager.CreateAsync(user);

                    //prepare and send an email for the email confirmation

                    await _userManager.AddToRoleAsync(user, "User");
                    await _userManager.AddLoginAsync(user, info);
                }
                else
                {
                    await _userManager.AddLoginAsync(user, info);
                }
            }

            if (user == null)
                return BadRequest("Invalid External Authentication.");

            var roleList = await _userManager.GetRolesAsync(user);

            return new UserDto
            {
                Email = user.Email,
                Token = await _tokenService.CreateToken(user),
                DisplayName = user.DisplayName,
                Roles = roleList.ToList()
            };

        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            if (CheckEmailExistAsync(registerDto.Email).Result.Value)
            {
                return new BadRequestObjectResult(new ApiValidationErrorResponse
                {
                    Errors = new[] { "Email already exist" }
                });
            }
            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest(new ApiResponse(400));

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var param = new Dictionary<string, string>
            {
                {"token",token},
                {"email",user.Email}
            };

            var callback = QueryHelpers.AddQueryString(registerDto.clientUri, param);

            var mailRequest = new MailRequest()
            {
                ToEmail = user.Email,
                Subject = "Account Activation",
                Body = callback
            };

            await _mailService.SendEmailAsync(mailRequest);

            await _userManager.AddToRoleAsync(user, AppIdentityDbContextSeed.AppRoles.User.ToString());

            return Ok();
        }

        [HttpGet("EmailConfirmation")]
        public async Task<ActionResult<UserDto>> EmailConfirmation([FromQuery] string email, [FromQuery] string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest("Invalid Email Confirmation Request");
            var confirmResult = await _userManager.ConfirmEmailAsync(user, token);
            if (!confirmResult.Succeeded)
                return BadRequest("Invalid Email Confirmation Request");
            var roleList = await _userManager.GetRolesAsync(user);

            return new UserDto
            {
                Email = user.Email,
                Token = await _tokenService.CreateToken(user),
                DisplayName = user.DisplayName,
                Roles = roleList.ToList()
            };
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
                return BadRequest("Invalid Request");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var param = new Dictionary<string, string>
            {
                {"token", token },
                {"email", forgotPasswordDto.Email }
            };

            var callback = QueryHelpers.AddQueryString(forgotPasswordDto.ClientUri, param);

            var mailRequest = new MailRequest()
            {
                ToEmail = user.Email,
                Subject = "Password Reset link",
                Body = callback
            };

            await _mailService.SendEmailAsync(mailRequest);

            return Ok();
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);

            if (user == null)
            {
                return new BadRequestObjectResult(new ApiValidationErrorResponse
                {
                    Errors = new[] { "Invalid Request" }
                });
            }

            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);

            if (!resetPassResult.Succeeded)
            {
                var errors = resetPassResult.Errors.Select(e => e.Description);

                return new BadRequestObjectResult(new ApiValidationErrorResponse
                {
                    Errors = errors
                });
            }

            return Ok();
        }

        [HttpPost("add-role")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<string>> AddRole(AddRoleDto roleDto)
        {
            var user = await _userManager.FindByEmailAsync(roleDto.Email);

            if (user == null)
            {
                return new BadRequestObjectResult(new ApiValidationErrorResponse
                {
                    Errors = new[] { "User with the given email not found" }
                });
            }

            var roleExists = Enum.GetNames(typeof(AppIdentityDbContextSeed.AppRoles))
                                    .Any(x => x.ToLower() == roleDto.Role.ToLower());

            if (roleExists)
            {
                var validRole = Enum.GetValues(typeof(AppIdentityDbContextSeed.AppRoles)).Cast<AppIdentityDbContextSeed.AppRoles>()
                                    .Where(x => x.ToString().ToLower() == roleDto.Role.ToLower()).FirstOrDefault();

                await _userManager.AddToRoleAsync(user, validRole.ToString());

                return $"Added {roleDto.Role} to user {roleDto.Email}.";
            }

            return "Specified Role not found";
        }

    }


}