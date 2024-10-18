using MedicalScheduleAPI.DTO;
using MedicalScheduleAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace MedicalScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly MedicalScheduleContext _context;

        public PatientController(MedicalScheduleContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetPatients()
        {
            try
            {
                var patient = _context.Patients
                    .Select(a => new PatientDTO
                    {
                        PatientId = a.PatientId,
                        UserId = a.UserId,
                        FullName = a.FullName,
                        DateOfBirth = a.DateOfBirth,
                        Gender = a.Gender,
                        Address = a.Address,
                        Email = a.Email,
                        Phone = a.Phone,
                    })
                    .ToList();

                return Ok(patient);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
            }
        }

        [HttpGet("GetPatient/{id}")]
        public IActionResult GetPatient(int id)
        {
            var patient = _context.Patients.FirstOrDefault(d => d.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }
            var patients = new PatientDTO
            {
                PatientId = patient.PatientId,
                UserId = patient.UserId,
                FullName = patient.FullName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Address = patient.Address,
                Email = patient.Email,
                Phone = patient.Phone,
            };
            return Ok(patients);
        }
        [HttpGet("GetPatientByUserId/{id}")]
        public IActionResult GetPatientByUserId(int id)
        {
            var patient = _context.Patients.FirstOrDefault(d => d.UserId == id);
            if (patient == null)
            {
                return NotFound();
            }
            var patients = new PatientDTO
            {
                PatientId = patient.PatientId,
                UserId = patient.UserId,
                FullName = patient.FullName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Address = patient.Address,
                Email = patient.Email,
                Phone = patient.Phone,
            };
            return Ok(patients);
        }
        [HttpGet("History/{patientId}")]
        public ActionResult<IEnumerable<PatientDTOs>> GetHistoryByPatientId(int patientId)
        {
            var history = _context.MedicalRecords.Include(x => x.Prescriptions)
                .Where(m => m.PatientId == patientId)
                .SelectMany(mr => mr.Prescriptions.Select(p => new PatientDTOs
                {
                    PatientName = mr.Patient.FullName,
                    RecordDate = mr.RecordDate,
                    MedicalHistory = mr.MedicalHistory,
                    MedicationName = p.MedicationName,
                    Dosage = p.Dosage,
                    Frequency = p.Frequency,
                    Instructions = p.Instructions,
                }))
                .ToList();

            if (history.Count == 0)
            {
                return NotFound();
            }

            return history;
        }

        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<PatientDTO>>> SearchPatients(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return BadRequest("Keyword cannot be empty");
                }

                var patients = await _context.Patients
                    .Where(p => p.FullName.Contains(keyword) ||
                                p.Gender.Contains(keyword) ||
                                p.Email.Contains(keyword) ||
                                p.Address.Contains(keyword) ||
                                p.Phone.Contains(keyword))
                    .Select(p => new PatientDTO
                    {
                        PatientId = p.PatientId,
                        UserId = p.UserId,
                        FullName = p.FullName,
                        DateOfBirth = p.DateOfBirth,
                        Gender = p.Gender,
                        Address = p.Address,
                        Email = p.Email,
                        Phone = p.Phone
                    })
                    .ToListAsync();

                return Ok(patients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("CheckPatientExist/{id}")]
        public IActionResult CheckPatientExist(int id)
        {
            var patient = _context.Patients.Where(x => x.UserId == id).FirstOrDefault();
            if (patient == null)
            {
                return NotFound();
            }
            var patients = new PatientDTO
            {
                PatientId = patient.PatientId,
                UserId = patient.UserId,
                FullName = patient.FullName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Address = patient.Address,
                Email = patient.Email,
                Phone = patient.Phone,
            };
            return Ok(patients);
        }

        [HttpPost("CreatePatient")]

        public IActionResult CreatePatient([FromBody] PatientDTOPost patientDTO)
        {
            try
            {
                if (_context.Patients.Any(x => x.FullName == patientDTO.FullName))
                {
                    return BadRequest(new { message = "Patient already exists" });
                }


                var patient = new Patient
                {
                    UserId = patientDTO.UserId,
                    FullName = patientDTO.FullName,
                    DateOfBirth = patientDTO.DateOfBirth,
                    Gender = patientDTO.Gender,
                    Address = patientDTO.Address,
                    Email = patientDTO.Email,
                    Phone = patientDTO.Phone,
                };

                _context.Patients.Add(patient);
                _context.SaveChanges();

                return CreatedAtAction(nameof(GetPatient), new { id = patient.PatientId }, patient);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
            }

        }

        [HttpPut("UpdatePatient/{id}")]
        public IActionResult UpdatePatient(int id, [FromBody] PatientDTOPut patientDTOPut)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var patient = _context.Patients.FirstOrDefault(d => d.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            patient.FullName = patientDTOPut.FullName;
            patient.DateOfBirth = patientDTOPut.DateOfBirth;
            patient.Gender = patientDTOPut.Gender;
            patient.Address = patientDTOPut.Address;
            patient.Email = patientDTOPut.Email;
            patient.Phone = patientDTOPut.Phone;

            _context.SaveChanges();

            return Ok(patient);
        }

        [HttpDelete("DeletePatient/{id}")]
        public IActionResult DeletePatient(int id)
        {
            var patient = _context.Patients.Include(x => x.Appointments).Include(x => x.MedicalRecords).FirstOrDefault(d => d.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }
            _context.Appointments.RemoveRange(patient.Appointments);
            _context.MedicalRecords.RemoveRange(patient.MedicalRecords);
            _context.Patients.Remove(patient);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
