using Barbershop.EntityLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Barbershop.DomainLayer
{
    public interface IReviewDomain
    {
        Task AddReviewAsync(Review review);
        Task<List<Review>> GetReviewsByBarberAsync(string barberEmail);
    }
}