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
    public class ProcedureCodeController : ApiController
    {
        private DataContext db = new DataContext();

        [HttpGet]
        public List<ProcedureCode> Get()
        {
            return db.ProcedureCodes.ToList();
        }

        [HttpGet]
        public List<ViewPatientProcedure> GetByPatient(int patientId)
        {
            var config = new MapperConfiguration(cfg =>
             cfg.CreateMap<PatientProcedure, ViewPatientProcedure>()
               .ForMember(dto => dto.Procedure, conf => conf.MapFrom(x => x.ProcedureCode.Procedure))
               .ForMember(dto => dto.CompletedTime, conf => conf.MapFrom(x=>x.CompletedTime))
               .ForMember(dto => dto.CompletedBy, conf => conf.MapFrom(x=>x.Provider.Name))
               .ForMember(dto => dto.procedureRole, conf => conf.MapFrom(x => x.ProcedureCode.Role))
            );

            var data = db.PatientProcedures.Where(x => x.PatientId == patientId).ProjectTo<ViewPatientProcedure>(config).ToList();
            data.ForEach(x => x.ShowRules.procedureRole = x.procedureRole);
            data.ForEach(x => x.ShowRules.Completed = x.Completed);
            return data;
        }

        [HttpGet]
        public List<ViewSupportProcedure> GetProcedureByRole(string role)
        {
            var config = new MapperConfiguration(cfg =>
             cfg.CreateMap<PatientProcedure, ViewSupportProcedure>()
               .ForMember(dto => dto.ProcedureName, conf => conf.MapFrom(x => x.ProcedureCode.Procedure))
               .ForMember(dto => dto.PatientName, conf => conf.MapFrom(x => x.Patient.Name))
               .ForMember(dto => dto.PatientId, conf => conf.MapFrom(x => x.Patient.PatientId))
            );

            var data = db.PatientProcedures.Where(x => x.ProcedureCode.Role == role && x.Completed == false)
                .ProjectTo<ViewSupportProcedure>(config).ToList();
            return data;
        }

        [HttpPost]
        public void Create(PatientProcedure procedure)
        {
            db.PatientProcedures.Add(procedure);
            db.SaveChanges();
        }

        [HttpPut]
        public void Complete(int patientProcedureId)
        {
            var providerId = FakeUser.getUser().Id;
            var patientProcedure = db.PatientProcedures.Where(x => x.PatientProcedureId == patientProcedureId).Single();
            patientProcedure.Completed = true;
            patientProcedure.ProviderId = providerId;
            patientProcedure.CompletedTime = DateTime.Now;
            db.Entry(patientProcedure).State = EntityState.Modified;
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
