using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop._1.ServiceLayer.DTOs
{
    public record ClientRegisterDto(
           string FirstName,
           string LastName,
           string Email,
           string Phone,
           string Password
       );
}
