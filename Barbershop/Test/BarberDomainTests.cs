using Azure.Identity;
using Barbershop.DomainLayer;
using Barbershop.EntityLayer;
using Barbershop.NetworkingLayer;
using Barbershop.RepositoryLayer;
using Barbershop.Utils;
using Barbershop.Utils.Exceptions;
using Moq;
using NUnit.Framework;
using System;
using AuthenticationFailedException = Barbershop.Utils.Exceptions.AuthenticationFailedException;

namespace Barbershop.Tests.DomainLayer
{
    public class BarberDomainTests
    {
        private Mock<IUserRepository<Barber>> _mockRepo;
        private Mock<IEmailVerifier> _mockVerifier;
        private IUserDomain<Barber> _domain;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IUserRepository<Barber>>();
            _mockVerifier = new Mock<IEmailVerifier>();
            _domain = new BarberDomain(_mockRepo.Object, _mockVerifier.Object);
        }

        [Test]
        public void Register_ExistingUser_ThrowsUserAlreadyExistsException()
        {
            _mockRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns(new Barber());
            Assert.Throws<UserAlreadyExistsException>(() => _domain.Register(new Barber { Email = "a@b.c" }, "pass"));
        }

        [Test]
        public void Register_NegativeSalary_ThrowsInvalidSalaryException()
        {
            _mockRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((Barber)null);
            Assert.Throws<InvalidSalaryException>(() => _domain.Register(new Barber { Email = "a@b.c", Salary = -1 }, "pass"));
        }

        [Test]
        public void Register_ShortEmail_ThrowsInvalidEmailException()
        {
            _mockRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((Barber)null);
            Assert.Throws<InvalidEmailException>(() => _domain.Register(new Barber { Email = "a", Salary = 100 }, "pass"));
        }

        [Test]
        public void Register_Valid_AddsUser()
        {
            _mockRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((Barber)null);
            _domain.Register(new Barber { Email = "valid@b.c", Salary = 100 }, "pass");
            _mockRepo.Verify(r => r.Add(It.IsAny<Barber>()), Times.Once);
        }

        [Test]
        public void Login_NotFound_ThrowsUserNotFoundException()
        {
            _mockRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((Barber)null);
            Assert.Throws<UserNotFoundException>(() => _domain.Login("a@b.c", "pass"));
        }

        [Test]
        public void Login_Inactive_ThrowsAuthenticationFailedException()
        {
            _mockRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns(new Barber { IsActive = false });
            Assert.Throws<AuthenticationFailedException>(() => _domain.Login("a@b.c", "pass"));
        }

        [Test]
        public void Login_WrongPassword_ThrowsAuthenticationFailedException()
        {
            var hash = SecurityUtils.Hash("correct");
            _mockRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns(new Barber { IsActive = true, PasswordHash = hash });
            Assert.Throws<AuthenticationFailedException>(() => _domain.Login("a@b.c", "wrong"));
        }

        [Test]
        public void UpdateStatus_NotFound_ThrowsException()
        {
            _mockRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((Barber)null);
            Assert.Throws<Exception>(() => _domain.UpdateStatus("a@b.c", false));
        }

        [Test]
        public void Delete_NotFound_ThrowsException()
        {
            _mockRepo.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((Barber)null);
            Assert.Throws<Exception>(() => _domain.Delete("a@b.c"));
        }
    }
}