using Azure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    public class DashboardController : ApiController
    {
        private DataContext db = new DataContext();

        [HttpGet]
        public void ProcedureInfo()
        {
            return db.PatientProcedures.GroupBy(x => x.ProcedureCode.Procedure)
                .Select(x => new { procedure = x.Key, count = x.Count() });
        }

        [HttpGet]
        public void DiagnosisInfo()
        {
            return db.Patients.GroupBy(x => x.DiagnosisCode.Diagnosis)
                .Select(x => new { diagnosis = x.Key, count = x.Count() });
        }

        [HttpGet]
        public void PatientInfo()
        {
            return db.Patients;
        }

    }
}
