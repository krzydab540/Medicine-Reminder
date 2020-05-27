using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace ManualAuth.Models
{
    public class Medicine
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        
        public string Id_patient { get; set; }        

        public string Id_doctor { get; set; }
 
        public DayOfWeek Day { get; set; }

        public int HourOfTaking { get; set; }

        public int MinuteOfTaking { get; set; }

    }
}
