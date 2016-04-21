using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.ClientObjects;
using Azure.DataObjects;
using Azure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Azure.Controllers
{
    public class AssignmentController : ApiController
    {
        private DataContext db = new DataContext();

        [HttpPost]
        public void Create(PatientProvider pp)
        {
            db.PatientProviders.Add(pp);
            db.SaveChanges();
        }

        [HttpDelete]
        public void Delete(int patientProviderId)
        {
            var pp = db.PatientProviders.Where(x => x.ProviderPatientId == patientProviderId).Single();
            db.PatientProviders.Remove(pp);
            db.SaveChanges();
        }

        [HttpGet]
        public List<ViewPatientProvider> Get(int patientId)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PatientProvider, ViewPatientProvider>()
                   .ForMember(dto => dto.Name, conf => conf.MapFrom(x => x.Provider.Name))
                   .ForMember(dto => dto.Role, conf => conf.MapFrom(x => x.Provider.Role));
            });

            var pp = db.PatientProviders.Where(x => x.PatientId == patientId && x.Active == true).ProjectTo<ViewPatientProvider>(config);
            return pp.ToList();
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
