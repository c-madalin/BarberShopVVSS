using Barbershop.EntityLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Barbershop.DomainLayer
{
    public interface IAppointmentDomain
    {
        Task CreateAsync(Appointment appointment);
        Task<List<Appointment>> GetByCustomerEmailAsync(string email);
        Task<List<Appointment>> GetByBarberEmailAsync(string email);
        Task CancelAsync(int id);
    }
}