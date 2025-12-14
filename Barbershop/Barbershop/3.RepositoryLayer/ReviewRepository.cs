using Barbershop.EntityLayer;
using Barbershop.IntegrationLayer;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace Barbershop.RepositoryLayer
{
    public sealed class ReviewRepository : IReviewRepository
    {
        public void Add(Review review)
        {
            using var conn = DbContext.GetConnection();
            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.Reviews (AppointmentId, ClientEmail, BarberEmail, Rating, Comment, DatePosted)
                VALUES (@AppointmentId, @ClientEmail, @BarberEmail, @Rating, @Comment, @DatePosted);", conn);

            cmd.Parameters.AddWithValue("@AppointmentId", review.AppointmentId);
            cmd.Parameters.AddWithValue("@ClientEmail", review.ClientEmail);
            cmd.Parameters.AddWithValue("@BarberEmail", review.BarberEmail);
            cmd.Parameters.AddWithValue("@Rating", review.Rating);
            cmd.Parameters.AddWithValue("@Comment", review.Comment ?? (object)DBNull.Value); // Handle null comments
            cmd.Parameters.AddWithValue("@DatePosted", review.DatePosted);

            cmd.ExecuteNonQuery();
        }

        public List<Review> GetByBarberEmail(string barberEmail)
        {
            var list = new List<Review>();
            using var conn = DbContext.GetConnection();

            // Putem aduce recenziile sortate descrescator dupa data (cele mai noi primele)
            string sql = @"
                SELECT ReviewId, AppointmentId, ClientEmail, BarberEmail, Rating, Comment, DatePosted
                FROM dbo.Reviews
                WHERE BarberEmail = @Email
                ORDER BY DatePosted DESC";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", barberEmail);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }
            return list;
        }

        public Review GetById(int id)
        {
            using var conn = DbContext.GetConnection();
            using var cmd = new SqlCommand(@"
                SELECT ReviewId, AppointmentId, ClientEmail, BarberEmail, Rating, Comment, DatePosted
                FROM dbo.Reviews
                WHERE ReviewId = @Id", conn);

            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return Map(reader);
            }
            return null;
        }

        public bool HasReviewForAppointment(int appointmentId)
        {
            using var conn = DbContext.GetConnection();
            using var cmd = new SqlCommand("SELECT COUNT(1) FROM dbo.Reviews WHERE AppointmentId = @AppId", conn);

            cmd.Parameters.AddWithValue("@AppId", appointmentId);

            var count = (int)cmd.ExecuteScalar();
            return count > 0;
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