using Barbershop.EntityLayer;
using System.Collections.Generic;

namespace Barbershop.DomainLayer
{
    public interface IReviewDomain
    {
        void AddReview(Review review);
        List<Review> GetReviewsByBarber(string barberEmail);
    }
}