using Barbershop.EntityLayer;
using Barbershop.RepositoryLayer;
using System;
using System.Collections.Generic;

namespace Barbershop.DomainLayer
{
    public sealed class ReviewDomain : IReviewDomain
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewDomain(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public void AddReview(Review review)
        {
            if (review.Rating < 1 || review.Rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5.");
            }

            if (review.Comment != null && review.Comment.Length > 500)
            {
                throw new ArgumentException("Comment is too long (max 500 chars).");
            }

            bool alreadyReviewed = _reviewRepository.HasReviewForAppointment(review.AppointmentId);
            if (alreadyReviewed)
            {
                throw new InvalidOperationException("This appointment has already been reviewed.");
            }

            _reviewRepository.Add(review);
        }

        public List<Review> GetReviewsByBarber(string barberEmail)
        {
            if (string.IsNullOrEmpty(barberEmail))
            {
                throw new ArgumentNullException(nameof(barberEmail), "Barber email cannot be null.");
            }

            return _reviewRepository.GetByBarberEmail(barberEmail);
        }
    }
}