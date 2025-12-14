using Barbershop.EntityLayer;
using System.Collections.Generic;

namespace Barbershop.RepositoryLayer
{
    public interface IReviewRepository
    {
        void Add(Review review);
        List<Review> GetByBarberEmail(string email);
        Review GetById(int id);
        bool HasReviewForAppointment(int appointmentId);
    }
}