using Barbershop.EntityLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Barbershop.ServiceLayer
{
    public interface IReviewService
    {
        Task AddReviewAsync(int appointmentId, string clientEmail, string barberEmail, int rating, string comment);
        Task<List<Review>> GetReviewsForBarberAsync(string barberEmail);
    }
}