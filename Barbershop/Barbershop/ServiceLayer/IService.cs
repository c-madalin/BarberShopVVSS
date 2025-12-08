using BarbershopVVSS.EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbershopVVSS.ServiceLayer
{
    public interface IService<T> where T : class
    {
        public T Login(string email, string password);
    }
}
