using MedicalScheduleAPI.DTO;
using MedicalScheduleAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;

namespace MedicalScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly MedicalScheduleContext _context;
        public AccountController(MedicalScheduleContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var accounts = _context.Accounts
                    .Select(a => new AccountDTOs
                    {
                        UserId = a.UserId,
                        Username = a.Username,
                        Password = a.Password,
                        UserType = a.UserType
                    })
                    .ToList();

                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var account = _context.Accounts.FirstOrDefault(x => x.UserId == id);
            if (account == null)
            {
                return NotFound();
            }
            var accountDTO = new AccountDTOs
            {
                UserId = account.UserId,
                Username = account.Username,
                Password = account.Password,
                UserType = account.UserType
            };

            return Ok(accountDTO);
        }
        [HttpGet("CheckUsernameExists")]
        public async Task<ActionResult<bool>> CheckUsernameExists(string username)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Username == username);
            return account != null;
        }
        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<Account>>> SearchAccounts(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return BadRequest("Keyword is required.");
            }

            // Tìm kiếm tài khoản theo từ khóa trong Username hoặc UserType
            var accounts = await _context.Accounts
                .Where(a => a.Username.Contains(keyword) || a.UserType.Contains(keyword))
                .ToListAsync();

            return accounts;
        }
        [HttpPost("CreateAccount")]
        public IActionResult CreateAccounnt([FromBody] AccountDTOPost accountDTO)
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
                    UserType = accountDTO.UserType,
                };

                _context.Accounts.Add(account);
                _context.SaveChanges();

                return CreatedAtAction(nameof(GetById), new { id = account.UserId }, account);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
            }

        }
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var account = _context.Accounts.Include(x => x.Doctors).Include(x => x.Patients).FirstOrDefault(x => x.UserId == id);
                if (account == null)
                {
                    return NotFound();
                }
                _context.Doctors.RemoveRange(account.Doctors);
                _context.Patients.RemoveRange(account.Patients);
                _context.Accounts.Remove(account);
                _context.SaveChanges();

                return NoContent();
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
