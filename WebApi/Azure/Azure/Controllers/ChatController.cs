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
    public class ChatController : ApiController
    {
        private DataContext db = new DataContext();

        [HttpGet]
        public List<ViewChatLog> Get(int patientId)
        {
            return db.PatientChatLogs.Where(x => x.PatientId == patientId).OrderBy(x => x.Created).ToList();
        }

        [HttpGet]
        public void Post(PatientChatLog message)
        {
            db.PatientChatLogs.Add(message);
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
