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
    internal class BarberService: IService<Barber>
    {
        private readonly IDomain<Barber> _barberDomain;
        private readonly IEmailVerifier _emailVerifier;
        public BarberService(IDomain<Barber> barberDomain, IEmailVerifier emailVerifier)
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
