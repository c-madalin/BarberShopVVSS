using Barbershop.DomainLayer;
using Barbershop.EntityLayer;
using Barbershop.NetworkingLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.ServiceLayer
{
    internal class BarberService: IUserService<Barber>
    {
        private readonly IUserDomain<Barber> _barberDomain;
        private readonly IEmailVerifier _emailVerifier;
        public BarberService(IUserDomain<Barber> barberDomain, IEmailVerifier emailVerifier)
        {
            _barberDomain = barberDomain;
            _emailVerifier = emailVerifier;
        }

        public async Task NewRegister(string firstName, string lastName, string email, string phone, string password, string specialisation, decimal salary)
        {
            if (!await _emailVerifier.IsValidEmailAsync(email))
            {
                throw new Exception("Email does not exist or is invalid.");
            }

            var newBarber = new Barber
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phone,
                Specialisation = specialisation,
                Salary = salary
            };

            _barberDomain.Register(newBarber, password);
        }

        public Barber Login(string email, string password)
        {
            return _barberDomain.Login(email, password);
        }
    }
}
