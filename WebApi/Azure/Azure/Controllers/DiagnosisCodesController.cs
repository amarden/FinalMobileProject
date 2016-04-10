using Azure.DataObjects;
using Azure.EhrAssets;
using Azure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Azure.Controllers
{
    public class DiagnosisCodesController : ApiController
    {
        [HttpGet]
        public List<DiagnosisCode> Get()
        {
            using (var db = new DataContext())
            {
                return db.DiagnosisCodes.ToList();
            }
        }

        [HttpGet]
        public List<Patient> Get(int howMany)
        {
            EHR ehr = new EHR();
            ehr.CreateNewPatients(howMany);
            using (var db = new DataContext())
            {
                var patients = db.Patients
                    .Include("DiagnosisCode")
                    .Include("Biometrics")
                    .Include("PatientChatLogs")
                    .Include("PatientToDos")
                    .Include("PatientProviders")
                    .Include("PatientProcedures")
                    .Include("PatientImagings")
                    .Include("Biometrics")
                    .ToList();
                return patients;
            }
        }
    }
}
