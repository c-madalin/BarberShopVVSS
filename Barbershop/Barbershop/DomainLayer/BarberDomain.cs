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
    internal sealed class BarberDomain: IUserDomain<Barber>
    {
        private readonly IUserRepository<Barber> _barberRepository;
        private readonly IEmailVerifier _emailVerifier;

        public BarberDomain(IUserRepository<Barber> barberRepository, IEmailVerifier emailVerifier)
        {
            _barberRepository = barberRepository;
            _emailVerifier = emailVerifier;
        }

        public void Register(Barber barber, string plainPassword)
        {

            if (_barberRepository.GetByEmail(barber.Email) != null)
            {
                throw new Exception("A barber with this email already exists.");
            }

            if (barber.Salary < 0)
            {
                throw new Exception("Salary cannot be negative.");
            }

            if (barber.Email.Count() < 5)
            {
                throw new Exception("Email too short!");
            }

            barber.PasswordHash = SecurityUtils.Hash(plainPassword);
            barber.IsActive = true;
            _barberRepository.Add(barber);
        }

        public Barber Login(string email, string password)
        {
            var barber = _barberRepository.GetByEmail(email);

            if (barber == null || !barber.IsActive)
            {
                return null;
            }

            string inputHash = SecurityUtils.Hash(password);
            if (barber.PasswordHash == inputHash)
            {
                return barber;
            }

            return null;
        }
    }
}
