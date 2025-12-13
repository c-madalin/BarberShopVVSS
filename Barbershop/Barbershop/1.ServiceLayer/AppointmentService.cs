using Barbershop.DomainLayer;
using Barbershop.EntityLayer;
using System;
using System.Collections.Generic;

namespace Barbershop.ServiceLayer
{
    public sealed class AppointmentService: IAppointmentService
    {
        private readonly IAppointmentDomain _domain;

        public AppointmentService(IAppointmentDomain domain)
        {
            _domain = domain;
        }

        public void CreateAppointment(string customerEmail, string barberEmail, DateTime date, string serviceType)
        {
            var appt = new Appointment
            {
                CustomerEmail = customerEmail,
                BarberEmail = barberEmail,
                AppointmentDate = date,
                ServiceType = serviceType
            };

            _domain.Create(appt);
        }

        public List<Appointment> GetHistoryClient(string email)
        {
            return _domain.GetByCustomerEmail(email);
        }

        public List<Appointment> GetHistoryBarber(string email)
        {
            return _domain.GetByBarberEmail(email);
        }

        public void Cancel(int id)
        {
            _domain.Cancel(id);
        }
    }
}