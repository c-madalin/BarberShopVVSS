using Barbershop.EntityLayer;
using Barbershop.RepositoryLayer;
using System;
using System.Collections.Generic;

namespace Barbershop.DomainLayer
{
    public class AppointmentDomain
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserRepository<Client> _clientRepository;
        private readonly IUserRepository<Barber> _barberRepository;

        public AppointmentDomain(
            IAppointmentRepository appointmentRepository,
            IUserRepository<Client> clientRepository,
            IUserRepository<Barber> barberRepository)
        {
            _appointmentRepository = appointmentRepository;
            _clientRepository = clientRepository;
            _barberRepository = barberRepository;
        }

        public void Create(Appointment appointment)
        {
            if (appointment.AppointmentDate <= DateTime.Now)
                throw new Exception("Date must be in the future.");

            var client = _clientRepository.GetByEmail(appointment.CustomerEmail);
            if (client == null) throw new Exception("Client not found.");

            var barber = _barberRepository.GetByEmail(appointment.BarberEmail);
            if (barber == null) throw new Exception("Barber not found.");

            _appointmentRepository.Add(appointment);
        }

        public List<Appointment> GetByCustomerEmail(string email)
        {
            return _appointmentRepository.GetByCustomerEmail(email);
        }

        public List<Appointment> GetByBarberEmail(string email)
        {
            return _appointmentRepository.GetByBarberEmail(email);
        }

        public void Cancel(int id)
        {
            _appointmentRepository.DeleteById(id);
        }
    }
}