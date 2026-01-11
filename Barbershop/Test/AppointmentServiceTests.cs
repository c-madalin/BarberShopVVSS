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
    public class AppointmentServiceTests
    {
        private IAppointmentDomain _domain;
        private AppointmentService _service;

        [SetUp]
        public void Setup()
        {
            _domain = Substitute.For<IAppointmentDomain>();
            _service = new AppointmentService(_domain);
        }


        [Test]
        public async Task CreateAppointmentAsync_ShouldMapFieldsAndCallDomain()
        {
            string clientEmail = "client@test.com";
            string barberEmail = "barber@test.com";
            DateTime date = DateTime.Now.AddDays(2);
            string serviceType = "Haircut";

            await _service.CreateAppointmentAsync(clientEmail, barberEmail, date, serviceType);

            await _domain.Received(1).CreateAsync(Arg.Is<Appointment>(x =>
                x.CustomerEmail == clientEmail &&
                x.BarberEmail == barberEmail &&
                x.AppointmentDate == date &&
                x.ServiceType == serviceType
            ));
        }

        [Test]
        public async Task GetHistoryClientAsync_ShouldReturnListFromDomain()
        {
            string email = "client@test.com";
            var expectedList = new List<Appointment> { new Appointment(), new Appointment() };
            _domain.GetByCustomerEmailAsync(email).Returns(expectedList);

            var result = await _service.GetHistoryClientAsync(email);

            Assert.AreEqual(expectedList, result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task GetHistoryBarberAsync_ShouldCallDomain()
        {
            string email = "b@test.com";
            await _service.GetHistoryBarberAsync(email);
            await _domain.Received(1).GetByBarberEmailAsync(email);
        }

        [Test]
        public async Task GetHistoryBarberAsync_ShouldReturnDomainData()
        {
            string email = "b@test.com";
            var expected = new List<Appointment> { new Appointment() };
            _domain.GetByBarberEmailAsync(email).Returns(expected);

            var result = await _service.GetHistoryBarberAsync(email);
            Assert.AreSame(expected, result);
        }

        [Test]
        public async Task CancelAsync_ShouldCallDomainWithCorrectId()
        {
            int id = 99;
            await _service.CancelAsync(id);
            await _domain.Received(1).CancelAsync(id);
        }

        [Test]
        public async Task CreateAppointmentAsync_ShouldMapServiceTypeCorrectly()
        {
            string serviceType = "Fade";
            await _service.CreateAppointmentAsync("c", "b", DateTime.Now, serviceType);
            await _domain.Received(1).CreateAsync(Arg.Is<Appointment>(a => a.ServiceType == serviceType));
        }

        [Test]
        public async Task CreateAppointmentAsync_ShouldMapCustomerEmailCorrectly()
        {
            string email = "custom@client.com";
            await _service.CreateAppointmentAsync(email, "b", DateTime.Now, "s");
            await _domain.Received(1).CreateAsync(Arg.Is<Appointment>(a => a.CustomerEmail == email));
        }

        [Test]
        public async Task CreateAppointmentAsync_ShouldMapBarberEmailCorrectly()
        {
            string email = "custom@barber.com";
            await _service.CreateAppointmentAsync("c", email, DateTime.Now, "s");
            await _domain.Received(1).CreateAsync(Arg.Is<Appointment>(a => a.BarberEmail == email));
        }

        [Test]
        public async Task CreateAppointmentAsync_ShouldMapDateCorrectly()
        {
            DateTime date = new DateTime(2025, 12, 25);
            await _service.CreateAppointmentAsync("c", "b", date, "s");
            await _domain.Received(1).CreateAsync(Arg.Is<Appointment>(a => a.AppointmentDate == date));
        }

        [Test]
        public async Task GetHistoryClientAsync_ShouldReturnEmpty_WhenDomainEmpty()
        {
            _domain.GetByCustomerEmailAsync(Arg.Any<string>()).Returns(new List<Appointment>());
            var result = await _service.GetHistoryClientAsync("any");

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }


        [Test]
        public void CreateAppointmentAsync_ShouldPropagateException_WhenDomainThrows()
        {

            _domain.When(x => x.CreateAsync(Arg.Any<Appointment>()))
                   .Do(x => { throw new InvalidOperationException("Domain Error"); });

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.CreateAppointmentAsync("c", "b", DateTime.Now, "s"));

            Assert.AreEqual("Domain Error", ex.Message);
        }

        [Test]
        public async Task GetHistoryClientAsync_ShouldCallDomainWithCorrectEmail()
        {

            string specificEmail = "unique@client.com";
            await _service.GetHistoryClientAsync(specificEmail);

            await _domain.Received(1).GetByCustomerEmailAsync(specificEmail);
        }

        [Test]
        public void CancelAsync_ShouldPropagateException_WhenDeletionFails()
        {

            int id = 500;
            _domain.When(x => x.CancelAsync(id))
                   .Do(x => { throw new ArgumentException("ID not found"); });

            Assert.ThrowsAsync<ArgumentException>(async () => await _service.CancelAsync(id));
        }

        [Test]
        public async Task CreateAppointmentAsync_ShouldPassNullServiceType_IfProvided()
        {

            string nullService = null;
            await _service.CreateAppointmentAsync("c", "b", DateTime.Now, nullService);

            await _domain.Received(1).CreateAsync(Arg.Is<Appointment>(a => a.ServiceType == null));
        }

        [Test]
        public async Task GetHistoryBarberAsync_ShouldReturnNull_WhenDomainReturnsNull()
        {

            _domain.GetByBarberEmailAsync(Arg.Any<string>()).Returns((List<Appointment>)null);

            var result = await _service.GetHistoryBarberAsync("barber@test.com");

            Assert.IsNull(result);
        }
    }
}