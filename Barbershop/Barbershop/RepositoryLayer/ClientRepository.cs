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
    internal sealed class ClientRepository: IUserRepository<Client>
    {
        public void Add(Client client)
        {
            using (var conn = DbContext.GetConnection())
            {
                using (var cmd = new SqlCommand("sp_InsertClient", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FirstName", client.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", client.LastName);
                    cmd.Parameters.AddWithValue("@Email", client.Email);
                    cmd.Parameters.AddWithValue("@PhoneNumber", client.PhoneNumber);
                    cmd.Parameters.AddWithValue("@PasswordHash", client.PasswordHash);
                    cmd.Parameters.AddWithValue("@IsActive", client.IsActive);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public Client GetByEmail(string email)
        {
            using (var conn = DbContext.GetConnection())
            {
                using (var cmd = new SqlCommand("sp_GetClientByEmail", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Email", email);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Client
                            {
                                Id = (int)reader["Id"],
                                FirstName = (string)reader["FirstName"] ??          throw new InvalidInsertFieldException("FirstName cannot be null."),
                                LastName = (string)reader["LastName"] ??            throw new InvalidInsertFieldException("LastName cannot be null."),
                                Email = (string)reader["Email"] ??                  throw new InvalidInsertFieldException("Email cannot be null."),
                                PhoneNumber = (string)reader["PhoneNumber"] ??      throw new InvalidInsertFieldException("PhoneNumber cannot be null."),
                                PasswordHash = (string)reader["PasswordHash"] ??    throw new InvalidInsertFieldException("PasswordHash cannot be null."),
                                IsActive = (bool)reader["IsActive"]
                            };
                        }
                        throw new UserNotFoundException("Client not found.");
                    }
                }
            }
        }
    }
}
