using Barbershop.DomainLayer;
using Barbershop.EntityLayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Barbershop.ServiceLayer
{
    public sealed class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentDomain _domain;

        public AppointmentService(IAppointmentDomain domain)
        {
            _domain = domain;
        }

        public async Task CreateAppointmentAsync(string customerEmail, string barberEmail, DateTime date, string serviceType)
        {
            var appt = new Appointment
            {
                CustomerEmail = customerEmail,
                BarberEmail = barberEmail,
                AppointmentDate = date,
                ServiceType = serviceType
            };

            await _domain.CreateAsync(appt);
        }

        public async Task<List<Appointment>> GetHistoryClientAsync(string email)
        {
            return await _domain.GetByCustomerEmailAsync(email);
        }

        public async Task<List<Appointment>> GetHistoryBarberAsync(string email)
        {
            return await _domain.GetByBarberEmailAsync(email);
        }

        public async Task CancelAsync(int id)
        {
            await _domain.CancelAsync(id);
        }
    }
}