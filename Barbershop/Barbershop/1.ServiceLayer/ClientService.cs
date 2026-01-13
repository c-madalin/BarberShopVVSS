using Barbershop._1.ServiceLayer.DTOs;
using Barbershop.DomainLayer;
using Barbershop.EntityLayer;
using Barbershop.NetworkingLayer;
using Barbershop.Utils.Logging;
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

        public async Task NewRegisterAsync(ClientRegisterDto dto)
        {
            AppLogger.Info($"Registering new client: {dto.Email}");

            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                AppLogger.Warn($"Registration failed: First name empty for {dto.Email}");
                throw new ArgumentException("First name cannot be empty.", nameof(dto.FirstName));
            }

            if (string.IsNullOrWhiteSpace(dto.LastName))
            {
                AppLogger.Warn($"Registration failed: Last name empty for {dto.Email}");
                throw new ArgumentException("Last name cannot be empty.", nameof(dto.LastName));
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                AppLogger.Warn($"Registration failed: Email empty");
                throw new ArgumentException("Email cannot be empty.", nameof(dto.Email));
            }

            if (string.IsNullOrWhiteSpace(dto.Phone))
            {
                AppLogger.Warn($"Registration failed: Phone number empty for {dto.Email}");
                throw new ArgumentException("Phone number cannot be empty.", nameof(dto.Phone));
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                AppLogger.Warn($"Registration failed: Password empty for {dto.Email}");
                throw new ArgumentException("Password cannot be empty.", nameof(dto.Password));
            }

            if (dto.Password.Length < 8)
            {
                AppLogger.Warn($"Registration failed: Password too short for {dto.Email}");
                throw new ArgumentException("Password must be at least 8 characters long.", nameof(dto.Password));
            }

            try
            {
                AppLogger.Info($"Verifying email for: {dto.Email}");
                if (!await _emailVerifier.IsValidEmailAsync(dto.Email))
                {
                    AppLogger.Warn($"Invalid email detected: {dto.Email}");
                    throw new Exception("Email address is invalid or does not exist.");
                }

                var client = new Client
                {
                    FirstName   = dto.FirstName.Trim(),
                    LastName    = dto.LastName.Trim(),
                    Email       = dto.Email.Trim(),
                    PhoneNumber = dto.Phone.Trim(),
                    IsActive    = true
                };

               await  _clientDomain.RegisterAsync(client, dto.Password);

                AppLogger.Info($"Client registration completed for: {dto.Email}");
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Registration exception for {dto.Email}: {ex.Message}");
                throw;
            }
        }

        public async Task<Client> LoginAsync(string email, string password)
        {
            AppLogger.Info($"Login attempt for: {email}");

            if (string.IsNullOrWhiteSpace(email))
            {
                AppLogger.Warn($"Login failed: Email empty");
                throw new ArgumentException("Email is required.", nameof(email));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                AppLogger.Warn($"Login failed: Password empty for {email}");
                throw new ArgumentException("Password is required.", nameof(password));
            }

            try
            {
                if (!await _emailVerifier.IsValidEmailAsync(email))
                {
                    AppLogger.Warn($"Login failed: Invalid email format for {email}");
                    throw new Exception("Email address is invalid or does not exist.");
                }

                var client = await _clientDomain.LoginAsync(email, password);

                AppLogger.Info($"Client {email} logged in successfully.");
                return client;
            }
            catch (Exception ex)
            {
                AppLogger.Warn($"Login failed for {email}: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(string email)
        {
            AppLogger.Info($"Delete requested for: {email}");

            if (string.IsNullOrWhiteSpace(email))
            {
                AppLogger.Warn($"Delete failed: Email empty");
                throw new ArgumentException("Email is required.", nameof(email));
            }

            try
            {
                await _clientDomain.DeleteAsync(email);
                AppLogger.Info($"Client deleted: {email}");
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Delete exception for {email}: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateStatusAsync(string email)
        {
            AppLogger.Info($"Status update requested for: {email}");

            if (string.IsNullOrWhiteSpace(email))
            {
                AppLogger.Warn($"Status update failed: Email empty");
                throw new ArgumentException("Email is required.", nameof(email));
            }

            try
            {
                await _clientDomain.UpdateStatusAsync(email, false);
                AppLogger.Info($"Client status updated: {email}");
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Status update exception for {email}: {ex.Message}");
                throw;
            }
        }
    }
}