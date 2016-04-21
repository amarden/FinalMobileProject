using System;

namespace Client.ClientObjects
{
    public class ViewChatLog
    {
        public string ProviderName { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; }
    }
}