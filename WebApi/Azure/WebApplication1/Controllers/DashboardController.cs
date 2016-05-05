using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.DataObjects;
using Azure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.DashboardObjects;

namespace WebApplication1.Controllers
{
    public class DashboardController : ApiController
    {
        private DataContext db = new DataContext();

        private IEnumerable<PatientDashboard> PatientInfo()
        {
            var config = new MapperConfiguration(cfg =>
              cfg.CreateMap<Patient, PatientDashboard>()
                .ForMember(dto => dto.procedures, conf => conf.MapFrom(x => x.PatientProcedures))
                .ForMember(dto => dto.diagnosis, conf => conf.MapFrom(x => x.DiagnosisCode.Diagnosis))
                .ForMember(dto => dto.chatCount, conf => conf.MapFrom(x => x.PatientChatLogs.Count()))
                .ForMember(dto => dto.imageCount, conf => conf.MapFrom(x => x.PatientImagings.Count()))
                .ForMember(dto => dto.providerCount, conf => conf.MapFrom(x => x.PatientProviders.Where(c => c.Provider.Role != "Administrator").Count()))
             );

            return db.Patients.ProjectTo<PatientDashboard>(config);
        }

        private IEnumerable<ProviderDashboard> ProviderInfo()
        {
            var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Provider, ProviderDashboard>()
                        .ForMember(dto => dto.procedures, conf => conf.MapFrom(x => x.PatientProcedures))
                        .ForMember(dto => dto.patients, conf => conf.MapFrom(x => x.PatientProviders.Select(c => c.Patient)));


                    cfg.CreateMap<Patient, PatientSimple>()
                        .ForMember(dto => dto.diagnosis, conf => conf.MapFrom(x => x.DiagnosisCode.Diagnosis));
                }
            );

            return db.Providers.ProjectTo<ProviderDashboard>(config);
        }

        private IEnumerable<Item> GetDiagnoses()
        {
            return db.DiagnosisCodes.Select(x => new Item
            {
                id = x.DiagnosisCodeId,
                name = x.Diagnosis
            });
        }

        private IEnumerable<Item> GetProcedures()
        {
            return db.ProcedureCodes.Select(x => new Item
            {
                id = x.ProcedureCodeId,
                name = x.Procedure
            });
        }


        [HttpGet]
        public DataInfo GetMetrics()
        {
            return new DataInfo
            {
                diagnoses = GetDiagnoses(),
                patients = PatientInfo(),
                providers = ProviderInfo(),
                procedures = GetProcedures()

            };
        }

    }
}
