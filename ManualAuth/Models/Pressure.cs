using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManualAuth.Models
{
    public class Pressure
    {

        [Key]
        public int Id { get; set; }

        public string Id_patient { get; set; }

        [Required]
        public DateTime PressureDateTime { get; set; }

        [Required]
        public int PressureSYSValue { get; set; }

        [Required]
        public int PressureDIAValue { get; set; }

    }

}
