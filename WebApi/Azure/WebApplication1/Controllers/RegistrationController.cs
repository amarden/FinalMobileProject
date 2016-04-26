using Azure.AuthenticationHelpers;
using Azure.DataObjects;
using Azure.Models;
using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class RegistrationController : ApiController
    {
        private DataContext db = new DataContext();
        //Config Settings that we pass to our Credentials Class
        public MobileAppSettingsDictionary ConfigSettings => Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

        [HttpPost]
        public async Task<HttpResponseMessage> CreateUser(ProviderType type)
        {
            Credentials cred = new Credentials(User, ConfigSettings, Request);
            var userInfo = await cred.GetUserInfo();
            var potentialId = userInfo.Provider + ":" + userInfo.UserId;
            var exist = db.Providers.Where(x => x.TwitterUserId == potentialId);
            if (exist.Count() > 0)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, new
                {
                    Message = "User has already been Registered",
                });
            }
            Provider p = new Provider();
            p.Name = userInfo.Name;
            p.TwitterUserId = potentialId;
            p.Role = type.role;
            db.Providers.Add(p);
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Created);
        }

        /// <summary>
        /// disposes our context
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }

    public class ProviderType
    {
        public string role { get; set; }
    }
}
