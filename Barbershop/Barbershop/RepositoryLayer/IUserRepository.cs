using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.RepositoryLayer
{
    public interface IUserRepository<T> where T: class
    {
        public void Add(T user);
        public T GetByEmail(string email);
    }
}
