using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Azure.ClientObjects
{
    public class ViewSupportProcedure
    {
        public int PatientProcedureId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string ProcedureName { get; set; }
    }
}