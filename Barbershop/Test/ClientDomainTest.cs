using NUnit.Framework;
using Moq;
using System;
using Barbershop.DomainLayer;
using Barbershop.EntityLayer;
using Barbershop.RepositoryLayer;
using Barbershop.NetworkingLayer;
using Barbershop.Utils.Exceptions;
using Barbershop.Utils;

namespace Barbershop.Tests.DomainLayer
{
    public class ClientDomainTests
    {
        private Mock<IUserRepository<Client>> _mockRepo;
        private Mock<IEmailVerifier> _mockVerifier;
        private ClientDomain _domain;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IUserRepository<Client>>();
            _mockVerifier = new Mock<IEmailVerifier>();
            _domain = new ClientDomain(_mockRepo.Object, _mockVerifier.Object);
        }

        [Test]
        public void Register_ExistingUser_ThrowsUserAlreadyExistsException()
        {
            _mockRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns(new Client());
            Assert.Throws<UserAlreadyExistsException>(() => _domain.Register(new Client { Email = "a@b.c" }, "pass"));
        }

        [Test]
        public void Register_Valid_AddsUser()
        {
            _mockRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((Client)null);
            _domain.Register(new Client { Email = "valid@b.c" }, "pass");
            _mockRepo.Verify(r => r.Add(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public void Login_NotFound_ThrowsUserNotFoundException()
        {
            _mockRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((Client)null);
            Assert.Throws<UserNotFoundException>(() => _domain.Login("a@b.c", "pass"));
        }

        [Test]
        public void Login_Inactive_ThrowsAuthenticationFailedException()
        {
            _mockRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns(new Client { IsActive = false });
            Assert.Throws<AuthenticationFailedException>(() => _domain.Login("a@b.c", "pass"));
        }

        [Test]
        public void Login_WrongPassword_ThrowsAuthenticationFailedException()
        {
            var hash = SecurityUtils.Hash("correct");
            _mockRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns(new Client { IsActive = true, PasswordHash = hash });
            Assert.Throws<AuthenticationFailedException>(() => _domain.Login("a@b.c", "wrong"));
        }

        [Test]
        public void UpdateStatus_NotFound_ThrowsException()
        {
            _mockRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((Client)null);
            Assert.Throws<Exception>(() => _domain.UpdateStatus("a@b.c", false));
        }

        [Test]
        public void Delete_NotFound_ThrowsException()
        {
            _mockRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((Client)null);
            Assert.Throws<Exception>(() => _domain.Delete("a@b.c"));
        }
    }
}