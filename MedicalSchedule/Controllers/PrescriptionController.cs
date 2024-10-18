using MedicalScheduleAPI.DTO;
using MedicalScheduleAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace MedicalScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly MedicalScheduleContext _context;

        public PrescriptionController(MedicalScheduleContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetPrescription()
        {
            try
            {
                var pres = _context.Prescriptions
                    .Include(p => p.Record).ThenInclude(a => a.Patient)
                    .Select(a => new PrescriptionDTO
                    {
                        PrescriptionId = a.PrescriptionId,
                        PatientName = a.Record.Patient.FullName,
                        MedicationName = a.MedicationName,
                        Dosage = a.Dosage,
                        Frequency = a.Frequency,
                        Instructions = a.Instructions,
                    })
                    .ToList();

                return Ok(pres);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
            }
        }

        [HttpGet("GetPrescription/{id}")]
        public IActionResult GetPrescription(int id)
        {
            var pres = _context.Prescriptions
                .Include(p => p.Record).ThenInclude(a => a.Patient)
                .FirstOrDefault(d => d.PrescriptionId == id);
            if (pres == null)
            {
                return NotFound();
            }
            var result = new PrescriptionDTO
            {
                PrescriptionId = pres.PrescriptionId,
                PatientName = pres.Record.Patient.FullName,
                MedicationName = pres.MedicationName,
                Dosage = pres.Dosage,
                Frequency = pres.Frequency,
                Instructions = pres.Instructions,
            };
            return Ok(result);
        }
        [HttpGet("GetPrescriptionByRecordId/{id}")]
        public IActionResult GetPrescriptionByRecordId(int id)
        {
            var pres = _context.Prescriptions
                .Include(p => p.Record).ThenInclude(a => a.Patient)
                .FirstOrDefault(d => d.RecordId == id);
            if (pres == null)
            {
                return NotFound();
            }
            var result = new PrescriptionDTO
            {
                PrescriptionId = pres.PrescriptionId,
                PatientName = pres.Record.Patient.FullName,
                MedicationName = pres.MedicationName,
                Dosage = pres.Dosage,
                Frequency = pres.Frequency,
                Instructions = pres.Instructions,
            };
            return Ok(result);
        }

        [HttpPost("CreatePrescription")]

        public IActionResult CreatePrescription([FromBody] PrescriptionDTOPost presDTO)
        {
            try
            {
                var pres = new Prescription
                {
                    RecordId = presDTO.RecordId,
                    MedicationName = presDTO.MedicationName,
                    Dosage = presDTO.Dosage,
                    Frequency = presDTO.Frequency,
                    Instructions = presDTO.Instructions,
                };

                _context.Prescriptions.Add(pres);
                _context.SaveChanges();

                return Ok(pres);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
            }

        }

        [HttpPut("UpdatePrescription/{id}")]
        public IActionResult UpdatePrescription(int id, [FromBody] PrescriptionDTOPut presDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pres = _context.Prescriptions.FirstOrDefault(d => d.PrescriptionId == id);
            if (pres == null)
            {
                return NotFound();
            }

            pres.MedicationName = presDTO.MedicationName;
            pres.Dosage = presDTO.Dosage;
            pres.Frequency = presDTO.Frequency;
            pres.Instructions = presDTO.Instructions;

            _context.SaveChanges();

            return Ok(pres);
        }

        [HttpDelete("DeletePrescription/{id}")]
        public IActionResult DeletePrescription(int id)
        {
            var pres = _context.Prescriptions.FirstOrDefault(d => d.PrescriptionId == id);
            if (pres == null)
            {
                return NotFound();
            }
            _context.Prescriptions.Remove(pres);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
