using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.NetworkingLayer
{
    public interface IEmailVerifier
    {
        Task<bool> IsValidEmailAsync(string email);
    }
}
