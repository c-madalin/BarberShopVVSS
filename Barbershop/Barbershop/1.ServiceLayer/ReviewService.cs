using Barbershop.DomainLayer;
using Barbershop.EntityLayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Barbershop.ServiceLayer
{
    public sealed class ReviewService : IReviewService
    {
        private readonly IReviewDomain _reviewDomain;

        public ReviewService(IReviewDomain reviewDomain)
        {
            _reviewDomain = reviewDomain;
        }

        public async Task AddReviewAsync(int appointmentId, string clientEmail, string barberEmail, int rating, string comment)
        {
            if (appointmentId <= 0) throw new ArgumentException("Invalid Appointment ID.");
            if (string.IsNullOrWhiteSpace(clientEmail)) throw new ArgumentException("Client email required.");
            if (string.IsNullOrWhiteSpace(barberEmail)) throw new ArgumentException("Barber email required.");

            var review = new Review
            {
                AppointmentId = appointmentId,
                ClientEmail = clientEmail,
                BarberEmail = barberEmail,
                Rating = rating,
                Comment = comment?.Trim(),
                DatePosted = DateTime.Now
            };

            await _reviewDomain.AddReviewAsync(review);
        }

        public async Task<List<Review>> GetReviewsForBarberAsync(string barberEmail)
        {
            return await _reviewDomain.GetReviewsByBarberAsync(barberEmail);
        }
    }
}