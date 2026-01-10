using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.RepositoryLayer
{
    public interface IUserRepository<T> where T: class
    {
        public Task AddAsync(T user);
        public Task<T?> GetByEmailAsync(string email);
        public Task UpdateStatusAsync(string email, bool isActive); // SOFT DELETE
        public Task DeleteAsync(string email); // HARD DELETE
    }
}
