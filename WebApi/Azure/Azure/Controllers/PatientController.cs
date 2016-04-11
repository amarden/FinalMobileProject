﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.ClientObjects;
using Azure.DataObjects;
using Azure.EhrAssets;
using Azure.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Azure.Controllers
{
    public class PatientController : ApiController
    {
        private DataContext db = new DataContext();

        [HttpPost]
        public void CreatePatients(int howMany)
        {
            EHR ehr = new EHR();
            ehr.CreateNewPatients(howMany);
        }

        [HttpGet]
        public ViewPatient GetPatient(int patientID)
        {
            var config = new MapperConfiguration(cfg =>
             cfg.CreateMap<Patient, ViewPatient>()
                .ForMember(dto => dto.Diagnosis, conf => conf.MapFrom(x=>x.DiagnosisCode.Diagnosis)));

            return db.Patients
                .Include("PatientToDos")
                .Include("PatientProviders")
                .Include("PatientProcedures")
                .Include("PatientChatLogs")
                .Where(x=>x.PatientId == patientID)
                .ProjectTo<ViewPatient>(config).Single();
        }

        public List<Patient> GetAssignedPatients(int providerId)
        {
            return db.ProviderPatients
                .Where(x => x.ProviderId == providerId && x.Patient.MedicalStatus != "discharged" && x.Patient.MedicalStatus != "dead")
                .Select(x => x.Patient).ToList();
        }

        [HttpPut]
        public void AssignPatient(Patient p, int providerId)
        {
            PatientProvider pp = new PatientProvider();
            pp.PatientId = p.PatientId;
            pp.ProviderId = providerId;
            p.AssignDate = new DateTime();
            db.Entry(p).State = EntityState.Modified;
            db.SaveChanges();
        }

        [HttpPut]
        public void DischargePatient(int patientId)
        {
            var patient = db.Patients.Where(x => x.PatientId == patientId).Single();
            patient.MedicalStatus = "discharged";
            db.Entry(patient).State = EntityState.Modified;
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
