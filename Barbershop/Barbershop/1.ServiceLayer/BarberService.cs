using Barbershop.DomainLayer;
using Barbershop.EntityLayer;
using Barbershop.NetworkingLayer;
using System;
using System.Threading.Tasks;

namespace Barbershop.ServiceLayer
{
    public sealed class BarberService : IUserService<Barber> 
    {
        private readonly IUserDomain<Barber> _barberDomain;
        private readonly IEmailVerifier _emailVerifier;

        public BarberService(IUserDomain<Barber> barberDomain, IEmailVerifier emailVerifier)
        {
            _barberDomain = barberDomain;
            _emailVerifier = emailVerifier;
        }

        public async Task NewRegister(string firstName,
           string lastName,
           string email,
           string phone,
           string password,
           string specialisation,
           decimal salary)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty.", nameof(firstName));

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty.", nameof(lastName));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.", nameof(email));

            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone number cannot be empty.", nameof(phone));

            if (string.IsNullOrWhiteSpace(specialisation))
                throw new ArgumentException("Specialisation cannot be empty.", nameof(specialisation));

            if (salary < 0)
                throw new ArgumentOutOfRangeException(nameof(salary), "Salary cannot be negative.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.", nameof(password));

            if (password.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters long.", nameof(password));

            if (!await _emailVerifier.IsValidEmailAsync(email))
                throw new Exception("Email address is invalid.");
            

            var barber = new Barber
            {
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Email = email.Trim(),
                PhoneNumber = phone.Trim(),
                Specialisation = specialisation.Trim(),
                Salary = salary,
                IsActive = true
            };

            _barberDomain.Register(barber, password);
        }

        public async Task<Barber> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));

            if (!await _emailVerifier.IsValidEmailAsync(email))
                throw new Exception("Email address is invalid.");

            return _barberDomain.Login(email, password);
        }

        public void Delete(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            _barberDomain.Delete(email);
        }

        public void UpdateStatus(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            _barberDomain.UpdateStatus(email, false);
        }
    }
}