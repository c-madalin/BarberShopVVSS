using Barbershop.EntityLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Barbershop.RepositoryLayer
{
    public interface IAppointmentRepository
    {
        Task AddAsync(Appointment appointment);
        Task<List<Appointment>> GetByCustomerEmailAsync(string email);
        Task<List<Appointment>> GetByBarberEmailAsync(string email);
        Task DeleteByIdAsync(int id);

        Task<Appointment?> GetByIdAsync(int id);

    }
}