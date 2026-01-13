using Barbershop.EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.ServiceLayer
{
    public interface IUserService<T> where T : class
    {
        public Task<T> LoginAsync(string email, string password);
        public Task DeleteAsync(string email);
        public Task UpdateStatusAsync(string email);
    }
}
