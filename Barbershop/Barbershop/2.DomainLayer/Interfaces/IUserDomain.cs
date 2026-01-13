using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.DomainLayer
{
    public interface IUserDomain<T> where T : class
    {
        public Task RegisterAsync(T client, string plainPassword);
        public Task<T> LoginAsync(string email, string password);
        public Task UpdateStatusAsync(string email, bool isActive);
        public Task DeleteAsync(string email);
    }
}
