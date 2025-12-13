using Barbershop.EntityLayer;
using Barbershop.NetworkingLayer;
using Barbershop.RepositoryLayer;
using Barbershop.Utils;
using Barbershop.Utils.Exceptions;

namespace Barbershop.DomainLayer
{
    public sealed class BarberDomain : IUserDomain<Barber>
    {
        private readonly IUserRepository<Barber> _barberRepository;
        private readonly IEmailVerifier _emailVerifier;

        public BarberDomain(IUserRepository<Barber> barberRepository, IEmailVerifier emailVerifier)
        {
            _barberRepository = barberRepository;
            _emailVerifier = emailVerifier;
        }

        public void Register(Barber barber, string plainPassword)
        {

            if (_barberRepository.GetByEmail(barber.Email) != null)
              throw new UserAlreadyExistsException("A barber with this email already exists.");
            

            if (barber.Salary < 0) 
                throw new InvalidSalaryException("Salary cannot be negative.");
            

            if (barber.Email.Count() < 5)
                throw new InvalidEmailException("Email too short!");
            

            barber.PasswordHash = SecurityUtils.Hash(plainPassword);
            barber.IsActive = true;
            _barberRepository.Add(barber);
        }
        public Barber Login(string email, string password)
        {
            var barber = _barberRepository.GetByEmail(email);

            if (barber == null)
                throw new UserNotFoundException("Barber not found.");


            if (!barber.IsActive)
                throw new AuthenticationFailedException("Barber account is inactive.");


            string inputHash = SecurityUtils.Hash(password);
            if (barber.PasswordHash == inputHash)
            {
                return barber;
            }

            throw new AuthenticationFailedException("Invalid password.");
        }
        public void UpdateStatus(string email, bool isActive)
        {
            var client = _barberRepository.GetByEmail(email);
            if (client == null)
            {
                throw new Exception("Client not found.");
            }

            _barberRepository.UpdateStatus(client.Email, isActive);
        }
        public void Delete(string email)
        {
            var client = _barberRepository.GetByEmail(email);
            if (client == null)
            {
                throw new Exception("Client not found.");
            }

            _barberRepository.Delete(client.Email);
        }
    }
}
