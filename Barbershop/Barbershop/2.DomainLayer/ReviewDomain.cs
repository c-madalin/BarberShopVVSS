using Barbershop.EntityLayer;
using Barbershop.RepositoryLayer;
using Barbershop.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Barbershop.DomainLayer
{
    public sealed class ReviewDomain : IReviewDomain
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewDomain(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task AddReviewAsync(Review review)
        {
            if (review.Rating < 1 || review.Rating > 5)
            {
                AppLogger.Warn($"Review failed: Invalid rating {review.Rating}.");
                throw new ArgumentException("Rating must be between 1 and 5.");
            }

            if (review.Comment != null && review.Comment.Length > 500)
            {
                AppLogger.Warn($"Review failed: Comment too long.");
                throw new ArgumentException("Comment is too long (max 500 chars).");
            }

            // Verificare asincronă pentru duplicat
            bool alreadyReviewed = await _reviewRepository.HasReviewForAppointmentAsync(review.AppointmentId);
            if (alreadyReviewed)
            {
                AppLogger.Warn($"Review failed: Appointment {review.AppointmentId} already reviewed.");
                throw new InvalidOperationException("This appointment has already been reviewed.");
            }

            await _reviewRepository.AddAsync(review);
            AppLogger.Info($"Review added for Appointment {review.AppointmentId} by {review.ClientEmail}");
        }

        public async Task<List<Review>> GetReviewsByBarberAsync(string barberEmail)
        {
            if (string.IsNullOrEmpty(barberEmail))
                throw new ArgumentNullException(nameof(barberEmail));

            return await _reviewRepository.GetByBarberEmailAsync(barberEmail);
        }
    }
}