using Barbershop.DomainLayer;
using Barbershop.EntityLayer;
using Barbershop.NetworkingLayer;
using System;
using System.Threading.Tasks;

namespace Barbershop.ServiceLayer
{
    public class BarberService : IUserService<Barber> 
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
            if (!await _emailVerifier.IsValidEmailAsync(email))
            {
                throw new Exception("Email invalid.");
            }

            var barber = new Barber
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phone,
                Specialisation = specialisation,
                Salary = salary,
                IsActive = true
            };

            _barberDomain.Register(barber, password);
        }

        public Barber Login(string email, string password)
        {
            return _barberDomain.Login(email, password);
        }
    }
}