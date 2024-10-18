using MedicalScheduleAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace MedicalScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly MedicalScheduleContext _context;

        public ExportController(MedicalScheduleContext context)
        {
            _context = context;
        }

        [HttpGet("excel")]
        public IActionResult ExportToExcel()
        {
            try
            {
                List<ExportData> data = GetExportData();

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Medical Records");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Patient Name";
                    worksheet.Cells[1, 2].Value = "Date of Birth";
                    worksheet.Cells[1, 3].Value = "Gender";
                    worksheet.Cells[1, 4].Value = "Address";
                    worksheet.Cells[1, 5].Value = "Email";
                    worksheet.Cells[1, 6].Value = "Phone";
                    worksheet.Cells[1, 7].Value = "Appointment Date";
                    worksheet.Cells[1, 8].Value = "Doctor Name";
                    worksheet.Cells[1, 9].Value = "Notes";
                    worksheet.Cells[1, 10].Value = "Medical History";
                    worksheet.Cells[1, 11].Value = "Medication Name";
                    worksheet.Cells[1, 12].Value = "Dosage";
                    worksheet.Cells[1, 13].Value = "Frequency";
                    worksheet.Cells[1, 14].Value = "Instructions";

                    // Populate data to worksheet
                    for (int i = 0; i < data.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = data[i].FullName;
                        worksheet.Cells[i + 2, 2].Value = data[i].DateOfBirth.ToString("MM/dd/yyyy");
                        worksheet.Cells[i + 2, 3].Value = data[i].Gender;
                        worksheet.Cells[i + 2, 4].Value = data[i].Address;
                        worksheet.Cells[i + 2, 5].Value = data[i].Email;
                        worksheet.Cells[i + 2, 6].Value = data[i].Phone;
                        worksheet.Cells[i + 2, 7].Value = data[i].AppointmentDateTime.ToString("MM/dd/yyyy HH:mm");
                        worksheet.Cells[i + 2, 8].Value = data[i].DoctorName;
                        worksheet.Cells[i + 2, 9].Value = data[i].Notes;
                        worksheet.Cells[i + 2, 10].Value = data[i].MedicalHistory;
                        worksheet.Cells[i + 2, 11].Value = data[i].MedicationName;
                        worksheet.Cells[i + 2, 12].Value = data[i].Dosage;
                        worksheet.Cells[i + 2, 13].Value = data[i].Frequency;
                        worksheet.Cells[i + 2, 14].Value = data[i].Instructions;
                    }

                    // Convert Excel package to byte array
                    var fileBytes = package.GetAsByteArray();
                    var base64String = Convert.ToBase64String(fileBytes);
                    // Return Excel file as response
                    return Ok(new { excelData = base64String });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        private List<ExportData> GetExportData()
        {
            return (from p in _context.Patients
                    join a in _context.Appointments on p.PatientId equals a.PatientId
                    join r in _context.MedicalRecords on p.PatientId equals r.PatientId
                    join pr in _context.Prescriptions on r.RecordId equals pr.RecordId
                    select new ExportData
                    {
                        FullName = p.FullName,
                        DateOfBirth = p.DateOfBirth,
                        Gender = p.Gender,
                        Address = p.Address,
                        Email = p.Email,
                        Phone = p.Phone,
                        AppointmentDateTime = a.AppointmentDateTime,
                        DoctorName = a.Doctor.FullName,
                        Notes = a.Notes,
                        MedicalHistory = r.MedicalHistory,
                        MedicationName = pr.MedicationName,
                        Dosage = pr.Dosage,
                        Frequency = pr.Frequency,
                        Instructions = pr.Instructions
                    }).ToList();
        }
    }

    public class ExportData
    {
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string DoctorName { get; set; }
        public string Notes { get; set; }
        public string MedicalHistory { get; set; }
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Instructions { get; set; }
    }
}
