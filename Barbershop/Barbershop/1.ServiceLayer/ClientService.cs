using Barbershop.DomainLayer;
using Barbershop.EntityLayer;
using Barbershop.NetworkingLayer;
using System;
using System.Threading.Tasks;

namespace Barbershop.ServiceLayer
{
    public sealed class ClientService : IUserService<Client>
    {
        private readonly IUserDomain<Client> _clientDomain;
        private readonly IEmailVerifier _emailVerifier;

        public ClientService(IUserDomain<Client> clientDomain, IEmailVerifier emailVerifier)
        {
            _clientDomain = clientDomain;
            _emailVerifier = emailVerifier;
        }

        public async Task NewRegister(string firstName,
            string lastName,
            string email,
            string phone,
            string password)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty.", nameof(firstName));

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty.", nameof(lastName));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.", nameof(email));

            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone number cannot be empty.", nameof(phone));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.", nameof(password));

            if (password.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters long.", nameof(password));

            if (!await _emailVerifier.IsValidEmailAsync(email))
                throw new Exception("Email address is invalid or does not exist.");
            

            var client = new Client
            {
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Email = email.Trim(),
                PhoneNumber = phone.Trim(),
                IsActive = true
            };

            _clientDomain.Register(client, password);
        }

        public async Task<Client> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));

            if (!await _emailVerifier.IsValidEmailAsync(email))
                throw new Exception("Email address is invalid or does not exist.");
            

            return _clientDomain.Login(email, password);
        }

        public void Delete(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            _clientDomain.Delete(email);
        }

        public void UpdateStatus(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            _clientDomain.UpdateStatus(email, false);
        }
    }
}