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
    public class DocumentController : ApiController
    {
        private DataContext db = new DataContext();

        [HttpGet]
        public List<PatientImaging> GetMeasurements(int patientId)
        {
            return db.PatientImagings
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.UploadDate)
                .ToList();
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
