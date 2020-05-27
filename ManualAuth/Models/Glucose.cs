using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManualAuth.Models
{
    public class Glucose
    {
        [Key]
        public int Id { get; set; }
        public string Id_patient { get; set; }

        [Required]
        public DateTime GlucoseDateTime { get; set; }

        [Required]
        public int GlucoseValue { get; set; }
    }
}
