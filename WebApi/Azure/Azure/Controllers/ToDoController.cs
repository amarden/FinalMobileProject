using Azure.ClientObjects;
using Azure.DataObjects;
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
    public class ToDoController : ApiController
    {
        private DataContext db = new DataContext();

        [HttpGet]
        public List<ToDoItem> Get(int patientId)
        {
            return db.PatientToDos.Where(x => x.PatientId == patientId).OrderByDescending(x => x.Created).ToList();
        }

        [HttpGet]
        public List<ToDoItem> Get()
        {
            int providerId = 0;
            return db.ProviderPatients.Where(x => x.ProviderId == providerId).Select(x=>x.Patient).SelectMany(x=>x.PatientToDos);
        }

        [HttpPut]
        public void CompleteToDoItem(PatientToDo item)
        {
            item.Complete = true;
            item.CompleteDate = DateTime.Now;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
        }

        [HttpPost]
        public void CreateToDo(PatientToDo item)
        {
            item.Created = DateTime.Now;
            item.Complete = false;
            db.PatientToDos.Add(item);
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
