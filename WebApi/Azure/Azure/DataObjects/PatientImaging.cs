﻿using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Azure.DataObjects
{
    public class PatientImaging
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientImagingId{ get; set; }
        public int PatientId { get; set; }
        public string ImageType { get; set; }
        public string ImagePath { get; set; }
        public DateTime UploadDate { get; set; }
        [JsonIgnore]
        public virtual Patient Patient { get; set; }
    }
}
