using Barbershop.DomainLayer;
using Barbershop.EntityLayer;
using Barbershop.ServiceLayer;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Barbershop.Tests
{
    [TestFixture]
    public class ReviewServiceTests
    {
        private IReviewDomain _reviewDomain;
        private ReviewService _reviewService;

        [SetUp]
        public void Setup()
        {
            _reviewDomain = Substitute.For<IReviewDomain>();
            _reviewService = new ReviewService(_reviewDomain);
        }

        [Test]
        public void AddReviewAsync_ShouldThrowException_WhenAppointmentIdIsInvalid()
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _reviewService.AddReviewAsync(-1, "client@a.com", "barber@a.com", 5, "Good"));
            Assert.AreEqual("Invalid Appointment ID.", ex.Message);
        }

        [TestCase("", "barber@test.com")]
        [TestCase("client@test.com", "")]
        [TestCase(null, "barber@test.com")]
        public void AddReviewAsync_ShouldThrowException_WhenEmailsAreInvalid(string clientEmail, string barberEmail)
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _reviewService.AddReviewAsync(1, clientEmail, barberEmail, 5, "Ok"));
        }

        [Test]
        public async Task AddReviewAsync_ShouldCallDomain_WhenInputIsValid()
        {
            int appId = 1;
            string client = "c@test.com";
            string barber = "b@test.com";
            int rating = 5;
            string comment = "Excellent!";

            await _reviewService.AddReviewAsync(appId, client, barber, rating, comment);

            await _reviewDomain.Received(1).AddReviewAsync(Arg.Is<Review>(r =>
                r.AppointmentId == appId &&
                r.ClientEmail == client &&
                r.BarberEmail == barber &&
                r.Rating == rating &&
                r.Comment == comment
            ));
        }

        [Test]
        public async Task GetReviewsForBarberAsync_ShouldCallDomain()
        {
            string email = "b@test.com";
            await _reviewService.GetReviewsForBarberAsync(email);
            await _reviewDomain.Received(1).GetReviewsByBarberAsync(email);
        }

        [Test]
        public async Task GetReviewsForBarberAsync_ShouldReturnList()
        {
            var list = new List<Review> { new Review { Rating = 5 } };
            _reviewDomain.GetReviewsByBarberAsync(Arg.Any<string>()).Returns(list);

            var result = await _reviewService.GetReviewsForBarberAsync("e");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(5, result[0].Rating);
        }

        [Test]
        public async Task AddReviewAsync_ShouldTrimComment()
        {
            string comment = "  Spaced Comment  ";
            await _reviewService.AddReviewAsync(1, "c", "b", 5, comment);

            await _reviewDomain.Received(1).AddReviewAsync(
                Arg.Is<Review>(r => r.Comment == "Spaced Comment"));
        }

        [Test]
        public async Task AddReviewAsync_ShouldHandleNullComment()
        {
            await _reviewService.AddReviewAsync(1, "c", "b", 5, null);
            await _reviewDomain.Received(1).AddReviewAsync(
                Arg.Is<Review>(r => r.Comment == null));
        }

        [Test]
        public async Task AddReviewAsync_ShouldSetDatePostedToRecentTime()
        {
            await _reviewService.AddReviewAsync(1, "c", "b", 5, "comm");

            await _reviewDomain.Received(1).AddReviewAsync(
                Arg.Is<Review>(r =>
                    r.DatePosted > DateTime.Now.AddSeconds(-5) &&
                    r.DatePosted <= DateTime.Now));
        }

        [Test]
        public async Task AddReviewAsync_ShouldPropagateRating()
        {
            int rating = 3;
            await _reviewService.AddReviewAsync(1, "c", "b", rating, "c");
            await _reviewDomain.Received(1).AddReviewAsync(
                Arg.Is<Review>(r => r.Rating == rating));
        }

        [Test]
        public void AddReviewAsync_ExceptionMessage_ShouldBeSpecificForId()
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _reviewService.AddReviewAsync(0, "c", "b", 5, "c"));
            Assert.AreEqual("Invalid Appointment ID.", ex.Message);
        }

        [Test]
        public void AddReviewAsync_ExceptionMessage_ShouldBeSpecificForClientEmail()
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _reviewService.AddReviewAsync(1, "", "b", 5, "c"));
            Assert.AreEqual("Client email required.", ex.Message);
        }
    }
}
