using Barbershop.EntityLayer;
using Barbershop.NetworkingLayer;
using Barbershop.RepositoryLayer;
using Barbershop.Utils;
using Barbershop.Utils.Exceptions;
using Barbershop.Utils.Logging;
using System.Threading.Tasks;

namespace Barbershop.DomainLayer
{
    public sealed class ClientDomain : IUserDomain<Client>
    {
        private readonly IUserRepository<Client> _clientRepository;
        public ClientDomain(IUserRepository<Client> clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task RegisterAsync(Client client, string plainPassword)
        {
            if (await _clientRepository.GetByEmailAsync(client.Email) != null)
            {
                AppLogger.Warn($"Domain validation failed: Client {client.Email} already exists.");
                throw new UserAlreadyExistsException("A client with this email already exists.");
            }

            client.PasswordHash = SecurityUtils.Hash(plainPassword);
            client.IsActive = true;

            await _clientRepository.AddAsync(client);
            AppLogger.Info($"Client domain logic complete. User persisted: {client.Email}");
        }

        public async Task<Client> LoginAsync(string email, string password)
        {
            var client = await _clientRepository.GetByEmailAsync(email);

            if (client == null)
            {
                AppLogger.Warn($"Login failed (Domain): Client not found - {email}");
                throw new UserNotFoundException("Client not found.");
            }

            if (!client.IsActive)
            {
                AppLogger.Warn($"Login blocked (Domain): Inactive account - {email}");
                throw new AuthenticationFailedException("Client account is inactive.");
            }

            string inputHash = SecurityUtils.Hash(password);
            if (client.PasswordHash != inputHash)
            {
                AppLogger.Warn($"Login failed (Domain): Invalid password hash for {email}");
                throw new AuthenticationFailedException("Invalid password.");
            }

            return client;
        }

        public async Task UpdateStatusAsync(string email, bool isActive)
        {
            var client = await _clientRepository.GetByEmailAsync(email);
            if (client == null)
            {
                AppLogger.Warn($"Update Status failed: Client {email} not found.");
                throw new UserNotFoundException("Client not found.");
            }

            await _clientRepository.UpdateStatusAsync(email, isActive);
            AppLogger.Info($"Client status changed: {email} -> {(isActive ? "Active" : "Inactive")}");
        }

        public async Task DeleteAsync(string email)
        {
            var client = await _clientRepository.GetByEmailAsync(email);
            if (client == null)
            {
                AppLogger.Warn($"Delete failed: Client {email} not found.");
                throw new UserNotFoundException("Client not found.");
            }

            await _clientRepository.DeleteAsync(email);
            AppLogger.Info($"Client deleted (Domain): {email}");
        }
    }
}