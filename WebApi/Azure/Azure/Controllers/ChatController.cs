using AutoMapper;
using Azure.ClientObjects;
using Azure.DataObjects;
using Azure.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper.QueryableExtensions;

namespace Azure.Controllers
{
    public class ChatController : ApiController
    {
        private DataContext db = new DataContext();

        [HttpGet]
        public List<ViewChatLog> Get(int patientId)
        {
            var config = new MapperConfiguration(cfg =>
             cfg.CreateMap<PatientChatLog, ViewChatLog>()
             .ForMember(dto => dto.ProviderName, conf => conf.MapFrom(ol => ol.Provider.Name)));

            return db.PatientChatLogs
                .Where(x => x.PatientId == patientId)
                .OrderBy(x => x.Created)
                .ProjectTo<ViewChatLog>(config)
                .ToList();
        }

        [HttpPost]
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
