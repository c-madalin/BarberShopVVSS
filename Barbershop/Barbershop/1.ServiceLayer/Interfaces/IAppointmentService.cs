using Barbershop.EntityLayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Barbershop.ServiceLayer
{
    public interface IAppointmentService
    {
        Task CreateAppointmentAsync(string customerEmail, string barberEmail, DateTime date, string serviceType);
        Task<List<Appointment>> GetHistoryClientAsync(string email);
        Task<List<Appointment>> GetHistoryBarberAsync(string email);
        Task CancelAsync(int id);
    }
}