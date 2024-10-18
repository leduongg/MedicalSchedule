using MedicalScheduleAPI.DTO;
using MedicalScheduleAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System;
using System.Linq;

namespace MedicalScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly MedicalScheduleContext _context;

        IConfiguration config;

        public AuthenticationController(MedicalScheduleContext context, IConfiguration config)
        {
            _context = context;
            this.config = config;
        }
        [HttpPost("Login")]
        public IActionResult Login([FromBody] AccountDTO accountDTO)
        {
            var pass = MD5Hash(accountDTO.Password);
            var user = _context.Accounts.FirstOrDefault(x => x.Username == accountDTO.Username && x.Password == pass);
            if (user != null)
            {
                var key = config["Jwt:Key"];
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var signingCredential = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                    new Claim(ClaimTypes.Role,user.UserType)
                };
                var token = new JwtSecurityToken(
                    issuer: config["Jwt:Issuer"],
                    audience: config["Jwt:Audience"],
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: signingCredential,
                    claims: claims
                    );
                var newToken = new JwtSecurityTokenHandler().WriteToken(token);
                return new JsonResult(new { token = newToken, claims = claims });
            }
            else
            {
                return new JsonResult(new { message = "Login incorect" });
            }
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] AccountDTO accountDTO)
        {
            try
            {
                if (_context.Accounts.Any(x => x.Username == accountDTO.Username))
                {
                    return BadRequest(new { message = "Username already exists" });
                }

                if (string.IsNullOrEmpty(accountDTO.Password))
                {
                    return BadRequest(new { message = "Password is required" });
                }

                var account = new Account
                {
                    Username = accountDTO.Username,
                    Password = MD5Hash(accountDTO.Password),
                    UserType = "Patient",
                };

                _context.Accounts.Add(account);
                _context.SaveChanges();

                return Ok(new { message = "Registration successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
            }
        }

        [HttpPost("MD5Hash")]
        public string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
