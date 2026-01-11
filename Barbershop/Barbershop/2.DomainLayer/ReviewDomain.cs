using Barbershop.EntityLayer;
using Barbershop.RepositoryLayer;
using Barbershop.Utils.Exceptions; // Asigură-te că ai acest namespace pentru AppointmentNotFoundException
using Barbershop.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Barbershop.DomainLayer
{
    public sealed class ReviewDomain : IReviewDomain
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IAppointmentRepository _appointmentRepository; // Dependență nouă

        // Constructor cu 2 argumente (așa cum așteaptă testele)
        public ReviewDomain(IReviewRepository reviewRepository, IAppointmentRepository appointmentRepository)
        {
            _reviewRepository = reviewRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task AddReviewAsync(Review review)
        {
            // 1. Validări simple
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

            // 2. Validare existență Appointment (Folosind noua dependență)
            var appointment = await _appointmentRepository.GetByIdAsync(review.AppointmentId);
            if (appointment == null)
            {
                AppLogger.Warn($"Review failed: Appointment {review.AppointmentId} not found.");
                throw new AppointmentNotFoundException($"Appointment with ID {review.AppointmentId} does not exist.");
            }

            // 3. Validare apartenență (Clientul care lasă review-ul trebuie să fie cel din programare)
            if (!string.Equals(appointment.CustomerEmail, review.ClientEmail, StringComparison.OrdinalIgnoreCase))
            {
                AppLogger.Warn($"Review failed: User {review.ClientEmail} tried to review appointment {review.AppointmentId} belonging to {appointment.CustomerEmail}.");
                throw new ArgumentException("You can only review your own appointments.");
            }

            // 4. Verificare duplicat
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