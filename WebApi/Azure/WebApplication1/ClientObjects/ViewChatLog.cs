using Azure.DataObjects;
using System;
using System.Collections.Generic;

namespace Azure.ClientObjects
{
    public class ViewChatLog
    {
        public string ProviderName { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; }
    }
}