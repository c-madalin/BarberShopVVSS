using Barbershop.EntityLayer;
using Barbershop.IntegrationLayer;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Barbershop.RepositoryLayer
{
    public sealed class ReviewRepository : IReviewRepository
    {
        public async Task AddAsync(Review review)
        {
            // CORECTIE
            using var conn = DbContext.CreateConnection();
            // await conn.OpenAsync();

            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.Reviews (AppointmentId, ClientEmail, BarberEmail, Rating, Comment, DatePosted)
                VALUES (@AppointmentId, @ClientEmail, @BarberEmail, @Rating, @Comment, @DatePosted);", conn);

            cmd.Parameters.AddWithValue("@AppointmentId", review.AppointmentId);
            cmd.Parameters.AddWithValue("@ClientEmail", review.ClientEmail);
            cmd.Parameters.AddWithValue("@BarberEmail", review.BarberEmail);
            cmd.Parameters.AddWithValue("@Rating", review.Rating);
            cmd.Parameters.AddWithValue("@Comment", review.Comment ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DatePosted", review.DatePosted);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<Review>> GetByBarberEmailAsync(string barberEmail)
        {
            var list = new List<Review>();

            // CORECTIE
            using var conn = DbContext.CreateConnection();

            string sql = @"
                SELECT ReviewId, AppointmentId, ClientEmail, BarberEmail, Rating, Comment, DatePosted
                FROM dbo.Reviews
                WHERE BarberEmail = @Email
                ORDER BY DatePosted DESC";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", barberEmail);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(Map(reader));
            }
            return list;
        }

        public async Task<bool> HasReviewForAppointmentAsync(int appointmentId)
        {
            // CORECTIE
            using var conn = DbContext.CreateConnection();

            using var cmd = new SqlCommand("SELECT COUNT(1) FROM dbo.Reviews WHERE AppointmentId = @AppId", conn);
            cmd.Parameters.AddWithValue("@AppId", appointmentId);

            var count = await cmd.ExecuteScalarAsync();
            return (int)count > 0;
        }

        private Review Map(SqlDataReader reader)
        {
            return new Review
            {
                ReviewId = (int)reader["ReviewId"],
                AppointmentId = (int)reader["AppointmentId"],
                ClientEmail = reader["ClientEmail"].ToString(),
                BarberEmail = reader["BarberEmail"].ToString(),
                Rating = (int)reader["Rating"],
                Comment = reader["Comment"] != DBNull.Value ? reader["Comment"].ToString() : string.Empty,
                DatePosted = (DateTime)reader["DatePosted"]
            };
        }
    }
}