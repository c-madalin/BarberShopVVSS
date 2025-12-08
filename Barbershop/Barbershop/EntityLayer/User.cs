using Barbershop.EntityLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.EntityLayer
{
    public abstract class User
    {
        #region DB Items
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; } = true;
        #endregion


        public UserRole Role { get; set; }
        public string FullName => $"{FirstName} {LastName}";    }
}
