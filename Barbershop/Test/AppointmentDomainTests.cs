using Barbershop.DomainLayer;
using Barbershop.EntityLayer;
using Barbershop.RepositoryLayer;
using Barbershop.Utils.Exceptions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Barbershop.Tests
{
    [TestFixture]
    public class AppointmentDomainTests
    {
        private IAppointmentRepository _appointmentRepository;
        private IUserRepository<Client> _clientRepository;
        private IUserRepository<Barber> _barberRepository;
        private AppointmentDomain _appointmentDomain;

        [SetUp]
        public void Setup()
        {
            // 1. ARRANGE (Pregătirea Mock-urilor)
            _appointmentRepository = Substitute.For<IAppointmentRepository>();
            _clientRepository = Substitute.For<IUserRepository<Client>>();
            _barberRepository = Substitute.For<IUserRepository<Barber>>();

            // Injectăm mock-urile în Domain
            _appointmentDomain = new AppointmentDomain(
                _appointmentRepository,
                _clientRepository,
                _barberRepository
            );
        }

        [Test]
        public void CreateAsync_ShouldThrowException_WhenDateIsInThePast()
        {
            // Arrange
            var pastAppointment = new Appointment
            {
                AppointmentDate = DateTime.Now.AddDays(-1), // Dată în trecut
                CustomerEmail = "client@test.com",
                BarberEmail = "barber@test.com"
            };

            // Act & Assert
            // Verificăm dacă se aruncă InvalidAppointmentDateException conform logicii din Domain
            Assert.ThrowsAsync<InvalidAppointmentDateException>(async () =>
                await _appointmentDomain.CreateAsync(pastAppointment));
        }

        [Test]
        public void CreateAsync_ShouldThrowException_WhenClientNotFound()
        {
            // Arrange
            var appointment = new Appointment
            {
                AppointmentDate = DateTime.Now.AddDays(1),
                CustomerEmail = "unknown@test.com"
            };

            // Simulăm că Repository-ul de clienți returnează NULL
            _clientRepository.GetByEmailAsync(appointment.CustomerEmail).Returns((Client)null);

            // Act & Assert
            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                await _appointmentDomain.CreateAsync(appointment));
        }

        [Test]
        public void CreateAsync_ShouldThrowException_WhenBarberNotFound()
        {
            // Arrange
            var appointment = new Appointment
            {
                AppointmentDate = DateTime.Now.AddDays(1),
                CustomerEmail = "client@test.com",
                BarberEmail = "unknown_barber@test.com"
            };

            // Simulăm că clientul există, dar frizerul NU
            _clientRepository.GetByEmailAsync(appointment.CustomerEmail).Returns(new Client());
            _barberRepository.GetByEmailAsync(appointment.BarberEmail).Returns((Barber)null);

            // Act & Assert
            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                await _appointmentDomain.CreateAsync(appointment));
        }

        [Test]
        public async Task CreateAsync_ShouldCallRepository_WhenDataIsValid()
        {
            // Arrange
            var appointment = new Appointment
            {
                AppointmentDate = DateTime.Now.AddDays(1),
                CustomerEmail = "client@test.com",
                BarberEmail = "barber@test.com"
            };

            // Simulăm existența ambilor useri
            _clientRepository.GetByEmailAsync(appointment.CustomerEmail).Returns(new Client());
            _barberRepository.GetByEmailAsync(appointment.BarberEmail).Returns(new Barber());

            // Act
            await _appointmentDomain.CreateAsync(appointment);

            // Assert
            // Verificăm dacă metoda AddAsync din repository a fost apelată o dată
            await _appointmentRepository.Received(1).AddAsync(appointment);
        }

        [Test]
        public async Task CancelAsync_ShouldCallDeleteOnRepository()
        {
            // Arrange
            int appointmentId = 10;

            // Act
            await _appointmentDomain.CancelAsync(appointmentId);

            // Assert
            await _appointmentRepository.Received(1).DeleteByIdAsync(appointmentId);
        }
    }
}