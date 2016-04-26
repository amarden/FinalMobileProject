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
    public class ProviderController : ApiController
    {
        private DataContext db = new DataContext();

        private MapperConfiguration config = new MapperConfiguration(cfg =>
                cfg.CreateMap<Provider, ViewProvider>());

        [HttpGet]
        public List<ViewProvider> Get()
        {
            return db.Providers.ProjectTo<ViewProvider>(config).ToList();
        }

        [HttpGet]
        public List<ViewProvider> GetByRole(string role)
        {
            return db.Providers
                .Where(x=>x.Role == role)
                .ProjectTo<ViewProvider>(config).ToList();
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
