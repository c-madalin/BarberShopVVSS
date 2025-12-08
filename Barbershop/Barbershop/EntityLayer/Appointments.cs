using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbershopVVSS.EntityLayer
{
    public class Appointments
    {
        public int AppointmentID { get; set; }
        public string CustomerEmail { get; set; }
        public string BarberEmail { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string ServiceType { get; set; }
    }
}
