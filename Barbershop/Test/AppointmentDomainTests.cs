using Barbershop.DomainLayer;
using Barbershop.EntityLayer;
using Barbershop.RepositoryLayer;
using Barbershop.Utils.Exceptions;
using Barbershop.Utils.Logging;
using Barbershop.Utils.Logging.Interface;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Barbershop.Tests
{
    [TestFixture]
    public class AppointmentDomainTests
    {
        private IAppointmentRepository _appointmentRepository;
        private IUserRepository<Client> _clientRepository;
        private IUserRepository<Barber> _barberRepository;
        private IAppLogger _mockLogger;
        private AppointmentDomain _appointmentDomain;

        [SetUp]
        public void Setup()
        {
            _appointmentRepository = Substitute.For<IAppointmentRepository>();
            _clientRepository = Substitute.For<IUserRepository<Client>>();
            _barberRepository = Substitute.For<IUserRepository<Barber>>();

            _mockLogger = Substitute.For<IAppLogger>();
            AppLogger.Init(_mockLogger);

            _appointmentDomain = new AppointmentDomain(
                _appointmentRepository,
                _clientRepository,
                _barberRepository
            );
        }

        [Test]
        public void CreateAsync_ShouldThrowException_WhenDateIsInThePast()
        {
            var pastAppointment = new Appointment
            {
                AppointmentDate = DateTime.Now.AddDays(-1),
                CustomerEmail = "client@test.com",
                BarberEmail = "barber@test.com"
            };

            Assert.ThrowsAsync<InvalidAppointmentDateException>(async () =>
                await _appointmentDomain.CreateAsync(pastAppointment));
        }

        [Test]
        public void CreateAsync_ShouldThrowException_WhenClientNotFound()
        {
            var appointment = new Appointment { AppointmentDate = DateTime.Now.AddDays(1), CustomerEmail = "unknown@test.com" };
            _clientRepository.GetByEmailAsync(appointment.CustomerEmail).Returns((Client)null);

            Assert.ThrowsAsync<UserNotFoundException>(async () => await _appointmentDomain.CreateAsync(appointment));
        }

        [Test]
        public void CreateAsync_ShouldThrowException_WhenBarberNotFound()
        {
            var appointment = new Appointment { AppointmentDate = DateTime.Now.AddDays(1), CustomerEmail = "c@test.com", BarberEmail = "unknown@b.com" };
            _clientRepository.GetByEmailAsync(appointment.CustomerEmail).Returns(new Client());
            _barberRepository.GetByEmailAsync(appointment.BarberEmail).Returns((Barber)null);

            Assert.ThrowsAsync<UserNotFoundException>(async () => await _appointmentDomain.CreateAsync(appointment));
        }

        [Test]
        public async Task CreateAsync_ShouldCallRepository_WhenDataIsValid()
        {
            var appointment = new Appointment { AppointmentDate = DateTime.Now.AddDays(1), CustomerEmail = "c@test.com", BarberEmail = "b@test.com" };
            _clientRepository.GetByEmailAsync(appointment.CustomerEmail).Returns(new Client());
            _barberRepository.GetByEmailAsync(appointment.BarberEmail).Returns(new Barber());

            await _appointmentDomain.CreateAsync(appointment);

            await _appointmentRepository.Received(1).AddAsync(appointment);
        }

        [Test]
        public async Task CancelAsync_ShouldCallDeleteOnRepository()
        {
            int appointmentId = 10;
            await _appointmentDomain.CancelAsync(appointmentId);
            await _appointmentRepository.Received(1).DeleteByIdAsync(appointmentId);
        }

        [Test]
        public void CreateAsync_ShouldThrowException_WhenDateIsExactlyNow()
        {
            var appointment = new Appointment { AppointmentDate = DateTime.Now };
            Assert.ThrowsAsync<InvalidAppointmentDateException>(async () => await _appointmentDomain.CreateAsync(appointment));
        }

        [Test]
        public async Task GetByCustomerEmailAsync_ShouldReturnList_WhenRepositoryHasData()
        {
            string email = "client@test.com";
            var expectedList = new List<Appointment> { new Appointment(), new Appointment() };
            _appointmentRepository.GetByCustomerEmailAsync(email).Returns(expectedList);

            var result = await _appointmentDomain.GetByCustomerEmailAsync(email);

            Assert.AreEqual(2, result.Count);
            Assert.AreSame(expectedList, result);
        }

        [Test]
        public async Task GetByCustomerEmailAsync_ShouldReturnEmptyList_WhenRepositoryReturnsEmpty()
        {
            string email = "new@test.com";
            _appointmentRepository.GetByCustomerEmailAsync(email).Returns(new List<Appointment>());
            var result = await _appointmentDomain.GetByCustomerEmailAsync(email);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetByBarberEmailAsync_ShouldReturnList_WhenRepositoryHasData()
        {
            string email = "barber@test.com";
            var expectedList = new List<Appointment> { new Appointment { AppointmentID = 1 } };
            _appointmentRepository.GetByBarberEmailAsync(email).Returns(expectedList);

            var result = await _appointmentDomain.GetByBarberEmailAsync(email);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public async Task GetByBarberEmailAsync_ShouldCallRepositoryWithCorrectEmail()
        {
            string email = "check@barber.com";
            await _appointmentDomain.GetByBarberEmailAsync(email);
            await _appointmentRepository.Received(1).GetByBarberEmailAsync(email);
        }

        [Test]
        public async Task CreateAsync_ShouldLogWarning_WhenClientNotFound()
        {
            var appt = new Appointment { AppointmentDate = DateTime.Now.AddDays(1), CustomerEmail = "missing@c.com" };
            _clientRepository.GetByEmailAsync(appt.CustomerEmail).Returns((Client)null);

            try { await _appointmentDomain.CreateAsync(appt); } catch { }

            _mockLogger.Received().Log(
                Arg.Is<string>(s => s.Contains("Client not found")),
                Arg.Is<Barbershop.Utils.Logging.Enum.LogLevel>(l => l == Barbershop.Utils.Logging.Enum.LogLevel.Warning));
        }

        [Test]
        public async Task CreateAsync_ShouldLogInfo_WhenCreatedSuccessfully()
        {
            var appt = new Appointment { AppointmentDate = DateTime.Now.AddDays(1), CustomerEmail = "c@test.com", BarberEmail = "b@test.com" };
            _clientRepository.GetByEmailAsync("c@test.com").Returns(new Client());
            _barberRepository.GetByEmailAsync("b@test.com").Returns(new Barber());

            await _appointmentDomain.CreateAsync(appt);

            _mockLogger.Received().Log(
                Arg.Is<string>(s => s.Contains("created successfully")),
                Arg.Is<Barbershop.Utils.Logging.Enum.LogLevel>(l => l == Barbershop.Utils.Logging.Enum.LogLevel.Info));
        }

        [Test]
        public async Task CancelAsync_ShouldLogInfo_AfterDeletion()
        {
            int id = 5;
            await _appointmentDomain.CancelAsync(id);

            _mockLogger.Received().Log(
                Arg.Is<string>(s => s.Contains($"Appointment cancelled: ID {id}")),
                Arg.Is<Barbershop.Utils.Logging.Enum.LogLevel>(l => l == Barbershop.Utils.Logging.Enum.LogLevel.Info));
        }

        [Test]
        public async Task CreateAsync_ShouldPassCorrectObjectToRepository()
        {
            var appt = new Appointment
            {
                AppointmentDate = DateTime.Now.AddDays(5),
                CustomerEmail = "valid@c.com",
                BarberEmail = "valid@b.com",
                ServiceType = "Beard Trim"
            };
            _clientRepository.GetByEmailAsync(appt.CustomerEmail).Returns(new Client());
            _barberRepository.GetByEmailAsync(appt.BarberEmail).Returns(new Barber());

            await _appointmentDomain.CreateAsync(appt);

            await _appointmentRepository.Received(1).AddAsync(
                Arg.Is<Appointment>(a => a.ServiceType == "Beard Trim" && a.AppointmentDate == appt.AppointmentDate));
        }


    }
}
