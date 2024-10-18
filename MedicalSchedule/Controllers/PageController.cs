using MedicalScheduleAPI.DTO;
using MedicalScheduleAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {

        private readonly MedicalScheduleContext _context;
        public PageController(MedicalScheduleContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetPage(string objectType, int page, int pageSize)
        {
            // Khai báo biến để lưu trữ kết quả trả về
            object result = null;

            // Kiểm tra loại đối tượng và truy vấn tương ứng từ DbContext
            switch (objectType)
            {
                case "Account":
                    var accounts = await _context.Accounts
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
                    result = accounts.Select(a => new { UserId = a.UserId, Username = a.Username, Password = a.Password, UserType = a.UserType }).ToList();
                    break; // Cần có lệnh break để kết thúc nhánh case
                case "Patient":
                    var patients = await _context.Patients
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
                    result = patients.Select(a => new
                    {
                        PatientId = a.PatientId,
                        UserId = a.UserId,
                        FullName = a.FullName,
                        DateOfBirth = a.DateOfBirth,
                        Gender = a.Gender,
                        Address = a.Address,
                        Email = a.Email,
                        Phone = a.Phone,
                    }).ToList();
                    break;
                case "Record":
                    var records = await _context.MedicalRecords
                        .Include(x=>x.Patient)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
                    result = records.Select(a => new
                    {
                        Recordid = a.RecordId,
                        PatientName = a.Patient.FullName,
                        RecordDate = a.RecordDate,
                        MedicalHistory= a.MedicalHistory,
                    }).ToList();
                    break;
                default:
                    return BadRequest("Invalid object type.");
            }

            // Trả về kết quả
            return Ok(result);
        }

        [HttpGet("PageCount")]
        public async Task<ActionResult<int>> GetPageCount(string objectType, int pageSize)
        {
            try
            {
                int totalCount = 0;
                switch (objectType.ToLower())
                {
                    case "account":
                        totalCount = await _context.Accounts.CountAsync();
                        break;
                    // Add cases for other object types if needed
                    case "patient":
                        totalCount = await _context.Patients.CountAsync();
                        break;
                    case "apppointment":
                        totalCount = await _context.Appointments.CountAsync();
                        break;
                    case "record":
                        totalCount = await _context.MedicalRecords.CountAsync();
                        break;
                    case "apppointmentAccept":
                        totalCount = await _context.Appointments.Where(a => a.Status == "Accept" && !_context.MedicalRecords.Any(mr => mr.PatientId == a.PatientId)).CountAsync();
                        break;
                    default:
                        return BadRequest("Invalid object type");
                }

                int pageCount = (int)Math.Ceiling((double)totalCount / pageSize);
                return pageCount;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("Count")]
        public async Task<ActionResult<int>> GetCount(string objectType)
        {
            try
            {
                int totalCount = 0;
                switch (objectType.ToLower())
                {
                    case "account":
                        totalCount = await _context.Accounts.CountAsync();
                        break;
                    // Add cases for other object types if needed
                    case "patient":
                        totalCount = await _context.Patients.CountAsync();
                        break;
                    case "apppointment":
                        totalCount = await _context.Appointments.CountAsync();
                        break;
                    case "clinic":
                        totalCount = await _context.Appointments.Where(a => a.Status == "Accept" && !_context.MedicalRecords.Any(mr => mr.PatientId == a.PatientId)).CountAsync();
                        break;
                    default:
                        return BadRequest("Invalid object type");
                }

                return totalCount;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
