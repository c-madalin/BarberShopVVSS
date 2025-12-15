using NUnit.Framework;
using Moq;
using System;
using System.Threading.Tasks;
using Barbershop.ServiceLayer;
using Barbershop.DomainLayer;
using Barbershop.EntityLayer;
using Barbershop.NetworkingLayer;

namespace Barbershop.Tests.ServiceLayer
{
    public class BarberServiceTests
    {
        private Mock<IUserDomain<Barber>> _mockDomain;
        private Mock<IEmailVerifier> _mockVerifier;
        private BarberService _service;

        [SetUp]
        public void Setup()
        {
            _mockDomain = new Mock<IUserDomain<Barber>>();
            _mockVerifier = new Mock<IEmailVerifier>();
            _service = new BarberService(_mockDomain.Object, _mockVerifier.Object);
        }

        [Test]
        public void NewRegister_NullFirstName_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _service.NewRegister(null, "Doe", "a@b.c", "123", "pass1234", "Cut", 100));
        }

        [Test]
        public void NewRegister_NullLastName_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _service.NewRegister("John", null, "a@b.c", "123", "pass1234", "Cut", 100));
        }

        [Test]
        public void NewRegister_NullEmail_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _service.NewRegister("John", "Doe", null, "123", "pass1234", "Cut", 100));
        }

        [Test]
        public void NewRegister_NullPhone_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _service.NewRegister("John", "Doe", "a@b.c", null, "pass1234", "Cut", 100));
        }

        [Test]
        public void NewRegister_NullSpecialisation_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _service.NewRegister("John", "Doe", "a@b.c", "123", "pass1234", null, 100));
        }

        [Test]
        public void NewRegister_NegativeSalary_ThrowsArgumentOutOfRangeException()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.NewRegister("John", "Doe", "a@b.c", "123", "pass1234", "Cut", -1));
        }

        [Test]
        public void NewRegister_NullPassword_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _service.NewRegister("John", "Doe", "a@b.c", "123", null, "Cut", 100));
        }

        [Test]
        public void NewRegister_ShortPassword_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _service.NewRegister("John", "Doe", "a@b.c", "123", "short", "Cut", 100));
        }

        [Test]
        public void NewRegister_InvalidEmail_ThrowsException()
        {
            _mockVerifier.Setup(v => v.IsValidEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
            Assert.ThrowsAsync<Exception>(() => _service.NewRegister("John", "Doe", "bad@email", "123", "pass1234", "Cut", 100));
        }

        [Test]
        public async Task NewRegister_ValidData_CallsDomainRegister()
        {
            _mockVerifier.Setup(v => v.IsValidEmailAsync(It.IsAny<string>())).ReturnsAsync(true);
            await _service.NewRegister("John", "Doe", "a@b.c", "123", "pass1234", "Cut", 100);
            _mockDomain.Verify(d => d.Register(It.IsAny<Barber>(), "pass1234"), Times.Once);
        }

        [Test]
        public void Login_NullEmail_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _service.Login(null, "pass"));
        }

        [Test]
        public void Login_NullPassword_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _service.Login("a@b.c", null));
        }

        [Test]
        public void Login_InvalidEmail_ThrowsException()
        {
            _mockVerifier.Setup(v => v.IsValidEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
            Assert.ThrowsAsync<Exception>(() => _service.Login("bad@email", "pass"));
        }

        [Test]
        public async Task Login_ValidData_CallsDomainLogin()
        {
            _mockVerifier.Setup(v => v.IsValidEmailAsync(It.IsAny<string>())).ReturnsAsync(true);
            await _service.Login("a@b.c", "pass");
            _mockDomain.Verify(d => d.Login("a@b.c", "pass"), Times.Once);
        }

        [Test]
        public void Delete_NullEmail_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _service.Delete(null));
        }

        [Test]
        public void UpdateStatus_NullEmail_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _service.UpdateStatus(null));
        }
    }
}