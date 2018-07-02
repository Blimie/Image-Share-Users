using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Homework180502.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    }
    public class ImageLoginDB
    {
        private string _connectionString;   
        public ImageLoginDB(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void AddUser(User user, string password)
        {
            string passwordSalt = PasswordHelper.GenerateSalt();
            string passwordHash = PasswordHelper.HashPassword(password, passwordSalt);
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Users (Name, Email, PasswordHash, PasswordSalt) 
                                    VALUES (@name, @email, @passwordHash, @passwordSalt)";
                cmd.Parameters.AddWithValue("@name", user.Name);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                cmd.Parameters.AddWithValue("@passwordSalt", passwordSalt);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }                    
        public User Login(string email, string password)
        {
            User user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }
            bool isCorrectPassword = PasswordHelper.PasswordMatch(password, user.PasswordSalt, user.PasswordHash);
            if (isCorrectPassword)
            {
                return user;
            }
            return null;
        }
        public User GetByEmail(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP 1 * FROM Users WHERE Email = @email";
                cmd.Parameters.AddWithValue("@email", email);
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                return new User
                {
                    Email = email,
                    Name = (string)reader["Name"],
                    Id = (int)reader["Id"],
                    PasswordHash = (string)reader["PasswordHash"],
                    PasswordSalt = (string)reader["PasswordSalt"],
                };
            }
        }
        public void AddImage(Image image)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Images(FileName, Password, ViewsCount, UserId) 
                                    VALUES(@fileName, @password, 0, @userId)
                                    SELECT CAST(SCOPE_IDENTITY() AS INT)";
                cmd.Parameters.AddWithValue("@fileName", image.FileName);
                cmd.Parameters.AddWithValue("@password", image.Password);
                cmd.Parameters.AddWithValue("@userId", image.UserId);
                connection.Open();
                image.Id = (int)cmd.ExecuteScalar();
            }
        }
        public void DeleteImage(int imageId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM Images WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", imageId);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void UpdateViewsCount(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"UPDATE Images SET ViewsCount = ViewsCount + 1 WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public Image GetImage(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Images WHERE id = @id";
                cmd.Parameters.AddWithValue("@Id", id);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                return new Image
                {
                    Id = (int)reader["Id"],
                    FileName = (string)reader["FileName"],
                    Password = (string)reader["Password"],
                    ViewsCount = (int)reader["ViewsCount"]
                };
            }
        }
        public IEnumerable<Image> GetImagesByUserId(int userId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Images WHERE UserId = @id";
                cmd.Parameters.AddWithValue("@id", userId);
                connection.Open();
                List<Image> list = new List<Image>();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Image
                    {
                        Id = (int)reader["Id"],
                        FileName = (string)reader["FileName"],
                        Password = (string)reader["Password"],
                        ViewsCount = (int)reader["ViewsCount"]
                    });
                }
                return list;
            }
        }            
    }      
}
