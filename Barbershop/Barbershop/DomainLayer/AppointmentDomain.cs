using BarbershopVVSS.DomainLayer;
using BarbershopVVSS.EntityLayer;
using BarbershopVVSS.RepositoryLayer; // <-- Ensure this using directive is present

using System;
using System.Collections.Generic;
using BarbershopVVSS.EntityLayer;
using BarbershopVVSS.RepositoryLayer;

namespace BarbershopVVSS.DomainLayer
{
    public class AppointmentDomain
    {
        private readonly AppointmentRepository _appointmentRepository;
        private readonly IRepository<Client> _clientRepository;
        private readonly IRepository<Barber> _barberRepository;

        public AppointmentDomain(
            AppointmentRepository appointmentRepository,
            IRepository<Client> clientRepository,
            IRepository<Barber> barberRepository)
        {
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
            _barberRepository = barberRepository ?? throw new ArgumentNullException(nameof(barberRepository));
        }

        public void Create(Appointments appointment)
        {
            if (appointment == null) throw new ArgumentNullException(nameof(appointment));
            if (string.IsNullOrWhiteSpace(appointment.CustomerEmail)) throw new ArgumentException("CustomerEmail is required.", nameof(appointment.CustomerEmail));
            if (string.IsNullOrWhiteSpace(appointment.BarberEmail)) throw new ArgumentException("BarberEmail is required.", nameof(appointment.BarberEmail));
            if (appointment.AppointmentDate <= DateTime.UtcNow.AddMinutes(1)) throw new ArgumentException("Appointment date must be in the future.", nameof(appointment.AppointmentDate));

            // Ensure client and barber exist
            var client = _clientRepository.GetByEmail(appointment.CustomerEmail);
            if (client == null) throw new InvalidOperationException($"Client not found: {appointment.CustomerEmail}");

            var barber = _barber_repository_guard(appointment.BarberEmail);

            // Persist appointment
            _appointmentRepository.Add(appointment);
        }

        private Barber _barber_repository_guard(string barberEmail)
        {
            var barber = _barberRepository.GetByEmail(barberEmail);
            if (barber == null) throw new InvalidOperationException($"Barber not found: {barberEmail}");
            return barber;
        }

        public List<Appointments> GetByCustomerEmail(string customerEmail)
        {
            return _appointmentRepository.GetByCustomerEmail(customerEmail ?? string.Empty);
        }

        public List<Appointments> GetByBarberEmail(string barberEmail)
        {
            return _appointmentRepository.GetByBarberEmail(barberEmail ?? string.Empty);
        }

        public void Cancel(int appointmentId)
        {
            if (appointmentId <= 0) throw new ArgumentException("Invalid appointment id.", nameof(appointmentId));
            _appointmentRepository.DeleteById(appointmentId);
        }
    }
}