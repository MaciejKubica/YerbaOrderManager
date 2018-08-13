using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using App.Data.Entities;
using App.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace App.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<StoreUserExtended> _signInManager;
        private readonly UserManager<StoreUserExtended> _userManager;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AccountController(ILogger<AccountController> logger,
            SignInManager<StoreUserExtended> signInManager,
            UserManager<StoreUserExtended> userManager,
            IConfiguration config, IMapper mapper)
        {
            this._logger = logger;
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._config = config;
            this._mapper = mapper;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);                

                if (result.Succeeded)
                {
                    return Ok();
                }
            }

            return BadRequest();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.UserName);
                var userRoles = await _userManager.GetRolesAsync(user);
                var isLockedOut = await _userManager.IsLockedOutAsync(user);

                if (user != null && !isLockedOut)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                    if(result.Succeeded)
                    {
                        var token = CreateToken(user.Email, userRoles);

                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo,
                            roles = userRoles
                        };

                        return Created("", results);
                    }
                }
                else
                {
                    return Forbid();
                }
            }

            return BadRequest();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RegisterUser([FromBody] UserViewModel userViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var storeUser = _mapper.Map<UserViewModel, StoreUserExtended>(userViewModel);

                    var result = await _userManager.CreateAsync(storeUser, userViewModel.Password);                    

                    var addToRoleResult = await _userManager.AddToRoleAsync(storeUser, "StandardUser");

                    if (result.Succeeded && addToRoleResult.Succeeded)
                    {
                        var user = await _userManager.FindByEmailAsync(userViewModel.Email);
                        var userRoles = await _userManager.GetRolesAsync(user);
                        var token = CreateToken(user.Email, userRoles);

                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo,
                            roles = userRoles
                        };

                        return Created("", results);
                    }
                    else
                    {
                        return BadRequest("Cannot register new user");
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Register user failed {0}", ex);
                return BadRequest(ex);
            }
        }


        private JwtSecurityToken CreateToken(string email, IList<string> userRoles)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(ClaimTypes.Name, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                _config["Tokens:Issuer"],
                _config["Tokens:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds);            
        }
    }
}
