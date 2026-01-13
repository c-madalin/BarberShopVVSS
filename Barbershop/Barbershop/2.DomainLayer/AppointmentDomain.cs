using Barbershop.EntityLayer;
using Barbershop.RepositoryLayer;
using Barbershop.Utils.Exceptions;
using Barbershop.Utils.Logging; 
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Barbershop.DomainLayer
{
    public sealed class AppointmentDomain : IAppointmentDomain
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

        public async Task CreateAsync(Appointment appointment)
        {
            if (appointment.AppointmentDate <= DateTime.Now)
            {
                AppLogger.Warn($"Appointment creation failed: Date in past ({appointment.AppointmentDate}).");
                throw new InvalidAppointmentDateException("Appointment date must be in the future.");
            }

            var client = await _clientRepository.GetByEmailAsync(appointment.CustomerEmail);
            if (client == null)
            {
                AppLogger.Warn($"Appointment creation failed: Client not found ({appointment.CustomerEmail}).");
                throw new UserNotFoundException($"Client with email {appointment.CustomerEmail} not found.");
            }

            var barber = await _barberRepository.GetByEmailAsync(appointment.BarberEmail);
            if (barber == null)
            {
                AppLogger.Warn($"Appointment creation failed: Barber not found ({appointment.BarberEmail}).");
                throw new UserNotFoundException($"Barber with email {appointment.BarberEmail} not found.");
            }

            await _appointmentRepository.AddAsync(appointment);
            AppLogger.Info($"Appointment created successfully: {appointment.CustomerEmail} with {appointment.BarberEmail} at {appointment.AppointmentDate}");
        }

        public async Task<List<Appointment>> GetByCustomerEmailAsync(string email)
        {
            return await _appointmentRepository.GetByCustomerEmailAsync(email);
        }

        public async Task<List<Appointment>> GetByBarberEmailAsync(string email)
        {
            return await _appointmentRepository.GetByBarberEmailAsync(email);
        }

        public async Task CancelAsync(int id)
        {
            await _appointmentRepository.DeleteByIdAsync(id);
            AppLogger.Info($"Appointment cancelled: ID {id}");
        }
    }
}