using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.DomainLayer
{
    internal interface IUserDomain<T> where T : class
    {
        public void Register(T client, string plainPassword);
        public T Login(string email, string password);
    }
}
