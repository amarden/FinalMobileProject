using Azure.ClientObjects;
using Azure.DataObjects;
using Azure.Temporary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Azure.Controllers
{
    public class LoginController : ApiController
    {
        [HttpGet]
        public User GetUser()
        {
            return FakeUser.getUser();
        }
    }
}
