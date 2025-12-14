using Barbershop.EntityLayer;
using System.Collections.Generic;

namespace Barbershop.ServiceLayer
{
    public interface IReviewService
    {
        void AddReview(int appointmentId, string clientEmail, string barberEmail, int rating, string comment);
        List<Review> GetReviewsForBarber(string barberEmail);
    }
}