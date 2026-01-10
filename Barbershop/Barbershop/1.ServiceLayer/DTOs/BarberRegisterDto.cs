using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop._1.ServiceLayer.DTOs
{
    public record BarberRegisterDto(
           string FirstName,
           string LastName,
           string Email,
           string Phone,
           string Password,
           string Specialisation,
           decimal Salary
       );
}
