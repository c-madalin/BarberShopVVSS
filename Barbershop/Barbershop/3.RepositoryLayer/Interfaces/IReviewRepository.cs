using Barbershop.EntityLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Barbershop.RepositoryLayer
{
    public interface IReviewRepository
    {
        Task AddAsync(Review review);
        Task<List<Review>> GetByBarberEmailAsync(string email);
        Task<bool> HasReviewForAppointmentAsync(int appointmentId);
    }
}