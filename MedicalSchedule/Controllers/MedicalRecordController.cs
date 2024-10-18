using MedicalScheduleAPI.DTO;
using MedicalScheduleAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly MedicalScheduleContext _context;

        public MedicalRecordController(MedicalScheduleContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetRecord()
        {
            try
            {
                var record = _context.MedicalRecords
                    .Select(a => new MedicalRecordDTO
                    {
                        RecordId = a.RecordId,
                        PatientName = a.Patient.FullName,
                        RecordDate = a.RecordDate,
                        MedicalHistory = a.MedicalHistory,
                    })
                    .ToList();

                return Ok(record);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
            }
        }

        [HttpGet("GetRecord/{id}")]
        public IActionResult GetRecord(int id)
        {
            var record = _context.MedicalRecords
                .Include(a => a.Patient)
                .FirstOrDefault(d => d.RecordId == id);
            if (record == null)
            {
                return NotFound();
            }
            var records = new MedicalRecordDTO
            {
                RecordId = record.RecordId,
                PatientName = record.Patient != null ? record.Patient.FullName : "Unknown",
                RecordDate = record.RecordDate,
                MedicalHistory = record.MedicalHistory,
            };
            return Ok(records);
        }


        [HttpPost("CreateRecord")]

        public IActionResult CreateRecord([FromBody] MedicalRecordDTOPost recordDTO)
        {
            try
            {
                var record = new MedicalRecord
                {
                    PatientId = recordDTO.PatientId,
                    RecordDate = recordDTO.RecordDate,
                    MedicalHistory = recordDTO.MedicalHistory,
                };

                _context.MedicalRecords.Add(record);
                _context.SaveChanges();

                return Ok(record);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
            }

        }
        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<MedicalRecordDTO>>> SearchRecord(string? keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return BadRequest("Keyword cannot be empty");
                }

                var records = await _context.MedicalRecords
                    .Include(x => x.Patient)
                    .Where(p => p.Patient.FullName.Contains(keyword) ||
                                p.MedicalHistory.Contains(keyword))
                    .Select(p => new MedicalRecordDTO
                    {
                        RecordId = p.RecordId,
                        PatientName = p.Patient != null ? p.Patient.FullName : "Unknown",
                        RecordDate = p.RecordDate,
                        MedicalHistory = p.MedicalHistory,
                    })
                    .ToListAsync();

                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPut("UpdateRecord/{id}")]
        public IActionResult UpdateRecord(int id, [FromBody] MedicalRecordDTOPut recordDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var record = _context.MedicalRecords.FirstOrDefault(d => d.RecordId == id);
            if (record == null)
            {
                return NotFound();
            }

            record.RecordDate = recordDTO.RecordDate;
            record.MedicalHistory = recordDTO.MedicalHistory;

            _context.SaveChanges();

            return Ok(record);
        }

        [HttpDelete("DeleteRecord/{id}")]
        public IActionResult DeleteRecord(int id)
        {
            var record = _context.MedicalRecords.Include(x=>x.Prescriptions).FirstOrDefault(d => d.RecordId == id);
            if (record == null)
            {
                return NotFound();
            }
            _context.Prescriptions.RemoveRange(record.Prescriptions);
            _context.MedicalRecords.Remove(record);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
