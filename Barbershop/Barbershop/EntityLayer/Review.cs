using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.EntityLayer
{
    public sealed class Review
    {
        public int ReviewId { get; set; }

        public int AppointmentId { get; set; }
        public string ClientEmail { get; set; }

        public string BarberEmail { get; set; }

        public int Rating { get; set; } 
        public string Comment { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.Now;
    }
}