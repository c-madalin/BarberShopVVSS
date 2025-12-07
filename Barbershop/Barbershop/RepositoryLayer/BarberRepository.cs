using Barbershop.EntityLayer;
using Barbershop.IntegrationLayer;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.RepositoryLayer
{
    internal sealed class BarberRepository: IUserRepository<Barber>
    {
        public void Add(Barber barber)
        {
            using (var conn = DbContext.GetConnection())
            {
                using (var cmd = new SqlCommand("sp_InsertBarber", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FirstName", barber.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", barber.LastName);
                    cmd.Parameters.AddWithValue("@Email", barber.Email);
                    cmd.Parameters.AddWithValue("@PhoneNumber", barber.PhoneNumber);
                    cmd.Parameters.AddWithValue("@PasswordHash", barber.PasswordHash);
                    cmd.Parameters.AddWithValue("@IsActive", barber.IsActive);
                    cmd.Parameters.AddWithValue("@Specialization", barber.Specialisation);
                    cmd.Parameters.AddWithValue("@Salary", barber.Salary);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public Barber GetByEmail(string email)
        {
            using (var conn = DbContext.GetConnection())
            {
                using (var cmd = new SqlCommand("sp_GetBarberByEmail", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Barber
                            {
                                Id = (int)reader["Id"],
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                PasswordHash = reader["PasswordHash"].ToString(),
                                IsActive = (bool)reader["IsActive"],
                                Specialisation = reader["Specialisation"].ToString(),
                                Salary = (decimal)reader["Salary"]
                            };
                        }
                        throw new Exception("PROVIZORY: NO BARBER FOUND");
                    }
                }
            }
        }
    }
}
