using BarbershopVVSS.DomainLayer;
using BarbershopVVSS.EntityLayer;
using BarbershopVVSS.NetworkingLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbershopVVSS.ServiceLayer
{
    internal class ClientService: IService<Client>
    {
        private readonly IDomain<Client> _clientDomain;
        private readonly IEmailVerifier _emailVerifier;

        public ClientService(ClientDomain clientDomain, IEmailVerifier emailVerifier)
        {
            _clientDomain = clientDomain;
            _emailVerifier = emailVerifier;
        }

        public async Task NewRegister(string firstName, string lastName, string email, string phone, string password)
        {
            if (!await _emailVerifier.IsValidEmailAsync(email))
            {
                throw new Exception("Email does not exist or is invalid.");
            }


            var newClient = new Client
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phone
            };

            _clientDomain.Register(newClient, password);
        }

        public Client Login(string email, string password)
        {
            return _clientDomain.Login(email, password);
        }
    }
}
