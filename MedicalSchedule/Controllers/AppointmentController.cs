using MedicalScheduleAPI.DTO;
using MedicalScheduleAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace MedicalScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly MedicalScheduleContext _context;

        public AppointmentController(MedicalScheduleContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAppointments()
        {
            try
            {
                var apoi = _context.Appointments
                    .Select(a => new AppointmentDTO
                    {
                        AppointmentId = a.AppointmentId,
                        PatientName = a.Patient.FullName,
                        DoctorName = a.Doctor.FullName,
                        AppointmentDateTime = a.AppointmentDateTime,
                        Notes = a.Notes,
                        Status = a.Status,
                    })
                    .ToList();

                return Ok(apoi);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
            }
        }

        [HttpGet("GetAppointment/{id}")]
        public IActionResult GetAppointment(int id)
        {
            var apoi = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefault(d => d.AppointmentId == id);
            if (apoi == null)
            {
                return NotFound();
            }
            var apois = new AppointmentDTO
            {
                AppointmentId = apoi.AppointmentId,
                PatientName = apoi.Patient != null ? apoi.Patient.FullName : "Unknown",
                DoctorName = apoi.Doctor != null ? apoi.Doctor.FullName : "Unknown",
                AppointmentDateTime = apoi.AppointmentDateTime,
                Notes = apoi.Notes,
                Status = apoi.Status,
            };
            return Ok(apois);
        }
        [HttpGet("Search")]
        public IActionResult SearchAppointments(string? keyword)
        {
            var appointments = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a =>
                    string.IsNullOrEmpty(keyword) ||
                    a.Patient.FullName.Contains(keyword) ||
                    a.Doctor.FullName.Contains(keyword) ||
                    a.Status.Contains(keyword) ||
                    a.Notes.Contains(keyword)
                )
                .Select(a => new AppointmentDTO
                {
                    AppointmentId = a.AppointmentId,
                    PatientName = a.Patient != null ? a.Patient.FullName : "Unknown",
                    DoctorName = a.Doctor != null ? a.Doctor.FullName : "Unknown",
                    AppointmentDateTime = a.AppointmentDateTime,
                    Notes = a.Notes,
                    Status = a.Status
                })
                .ToList();

            return Ok(appointments);
        }

        [HttpPost("CreateAppointment")]

        public IActionResult CreateAppointment([FromBody] AppointmentDTOPost apoiDTO)
        {
            try
            {
                var apoi = new Appointment
                {
                    PatientId = apoiDTO.PatientId,
                    DoctorId = apoiDTO.DoctorId,
                    AppointmentDateTime = apoiDTO.AppointmentDateTime,
                    Notes = apoiDTO.Notes,
                    Status = apoiDTO.Status,
                };

                _context.Appointments.Add(apoi);
                _context.SaveChanges();

                return Ok(apoi);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
            }

        }

        [HttpPut("UpdateAppointment/{id}")]
        public IActionResult UpdateAppointment(int id, [FromBody] AppointmentDTOPut appointmentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var apoi = _context.Appointments.FirstOrDefault(d => d.AppointmentId == id);
            if (apoi == null)
            {
                return NotFound();
            }

            apoi.AppointmentDateTime = appointmentDTO.AppointmentDateTime;
            apoi.Notes = appointmentDTO.Notes;
            apoi.Status = appointmentDTO.Status;

            _context.SaveChanges();

            return Ok(apoi);
        }

        [HttpDelete("DeleteAppointment/{id}")]
        public IActionResult DeleteAppointment(int id)
        {
            var apoi = _context.Appointments.FirstOrDefault(d => d.AppointmentId == id);
            if (apoi == null)
            {
                return NotFound();
            }
            _context.Appointments.Remove(apoi);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpGet("GetAppointments")]
        public ActionResult GetAppointments(int page = 1, int pageSize = 10)
        {
            try
            {
                // Calculate the index of the first item on the requested page
                int startIndex = (page - 1) * pageSize;

                // Retrieve appointments for the requested page
                var appointments = _context.Appointments
                    .Select(a => new AppointmentDTO
                    {
                        AppointmentId = a.AppointmentId,
                        PatientName = a.Patient.FullName,
                        DoctorName = a.Doctor.FullName,
                        AppointmentDateTime = a.AppointmentDateTime,
                        Notes = a.Notes,
                        Status = a.Status,
                    })
                    .OrderBy(a => a.AppointmentDateTime)
                    .Skip(startIndex)
                    .Take(pageSize)
                    .ToList();

                // Calculate total number of appointments for pagination
                int totalAppointments = _context.Appointments.Count();
                int totalPages = (int)Math.Ceiling((double)totalAppointments / pageSize);

                // Return the retrieved appointments along with total pages information
                return Ok(new { data = appointments, totalPages });
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetAppointmentAccept")]
        public ActionResult GetAppointmentAccept(int page = 1, int pageSize = 10)
        {
            try
            {
                // Calculate the index of the first item on the requested page
                int startIndex = (page - 1) * pageSize;

                // Retrieve appointments for the requested page
                var appointments = _context.Appointments
                    .Where(a => a.Status == "Accept" && !_context.MedicalRecords.Any(mr => mr.PatientId == a.PatientId))
                    .Select(a => new AppointmentDTOs
                    {
                        AppointmentId = a.AppointmentId,
                        PatientId= a.PatientId,
                        PatientName = a.Patient.FullName,
                        DoctorName = a.Doctor.FullName,
                        AppointmentDateTime = a.AppointmentDateTime,
                        Notes = a.Notes,
                        Status = a.Status,
                    })
                    .OrderBy(a => a.AppointmentDateTime)
                    .Skip(startIndex)
                    .Take(pageSize)
                    .ToList();

                // Calculate total number of appointments for pagination
                int totalAppointments = _context.Appointments.Count();
                int totalPages = (int)Math.Ceiling((double)totalAppointments / pageSize);

                // Return the retrieved appointments along with total pages information
                return Ok(new { data = appointments, totalPages });
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
