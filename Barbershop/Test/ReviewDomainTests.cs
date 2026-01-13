using Barbershop.DomainLayer;
using Barbershop.EntityLayer;
using Barbershop.RepositoryLayer;
using Barbershop.Utils.Exceptions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Barbershop.Tests
{
    [TestFixture]
    public class ReviewDomainTests
    {
        private IReviewRepository _reviewRepository;
        private IAppointmentRepository _appointmentRepository;
        private ReviewDomain _reviewDomain;

        [SetUp]
        public void Setup()
        {
            _reviewRepository = Substitute.For<IReviewRepository>();
            _appointmentRepository = Substitute.For<IAppointmentRepository>();
            _reviewDomain = new ReviewDomain(_reviewRepository, _appointmentRepository);
        }

        

        [Test]
        public async Task AddReviewAsync_ShouldCallRepository_WhenDataIsValid()
        {
            var review = new Review { AppointmentId = 1, Rating = 5 };
            _appointmentRepository.GetByIdAsync(1).Returns(new Appointment());
            _reviewRepository.HasReviewForAppointmentAsync(1).Returns(false);

            await _reviewDomain.AddReviewAsync(review);

            await _reviewRepository.Received(1).AddAsync(review);
        }

        [Test]
        public async Task GetReviewsByBarberAsync_ShouldReturnList_WhenReviewsExist()
        {
            string email = "barber@test.com";
            var expectedList = new List<Review> { new Review { Rating = 5 }, new Review { Rating = 4 } };
            _reviewRepository.GetByBarberEmailAsync(email).Returns(expectedList);

            var result = await _reviewDomain.GetReviewsByBarberAsync(email);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(5, result[0].Rating);
        }

        [Test]
        public async Task GetReviewsByBarberAsync_ShouldReturnEmptyList_WhenNoReviewsFound()
        {
            string email = "newbarber@test.com";
            _reviewRepository.GetByBarberEmailAsync(email).Returns(new List<Review>());

            var result = await _reviewDomain.GetReviewsByBarberAsync(email);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task AddReviewAsync_ShouldThrowException_WhenRatingIsInvalidHigh()
        {
            var review = new Review { AppointmentId = 1, Rating = 6 };
            _appointmentRepository.GetByIdAsync(1).Returns(new Appointment());

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _reviewDomain.AddReviewAsync(review));
        }

        [Test]
        public async Task AddReviewAsync_ShouldThrowException_WhenRatingIsInvalidLow()
        {
            var review = new Review { AppointmentId = 1, Rating = 0 };
            _appointmentRepository.GetByIdAsync(1).Returns(new Appointment());

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _reviewDomain.AddReviewAsync(review));
        }

        [Test]
        public async Task AddReviewAsync_ShouldVerifyAppointmentBelongsToClient()
        {
            var review = new Review { AppointmentId = 1, ClientEmail = "client@test.com" };
            var appointment = new Appointment { AppointmentID = 1, CustomerEmail = "other@test.com" };

            _appointmentRepository.GetByIdAsync(1).Returns(appointment);

            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _reviewDomain.AddReviewAsync(review));
        }
    }
}