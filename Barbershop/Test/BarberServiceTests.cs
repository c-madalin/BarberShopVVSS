using Barbershop._1.ServiceLayer.DTOs;
using Barbershop.DomainLayer;
using Barbershop.EntityLayer;
using Barbershop.NetworkingLayer;
using Barbershop.ServiceLayer;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

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
        public void NewRegisterAsync_NullFirstName_ThrowsArgumentException()
        {
            var dto = new BarberRegisterDto(null, "Doe", "a@b.c", "123", "pass1234", "Cut", 100);
            Assert.ThrowsAsync<ArgumentException>(() => _service.NewRegisterAsync(dto));
        }

        [Test]
        public void NewRegisterAsync_NullLastName_ThrowsArgumentException()
        {
            var dto = new BarberRegisterDto("John", null, "a@b.c", "123", "pass1234", "Cut", 100);
            Assert.ThrowsAsync<ArgumentException>(() => _service.NewRegisterAsync(dto));
        }

        [Test]
        public void NewRegisterAsync_NullEmail_ThrowsArgumentException()
        {
            var dto = new BarberRegisterDto("John", "Doe", null, "123", "pass1234", "Cut", 100);
            Assert.ThrowsAsync<ArgumentException>(() => _service.NewRegisterAsync(dto));
        }

        [Test]
        public void NewRegisterAsync_NullPhone_ThrowsArgumentException()
        {
            var dto = new BarberRegisterDto("John", "Doe", "a@b.c", null, "pass1234", "Cut", 100);
            Assert.ThrowsAsync<ArgumentException>(() => _service.NewRegisterAsync(dto));
        }

        [Test]
        public void NewRegisterAsync_NullSpecialisation_ThrowsArgumentException()
        {
            var dto = new BarberRegisterDto("John", "Doe", "a@b.c", "123", "pass1234", null, 100);
            Assert.ThrowsAsync<ArgumentException>(() => _service.NewRegisterAsync(dto));
        }

        [Test]
        public void NewRegisterAsync_NegativeSalary_ThrowsArgumentOutOfRangeException()
        {
            var dto = new BarberRegisterDto("John", "Doe", "a@b.c", "123", "pass1234", "Cut", -1);
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.NewRegisterAsync(dto));
        }

        [Test]
        public void NewRegisterAsync_NullPassword_ThrowsArgumentException()
        {
            var dto = new BarberRegisterDto("John", "Doe", "a@b.c", "123", null, "Cut", 100);
            Assert.ThrowsAsync<ArgumentException>(() => _service.NewRegisterAsync(dto));
        }

        [Test]
        public void NewRegisterAsync_ShortPassword_ThrowsArgumentException()
        {
            var dto = new BarberRegisterDto("John", "Doe", "a@b.c", "123", "short", "Cut", 100);
            Assert.ThrowsAsync<ArgumentException>(() => _service.NewRegisterAsync(dto));
        }

        [Test]
        public void NewRegisterAsync_InvalidEmail_ThrowsException()
        {
            _mockVerifier.Setup(v => v.IsValidEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
            var dto = new BarberRegisterDto("John", "Doe", "bad@email", "123", "pass1234", "Cut", 100);
            Assert.ThrowsAsync<Exception>(() => _service.NewRegisterAsync(dto));
        }

        [Test]
        public async Task NewRegisterAsync_ValidData_CallsDomainRegister()
        {
            _mockVerifier.Setup(v => v.IsValidEmailAsync(It.IsAny<string>())).ReturnsAsync(true);
            var dto = new BarberRegisterDto("John", "Doe", "a@b.c", "123", "pass1234", "Cut", 100);

            await _service.NewRegisterAsync(dto);

            _mockDomain.Verify(d => d.RegisterAsync(It.IsAny<Barber>(), "pass1234"), Times.Once);
        }

        [Test]
        public void LoginAsync_NullEmail_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _service.LoginAsync(null, "pass"));
        }

        [Test]
        public void LoginAsync_NullPassword_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _service.LoginAsync("a@b.c", null));
        }

        [Test]
        public void LoginAsync_InvalidEmail_ThrowsException()
        {
            _mockVerifier.Setup(v => v.IsValidEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
            Assert.ThrowsAsync<Exception>(() => _service.LoginAsync("bad@email", "pass"));
        }

        [Test]
        public async Task LoginAsync_ValidData_CallsDomainLogin()
        {
            _mockVerifier.Setup(v => v.IsValidEmailAsync(It.IsAny<string>())).ReturnsAsync(true);
            await _service.LoginAsync("a@b.c", "pass");
            _mockDomain.Verify(d => d.LoginAsync("a@b.c", "pass"), Times.Once);
        }

        [Test]
        public void DeleteAsync_NullEmail_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _service.DeleteAsync(null));
        }

        [Test]
        public void UpdateStatusAsync_NullEmail_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateStatusAsync(null));
        }
    }
}