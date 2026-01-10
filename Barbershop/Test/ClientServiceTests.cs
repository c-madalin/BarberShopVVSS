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
    public class ClientServiceTests
    {
        private Mock<IUserDomain<Client>> _mockDomain;
        private Mock<IEmailVerifier> _mockVerifier;
        private ClientService _service;

        [SetUp]
        public void Setup()
        {
            _mockDomain = new Mock<IUserDomain<Client>>();
            _mockVerifier = new Mock<IEmailVerifier>();
            _service = new ClientService(_mockDomain.Object, _mockVerifier.Object);
        }

        [Test]
        public void NewRegisterAsync_NullFirstName_ThrowsArgumentException()
        {
            var dto = new ClientRegisterDto(null, "Doe", "a@b.c", "123", "pass1234");
            Assert.ThrowsAsync<ArgumentException>(() => _service.NewRegisterAsync(dto));
        }

        [Test]
        public void NewRegisterAsync_NullLastName_ThrowsArgumentException()
        {
            var dto = new ClientRegisterDto("John", null, "a@b.c", "123", "pass1234");
            Assert.ThrowsAsync<ArgumentException>(() => _service.NewRegisterAsync(dto));
        }

        [Test]
        public void NewRegisterAsync_NullEmail_ThrowsArgumentException()
        {
            var dto = new ClientRegisterDto("John", "Doe", null, "123", "pass1234");
            Assert.ThrowsAsync<ArgumentException>(() => _service.NewRegisterAsync(dto));
        }

        [Test]
        public void NewRegisterAsync_NullPhone_ThrowsArgumentException()
        {
            var dto = new ClientRegisterDto("John", "Doe", "a@b.c", null, "pass1234");
            Assert.ThrowsAsync<ArgumentException>(() => _service.NewRegisterAsync(dto));
        }

        [Test]
        public void NewRegisterAsync_NullPassword_ThrowsArgumentException()
        {
            var dto = new ClientRegisterDto("John", "Doe", "a@b.c", "123", null);
            Assert.ThrowsAsync<ArgumentException>(() => _service.NewRegisterAsync(dto));
        }

        [Test]
        public void NewRegisterAsync_ShortPassword_ThrowsArgumentException()
        {
            var dto = new ClientRegisterDto("John", "Doe", "a@b.c", "123", "short");
            Assert.ThrowsAsync<ArgumentException>(() => _service.NewRegisterAsync(dto));
        }

        [Test]
        public void NewRegisterAsync_InvalidEmail_ThrowsException()
        {
            _mockVerifier.Setup(v => v.IsValidEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
            var dto = new ClientRegisterDto("John", "Doe", "bad@email", "123", "pass1234");

            Assert.ThrowsAsync<Exception>(() => _service.NewRegisterAsync(dto));
        }

        [Test]
        public async Task NewRegisterAsync_ValidData_CallsDomainRegister()
        {
            _mockVerifier.Setup(v => v.IsValidEmailAsync(It.IsAny<string>())).ReturnsAsync(true);
            var dto = new ClientRegisterDto("John", "Doe", "a@b.c", "123", "pass1234");

            await _service.NewRegisterAsync(dto);

            _mockDomain.Verify(d => d.RegisterAsync(It.IsAny<Client>(), "pass1234"), Times.Once);
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
        public async Task DeleteAsync_ValidEmail_CallsDomainDelete()
        {
            await _service.DeleteAsync("a@b.c");
            _mockDomain.Verify(d => d.DeleteAsync("a@b.c"), Times.Once);
        }

        [Test]
        public void UpdateStatusAsync_NullEmail_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateStatusAsync(null));
        }

        [Test]
        public async Task UpdateStatusAsync_ValidEmail_CallsDomainUpdateStatus()
        {
            await _service.UpdateStatusAsync("a@b.c");
            _mockDomain.Verify(d => d.UpdateStatusAsync("a@b.c", false), Times.Once);
        }
    }
}