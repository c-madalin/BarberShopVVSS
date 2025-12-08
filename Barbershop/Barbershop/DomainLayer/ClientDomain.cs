using Barbershop.EntityLayer;
using Barbershop.NetworkingLayer;
using Barbershop.RepositoryLayer;
using Barbershop.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.DomainLayer
{
    internal sealed class ClientDomain: IUserDomain<Client>
    {
        private readonly IUserRepository<Client> _clientRepository;
        private readonly IEmailVerifier _emailVerifier;

        public ClientDomain(IUserRepository<Client> clientRepository, IEmailVerifier emailVerifier)
        {
            _clientRepository = clientRepository;
            _emailVerifier = emailVerifier;
        }

        public void Register(Client client, string plainPassword)
        {
            if (_clientRepository.GetByEmail(client.Email) != null)
            {
                throw new Exception("A client with this email already exists.");
            }

            client.PasswordHash = SecurityUtils.Hash(plainPassword);
            client.IsActive = true;

            _clientRepository.Add(client);
        }

        public Client Login(string email, string password)
        {
            var client = _clientRepository.GetByEmail(email);

            if (client == null || !client.IsActive)
            {
                return null;
            }
            string inputHash = SecurityUtils.Hash(password);
            if (client.PasswordHash == inputHash)
            {
                return client;
            }

            return null;
        }
    }
}

