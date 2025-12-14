using Barbershop.DomainLayer;
using Barbershop.EntityLayer;
using System;
using System.Collections.Generic;

namespace Barbershop.ServiceLayer
{
    public sealed class ReviewService : IReviewService
    {
        private readonly IReviewDomain _reviewDomain;

        public ReviewService(IReviewDomain reviewDomain)
        {
            _reviewDomain = reviewDomain;
        }

        public void AddReview(int appointmentId, string clientEmail, string barberEmail, int rating, string comment)
        {
            if (appointmentId <= 0)
                throw new ArgumentException("Invalid Appointment ID.", nameof(appointmentId));

            if (string.IsNullOrWhiteSpace(clientEmail))
                throw new ArgumentException("Client email is required.", nameof(clientEmail));

            if (string.IsNullOrWhiteSpace(barberEmail))
                throw new ArgumentException("Barber email is required.", nameof(barberEmail));

            if (rating < 1 || rating > 5)
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");

            var review = new Review
            {
                AppointmentId = appointmentId,
                ClientEmail = clientEmail,
                BarberEmail = barberEmail,
                Rating = rating,
                Comment = comment?.Trim(), 
                DatePosted = DateTime.Now
            };

            _reviewDomain.AddReview(review);
        }

        public List<Review> GetReviewsForBarber(string barberEmail)
        {
            if (string.IsNullOrWhiteSpace(barberEmail))
                throw new ArgumentException("Barber email is required.", nameof(barberEmail));

            return _reviewDomain.GetReviewsByBarber(barberEmail);
        }
    }
}