using Authorization_and_Authentication_WEB_API.Authentication;
using Authorization_and_Authentication_WEB_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Authorization_and_Authentication_WEB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        //Constructor Injection
        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        //User Registration  method
        [HttpPost]
        [Route("Registration")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExist = await _userManager.FindByNameAsync(model.UserName);
            if(userExist!=null)
             return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already Exist" });
            ApplicationUser user = new ApplicationUser()
            {
                
                
                Email=model.Email,
                SecurityStamp=Guid.NewGuid().ToString(),
                UserName=model.UserName,
            };
            var result= await _userManager.CreateAsync(user, model.Password);
           
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Registration Creation Failed" });
            }
            return Ok(new Response { Status = "Success", Message = "Registration completed successfully" });

        }
        [HttpPost]
        [Route("Login")]
        //UserLogin Method
        public async Task<IActionResult>Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if(user!=null && await  _userManager.CheckPasswordAsync(user,model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
               
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),

                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddMinutes(15),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha256)
                    );
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),


                });

            }
            return Unauthorized();
        }
    }
}
