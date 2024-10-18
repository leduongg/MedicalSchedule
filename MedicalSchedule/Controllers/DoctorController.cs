using MedicalScheduleAPI.DTO;
using MedicalScheduleAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MedicalScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly MedicalScheduleContext _context;

        public DoctorController(MedicalScheduleContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetDoctors()
        {
            try
            {
                var doctors = _context.Doctors
                    .Select(a => new DoctorDTO
                    {
                        DoctorId = a.DoctorId,
                        UserId = a.UserId,
                        FullName = a.FullName,
                        Specialty = a.Specialty,
                        Email = a.Email,
                        Phone = a.Phone,
                    })
                    .ToList();

                return Ok(doctors);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
            }
        }

        [HttpGet("GetDoctor/{id}")]
        public IActionResult GetDoctor(int id)
        {
            var doctor = _context.Doctors.FirstOrDefault(d => d.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }
            var doctors = new DoctorDTO
            {
                DoctorId = doctor.DoctorId,
                UserId = doctor.UserId,
                FullName = doctor.FullName,
                Specialty = doctor.Specialty,
                Email = doctor.Email,
                Phone = doctor.Phone,
            };
            return Ok(doctors);
        }

        [HttpPost("CreateDoctor")]

        public IActionResult CreateDoctor([FromBody] DoctorDTO doctorDTO)
        {
            try
            {
                if (_context.Doctors.Any(x => x.FullName == doctorDTO.FullName))
                {
                    return BadRequest(new { message = "Doctor already exists" });
                }


                var doctor = new Doctor
                {
                    UserId = doctorDTO.UserId,
                    FullName = doctorDTO.FullName,
                    Specialty = doctorDTO.Specialty,
                    Email = doctorDTO.Email,
                    Phone = doctorDTO.Phone
                };

                _context.Doctors.Add(doctor);
                _context.SaveChanges();

                return CreatedAtAction(nameof(GetDoctor), new { id = doctor.DoctorId }, doctor);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
            }

        }

        [HttpPut("UpdateDoctor/{id}")]
        public IActionResult UpdateDoctor(int id, [FromBody] DoctorDTOPut doctorDTOPut)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var doctor = _context.Doctors.FirstOrDefault(d => d.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }

            doctor.FullName = doctorDTOPut.FullName;
            doctor.Specialty = doctorDTOPut.Specialty;
            doctor.Email = doctorDTOPut.Email;
            doctor.Phone = doctorDTOPut.Phone;

            _context.SaveChanges();

            return Ok(doctor);
        }

        [HttpDelete("DeleteDoctor/{id}")]
        public IActionResult DeleteDoctor(int id)
        {
            var doctor = _context.Doctors.Include(x => x.Appointments).FirstOrDefault(d => d.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }
            _context.Appointments.RemoveRange(doctor.Appointments);
            _context.Doctors.Remove(doctor);
            _context.SaveChanges();

            return NoContent();
        }
    }
}

