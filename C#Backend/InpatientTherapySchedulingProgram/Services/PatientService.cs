﻿using InpatientTherapySchedulingProgram.Models;
using InpatientTherapySchedulingProgram.Services.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using InpatientTherapySchedulingProgram.Exceptions.PatientExceptions;

namespace InpatientTherapySchedulingProgram.Services
{
    public class PatientService : IPatientService
    {
        private readonly CoreDbContext _context;

        public PatientService(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<Patient> AddPatient(Patient patient)
        {
            if (await PatientExists(patient.PatientId)) {
                throw new PidAlreadyExistsException();
            }

            patient.Active = true;

            _context.Patient.Add(patient);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return patient;
        }

        public async Task<Patient> DeletePatient(int pid)
        {
            var patient = await _context.Patient.FindAsync(pid);

            if (patient == null)
            {
                return null;
            }

            _context.Patient.Remove(patient);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return patient;
        }

        public async Task<IEnumerable<Patient>> GetAllPatients()
        {
            return await _context.Patient.ToListAsync();
        }

        public async Task<Patient> GetPatientByPid(int pid)
        {
            return await _context.Patient.FindAsync(pid);
        }

        public async Task<Patient> UpdatePatient(int pid, Patient patient)
        {
            if (pid != patient.PatientId)
            {
                throw new PatientPidsDoNotMatchException();
            }
            if (!await PatientExists(pid))
            {
                throw new PatientDoesNotExistException();
            }

            var local = _context.Set<Patient>()
                .Local
                .FirstOrDefault(p => p.PatientId == patient.PatientId);

            _context.Entry(local).State = EntityState.Detached;

            _context.Entry(patient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return patient;
        }

        private async Task<bool> PatientExists(int patientId)
        {
            return await _context.Patient.FindAsync(patientId) != null;
        }
    }
}
