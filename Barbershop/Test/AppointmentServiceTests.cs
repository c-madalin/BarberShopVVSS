using NUnit.Framework;
using Moq;
using System;
using Barbershop.ServiceLayer;
using Barbershop.DomainLayer;
using Barbershop.EntityLayer;

namespace Barbershop.Tests.ServiceLayer
{
    public class AppointmentServiceTests
    {
        private Mock<IAppointmentDomain> _mockDomain;
        private AppointmentService _service;

        [SetUp]
        public void Setup()
        {
            _mockDomain = new Mock<IAppointmentDomain>();
            _service = new AppointmentService(_mockDomain.Object);
        }

        [Test]
        public void CreateAppointment_CallsDomainCreate()
        {
            _service.CreateAppointment("c@b.c", "b@b.c", DateTime.Now.AddDays(1), "Cut");
            _mockDomain.Verify(d => d.Create(It.IsAny<Appointment>()), Times.Once);
        }

        [Test]
        public void GetHistoryClient_CallsDomain()
        {
            _service.GetHistoryClient("c@b.c");
            _mockDomain.Verify(d => d.GetByCustomerEmail("c@b.c"), Times.Once);
        }

        [Test]
        public void GetHistoryBarber_CallsDomain()
        {
            _service.GetHistoryBarber("b@b.c");
            _mockDomain.Verify(d => d.GetByBarberEmail("b@b.c"), Times.Once);
        }

        [Test]
        public void Cancel_CallsDomain()
        {
            _service.Cancel(1);
            _mockDomain.Verify(d => d.Cancel(1), Times.Once);
        }
    }
}