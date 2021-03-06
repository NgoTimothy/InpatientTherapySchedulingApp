using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InpatientTherapySchedulingProgram.Models;
using InpatientTherapySchedulingProgram.Exceptions.PatientExceptions;
using InpatientTherapySchedulingProgram.Services.Interfaces;


namespace InpatientTherapySchedulingProgram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _service;

        public PatientController(IPatientService service)
        {
            _service = service;
        }

        // GET: api/Patient
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatient()
        {
            var allPatients = await _service.GetAllPatients();

            return Ok(allPatients);
        }

        // GET: api/Patient/5
        [HttpGet("{patientId}")]
        public async Task<ActionResult<Patient>> GetPatient(int patientId)
        {
            var patient = await _service.GetPatientByPatientId(patientId);

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        // PUT: api/Patient/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{patientId}")]
        public async Task<IActionResult> PutPatient(int patientId, Patient patient)
        {
            try
            {
                await _service.UpdatePatient(patientId, patient);
            }
            catch (PatientPidsDoNotMatchException e)
            {
                return BadRequest(e);
            }
            catch (PatientDoesNotExistException e)
            {
                return BadRequest(e);
            }
            catch (DbUpdateConcurrencyException) {
                throw;
            }

            return NoContent();
        }

        // POST: api/Patient
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(Patient patient)
        {
            try
            {
                await _service.AddPatient(patient);
            }
            catch (PatientIdAlreadyExistException e)
            {
                return Conflict(e);
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return CreatedAtAction("GetPatient", new { id = patient.PatientId }, patient);
        }

        // DELETE: api/Patient/5
        [HttpDelete("{patientId}")]
        public async Task<ActionResult<Patient>> DeletePatient(int patientId)
        {
            Patient patient;

            try
            {
                patient = await _service.DeletePatient(patientId);
            }
            catch (DbUpdateConcurrencyException) {
                throw;
            }

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }
    }
}
