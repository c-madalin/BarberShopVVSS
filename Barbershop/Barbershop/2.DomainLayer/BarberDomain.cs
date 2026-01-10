using Barbershop.EntityLayer;
using Barbershop.NetworkingLayer;
using Barbershop.RepositoryLayer;
using Barbershop.Utils;
using Barbershop.Utils.Exceptions;
using Barbershop.Utils.Logging;
using System.Threading.Tasks;

namespace Barbershop.DomainLayer
{
    public sealed class BarberDomain : IUserDomain<Barber>
    {
        private readonly IUserRepository<Barber> _barberRepository;

        public BarberDomain(IUserRepository<Barber> barberRepository)
        {
            _barberRepository = barberRepository;
        }

        public async Task RegisterAsync(Barber barber, string plainPassword)
        {
            if (await _barberRepository.GetByEmailAsync(barber.Email) != null)
            {
                AppLogger.Warn($"Domain validation failed: Barber {barber.Email} already exists.");
                throw new UserAlreadyExistsException("A barber with this email already exists.");
            }

            if (barber.Salary < 0)
            {
                AppLogger.Warn($"Domain validation failed: Negative salary detected for {barber.Email}.");
                throw new InvalidSalaryException("Salary cannot be negative.");
            }
            if (barber.Email.Length < 5)
            {
                AppLogger.Warn($"Domain validation failed: Email too short for {barber.Email}.");
                throw new InvalidEmailException("Email too short!");
            }

            barber.PasswordHash = SecurityUtils.Hash(plainPassword);
            barber.IsActive = true;

            await _barberRepository.AddAsync(barber);
            AppLogger.Info($"Barber domain logic complete. User persisted: {barber.Email}");
        }

        public async Task<Barber> LoginAsync(string email, string password)
        {
            var barber = await _barberRepository.GetByEmailAsync(email);

            if (barber == null)
            {
                AppLogger.Warn($"Login failed (Domain): Barber not found - {email}");
                throw new UserNotFoundException("Barber not found.");
            }

            if (!barber.IsActive)
            {
                AppLogger.Warn($"Login blocked (Domain): Inactive account - {email}");
                throw new AuthenticationFailedException("Barber account is inactive.");
            }

            string inputHash = SecurityUtils.Hash(password);
            if (barber.PasswordHash != inputHash)
            {
                AppLogger.Warn($"Login failed (Domain): Invalid password hash for {email}");
                throw new AuthenticationFailedException("Invalid password.");
            }

            return barber;
        }

        public async Task UpdateStatusAsync(string email, bool isActive)
        {
            var barber = await _barberRepository.GetByEmailAsync(email);
            if (barber == null)
            {
                AppLogger.Warn($"Update Status failed: Barber {email} not found.");
                throw new UserNotFoundException("Barber not found.");
            }

            await _barberRepository.UpdateStatusAsync(email, isActive);
            AppLogger.Info($"Barber status changed: {email} -> {(isActive ? "Active" : "Inactive")}");
        }

        public async Task DeleteAsync(string email)
        {
            var barber = await _barberRepository.GetByEmailAsync(email);
            if (barber == null)
            {
                AppLogger.Warn($"Delete failed: Barber {email} not found.");
                throw new UserNotFoundException("Barber not found.");
            }

            await _barberRepository.DeleteAsync(email);
            AppLogger.Info($"Barber deleted (Domain): {email}");
        }
    }
}