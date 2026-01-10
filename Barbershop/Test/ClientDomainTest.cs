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
    public class ClientDomainTests
    {
        private Mock<IUserRepository<Client>> _mockRepo;
        private ClientDomain _domain;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IUserRepository<Client>>();
            _domain = new ClientDomain(_mockRepo.Object);
        }

        [Test]
        public void Register_ExistingUser_ThrowsUserAlreadyExistsException()
        {
            _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(new Client());
            Assert.ThrowsAsync<UserAlreadyExistsException>(async () => await _domain.RegisterAsync(new Client { Email = "a@b.c" }, "pass"));
        }

        [Test]
        public async Task Register_Valid_AddsUser()
        {
            _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Client)null);
            await _domain.RegisterAsync(new Client { Email = "valid@b.c" }, "pass");
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public void Login_NotFound_ThrowsUserNotFoundException()
        {
            _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Client)null);
            Assert.ThrowsAsync<UserNotFoundException>(async () => await _domain.LoginAsync("a@b.c", "pass"));
        }

        [Test]
        public void Login_Inactive_ThrowsAuthenticationFailedException()
        {
            _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(new Client { IsActive = false });
            Assert.ThrowsAsync<AuthenticationFailedException>(async () => await _domain.LoginAsync("a@b.c", "pass"));
        }

        [Test]
        public void Login_WrongPassword_ThrowsAuthenticationFailedException()
        {
            var hash = SecurityUtils.Hash("correct");
            _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(new Client { IsActive = true, PasswordHash = hash });
            Assert.ThrowsAsync<AuthenticationFailedException>(async () => await _domain.LoginAsync("a@b.c", "wrong"));
        }

        [Test]
        public async Task Login_Success_ReturnsClient()
        {
            var hash = SecurityUtils.Hash("correct");
            var client = new Client { Email = "a@b.c", IsActive = true, PasswordHash = hash };
            _mockRepo.Setup(r => r.GetByEmailAsync("a@b.c")).ReturnsAsync(client);

            var result = await _domain.LoginAsync("a@b.c", "correct");

            Assert.IsNotNull(result);
            Assert.AreEqual("a@b.c", result.Email);
        }

        [Test]
        public void UpdateStatus_NotFound_ThrowsUserNotFoundException()
        {
            _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Client)null);
            Assert.ThrowsAsync<UserNotFoundException>(async () => await _domain.UpdateStatusAsync("a@b.c", false));
        }

        [Test]
        public async Task UpdateStatus_Valid_CallsRepo()
        {
            _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(new Client());
            await _domain.UpdateStatusAsync("a@b.c", false);
            _mockRepo.Verify(r => r.UpdateStatusAsync("a@b.c", false), Times.Once);
        }

        [Test]
        public void Delete_NotFound_ThrowsUserNotFoundException()
        {
            _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Client)null);
            Assert.ThrowsAsync<UserNotFoundException>(async () => await _domain.DeleteAsync("a@b.c"));
        }

        [Test]
        public async Task Delete_Valid_CallsRepo()
        {
            _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(new Client());
            await _domain.DeleteAsync("a@b.c");
            _mockRepo.Verify(r => r.DeleteAsync("a@b.c"), Times.Once);
        }
    }
}