using System;
using System.Collections.Generic;
using System.Linq;

namespace Client.ClientObjects
{
    public class ViewDocument
    {
        public int PatientImagingId { get; set; }
        public string ImageType { get; set; }
        public string ImageName { get; set; }
        public DateTime UploadDate { get; set; }
    }
}