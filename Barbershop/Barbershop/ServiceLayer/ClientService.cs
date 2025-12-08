using Barbershop.DomainLayer;
using Barbershop.EntityLayer;
using Barbershop.NetworkingLayer;
using System;
using System.Threading.Tasks;

namespace Barbershop.ServiceLayer
{
    public class ClientService : IUserService<Client>
    {
        private readonly IUserDomain<Client> _clientDomain;
        private readonly IEmailVerifier _emailVerifier;

        public ClientService(IUserDomain<Client> clientDomain, IEmailVerifier emailVerifier)
        {
            _clientDomain = clientDomain;
            _emailVerifier = emailVerifier;
        }

        public async Task NewRegister(string firstName, string lastName, string email, string phone, string password)
        {
            if (!await _emailVerifier.IsValidEmailAsync(email))
            {
                throw new Exception("Email invalid.");
            }

            var client = new Client
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phone,
                IsActive = true
            };

            _clientDomain.Register(client, password);
        }

        public Client Login(string email, string password)
        {
            return _clientDomain.Login(email, password);
        }
    }
}