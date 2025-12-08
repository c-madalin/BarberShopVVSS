using Barbershop.EntityLayer.Enums;
using System;

namespace Barbershop.EntityLayer
{
    public class Appointment
    {
        public int AppointmentID { get; set; }

        public string CustomerEmail { get; set; }
        public string BarberEmail { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string ServiceType { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        public string BarberName { get; set; }
        public string ClientName { get; set; }
    }
}