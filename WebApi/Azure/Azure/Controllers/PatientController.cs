using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.ClientObjects;
using Azure.DataObjects;
using Azure.EhrAssets;
using Azure.Models;
using Azure.Temporary;
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

        public class CustomResolver : ValueResolver<Patient, string>
        {
            protected override string ResolveCore(Patient patient)
            {
                return patient.AdmitDate.ToShortDateString();
            }
        }

        [HttpPost]
        public void CreatePatients()
        {
            EHR ehr = new EHR();
            ehr.CreateNewPatients(100);
        }

        [HttpGet]
        public ViewPatientDetail GetPatient(int patientID)
        {
             var config = new MapperConfiguration(cfg =>
             {
                 cfg.CreateMap<Patient, ViewPatientDetail>()
                    .ForMember(dto => dto.Diagnosis, conf => conf.MapFrom(x => x.DiagnosisCode.Diagnosis))
                    .ForMember(dto => dto.ProviderNumber, conf => conf.MapFrom(x => x.PatientProviders.Where(c => c.Active == true).Count()))
                    .ForMember(dto => dto.ChatActivityNumber, conf => conf.MapFrom(x => x.PatientChatLogs.Count()))
                    .ForMember(dto => dto.ImageNumber, conf => conf.MapFrom(x => x.PatientImagings.Count()))
                    .ForMember(dto => dto.ProcedureNumber, conf => conf.MapFrom(x => x.PatientProcedures.Where(c=>c.Completed == false).Count()));
             });

            return db.Patients
                .Include("PatientToDos")
                .Include("PatientProviders")
                .Include("PatientProcedures")
                .Include("PatientChatLogs")
                .Where(x=>x.PatientId == patientID)
                .ProjectTo<ViewPatientDetail>(config).Single();
        }

        [HttpGet]
        public List<ViewPatient> GetAssignedPatients()
        {
            var config = new MapperConfiguration(cfg =>
             cfg.CreateMap<Patient, ViewPatient>()
               .ForMember(dto => dto.Diagnosis, conf => conf.MapFrom(x => x.DiagnosisCode.Diagnosis))
               .ForMember(dto => dto.NumProvidersAssigned, conf => conf.MapFrom(x=>x.PatientProviders.Where(c=>c.Provider.Role != "Administrator").Count()))
               );

            var providerId = FakeUser.getUser().Id; 
            return db.PatientProviders
                .Where(x => x.ProviderId == providerId && x.Patient.MedicalStatus != "discharged" && x.Patient.MedicalStatus != "dead" && x.Active == true)
                .Select(x => x.Patient)
                .ProjectTo<ViewPatient>(config)
                .ToList();
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
