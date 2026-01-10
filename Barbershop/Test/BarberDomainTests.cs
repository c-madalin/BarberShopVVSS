using Barbershop.DomainLayer;
using Barbershop.EntityLayer;
using Barbershop.RepositoryLayer;
using Barbershop.Utils;
using Barbershop.Utils.Exceptions;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Barbershop.Tests.DomainLayer
{
    [TestFixture]
    public class BarberDomainTests
    {
        private Mock<IUserRepository<Barber>> _mockRepo;
        private IUserDomain<Barber> _domain;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IUserRepository<Barber>>();
            _domain = new BarberDomain(_mockRepo.Object);
        }

        [Test]
        public void Register_ExistingUser_ThrowsUserAlreadyExistsException()
        {
            var existingEmail = "exists@barber.com";
            _mockRepo.Setup(r => r.GetByEmailAsync(existingEmail))
                     .ReturnsAsync(new Barber());

            var newBarber = new Barber { Email = existingEmail };

            Assert.ThrowsAsync<UserAlreadyExistsException>(async () =>
                await _domain.RegisterAsync(newBarber, "password123"));
        }

        [Test]
        public void Register_NegativeSalary_ThrowsInvalidSalaryException()
        {
            _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                     .ReturnsAsync((Barber)null);

            var barber = new Barber { Email = "new@barber.com", Salary = -100 };

            Assert.ThrowsAsync<InvalidSalaryException>(async () =>
                await _domain.RegisterAsync(barber, "password123"));
        }

        [Test]
        public void Register_ShortEmail_ThrowsInvalidEmailException()
        {
            _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                     .ReturnsAsync((Barber)null);

            var barber = new Barber { Email = "a", Salary = 1000 };

            Assert.ThrowsAsync<InvalidEmailException>(async () =>
                await _domain.RegisterAsync(barber, "password123"));
        }

        [Test]
        public async Task Register_Valid_AddsUser()
        {
            string email = "valid@barber.com";
            _mockRepo.Setup(r => r.GetByEmailAsync(email))
                     .ReturnsAsync((Barber)null);

            var barber = new Barber { Email = email, Salary = 5000 };

            await _domain.RegisterAsync(barber, "password123");

            _mockRepo.Verify(r => r.AddAsync(It.Is<Barber>(b => b.Email == email && b.PasswordHash != null)), Times.Once);
        }

        [Test]
        public void Login_NotFound_ThrowsUserNotFoundException()
        {
            _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                     .ReturnsAsync((Barber)null);

            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                await _domain.LoginAsync("unknown@barber.com", "password"));
        }

        [Test]
        public void Login_Inactive_ThrowsAuthenticationFailedException()
        {
            var inactiveBarber = new Barber { Email = "inactive@barber.com", IsActive = false };
            _mockRepo.Setup(r => r.GetByEmailAsync(inactiveBarber.Email))
                     .ReturnsAsync(inactiveBarber);

            Assert.ThrowsAsync<AuthenticationFailedException>(async () =>
                await _domain.LoginAsync(inactiveBarber.Email, "password"));
        }

        [Test]
        public void Login_WrongPassword_ThrowsAuthenticationFailedException()
        {
            string correctPass = "CorrectPass123";
            string email = "test@barber.com";
            string hash = SecurityUtils.Hash(correctPass);

            var barber = new Barber { Email = email, IsActive = true, PasswordHash = hash };

            _mockRepo.Setup(r => r.GetByEmailAsync(email))
                     .ReturnsAsync(barber);

            Assert.ThrowsAsync<AuthenticationFailedException>(async () =>
                await _domain.LoginAsync(email, "WrongPass"));
        }

        [Test]
        public async Task Login_Success_ReturnsBarber()
        {
            string password = "CorrectPass123";
            string email = "test@barber.com";
            string hash = SecurityUtils.Hash(password);

            var barber = new Barber { Email = email, IsActive = true, PasswordHash = hash };

            _mockRepo.Setup(r => r.GetByEmailAsync(email))
                     .ReturnsAsync(barber);

            var result = await _domain.LoginAsync(email, password);

            Assert.IsNotNull(result);
            Assert.AreEqual(email, result.Email);
        }

        [Test]
        public void UpdateStatus_NotFound_ThrowsUserNotFoundException()
        {
            _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                     .ReturnsAsync((Barber)null);

            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                await _domain.UpdateStatusAsync("unknown@barber.com", false));
        }

        [Test]
        public async Task UpdateStatus_Valid_CallsRepo()
        {
            string email = "valid@barber.com";
            _mockRepo.Setup(r => r.GetByEmailAsync(email))
                     .ReturnsAsync(new Barber { Email = email });

            await _domain.UpdateStatusAsync(email, false);

            _mockRepo.Verify(r => r.UpdateStatusAsync(email, false), Times.Once);
        }

        [Test]
        public void Delete_NotFound_ThrowsUserNotFoundException()
        {
            _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                     .ReturnsAsync((Barber)null);

            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                await _domain.DeleteAsync("unknown@barber.com"));
        }

        [Test]
        public async Task Delete_Valid_CallsRepo()
        {
            string email = "valid@barber.com";
            _mockRepo.Setup(r => r.GetByEmailAsync(email))
                     .ReturnsAsync(new Barber { Email = email });

            await _domain.DeleteAsync(email);

            _mockRepo.Verify(r => r.DeleteAsync(email), Times.Once);
        }
    }
}