using Barbershop.EntityLayer;
using Barbershop.IntegrationLayer;
using Barbershop.Utils.Exceptions;
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
                                FirstName = (string)reader["FirstName"] ??              throw new InvalidInsertFieldException("FirstName cannot be null."),
                                LastName = (string)reader["LastName"] ??                throw new InvalidInsertFieldException("LastName cannot be null."),
                                Email = (string)reader["Email"] ??                      throw new InvalidInsertFieldException("Email cannot be null."),
                                PhoneNumber = (string)reader["PhoneNumber"] ??          throw new InvalidInsertFieldException("PhoneNumber cannot be null."),
                                PasswordHash = (string)reader["PasswordHash"] ??        throw new InvalidInsertFieldException("PasswordHash cannot be null."),
                                IsActive = (bool)reader["IsActive"],
                                Specialisation = (string)reader["Specialisation"] ??    throw new InvalidInsertFieldException("FirstName cannot be null."),
                                Salary = (decimal)reader["Salary"]
                            };
                        }
                        throw new UserNotFoundException("Barber not found.");
                    }
                }
            }
        }
        public void UpdateStatus(string email, bool isActive)
        {
            using (var conn = DbContext.GetConnection())
            {
                using (var cmd = new SqlCommand("sp_Barber_SetStatus", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete(string email)
        {
            using (var conn = DbContext.GetConnection())
            {
                using (var cmd = new SqlCommand("sp_Barber_Delete", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
