using System;
using System.Threading.Tasks;
using Npgsql;
using CobaHW7.Class;  
using System.Security.Cryptography;
using System.Text;

namespace CobaHW7.Data
{
    public class UserRepository
    {
        private string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            using var conn = Db.GetOpenConnection();
            using var cmd = new NpgsqlCommand(
                "SELECT id, username, email, password, is_admin FROM users WHERE username=@u", conn);
            cmd.Parameters.AddWithValue("u", username);

            using var rd = await cmd.ExecuteReaderAsync();
            if (await rd.ReadAsync())
            {
                return new User
                {
                    Id = rd.GetInt32(0),
                    Name = rd.GetString(1),
                    Email = rd.IsDBNull(2) ? "" : rd.GetString(2),
                    Password = rd.GetString(3),
                    IsAdmin = rd.GetBoolean(4)
                };
            }
            return null;
        }

        public async Task<int> CreateAsync(User u)
        {
            using var conn = Db.GetOpenConnection();
            using var cmd = new NpgsqlCommand(
                @"INSERT INTO users(username,email,password,is_admin)
                  VALUES(@un,@em,@pw,@ad) RETURNING id", conn);
            cmd.Parameters.AddWithValue("un", u.Name);
            cmd.Parameters.AddWithValue("em", (object?)u.Email ?? "");
            cmd.Parameters.AddWithValue("pw", Hash(u.Password));  // simpan hash
            cmd.Parameters.AddWithValue("ad", u.IsAdmin);

            var id = (int)await cmd.ExecuteScalarAsync();
            return id;
        }

        public bool VerifyPassword(User u, string plain)
        {
            var h = Hash(plain);
            return string.Equals(h, u.Password, StringComparison.OrdinalIgnoreCase);
        }
    }
}
