using BarbershopVVSS.EntityLayer;
using BarbershopVVSS.ServiceLayer;

using System;
using System.Collections.Generic;
using BarbershopVVSS.DomainLayer;
using BarbershopVVSS.EntityLayer;

namespace BarbershopVVSS.ServiceLayer
{
    internal class AppointmentService
    {
        private readonly AppointmentDomain _domain;

        public AppointmentService(AppointmentDomain domain)
        {
            _domain = domain ?? throw new ArgumentNullException(nameof(domain));
        }

        public void CreateAppointment(string customerEmail, string barberEmail, DateTime appointmentDate, string serviceType)
        {
            var appt = new Appointments
            {
                CustomerEmail = customerEmail,
                BarberEmail = barberEmail,
                AppointmentDate = appointmentDate,
                ServiceType = serviceType
            };

            _domain.Create(appt);
        }

        public List<Appointments> GetAppointmentsForCustomer(string customerEmail)
        {
            return _domain.GetByCustomerEmail(customerEmail);
        }

        public List<Appointments> GetAppointmentsForBarber(string barberEmail)
        {
            return _domain.GetByBarberEmail(barberEmail);
        }

        public void CancelAppointment(int appointmentId)
        {
            _domain.Cancel(appointmentId);
        }
    }
}