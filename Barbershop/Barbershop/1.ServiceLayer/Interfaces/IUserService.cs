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
        public Task<T> Login(string email, string password);
        public void Delete(string email);
        public void UpdateStatus(string email);
    }
}
