using System;
using System.Collections.Generic;

namespace CobaHW7.Class
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        // CATATAN: untuk demo saja; di produksi gunakan hash+salt.
        public string Password { get; set; }

        public bool IsAdmin { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public bool Login(string email, string password)
            => string.Equals(Email, email, StringComparison.OrdinalIgnoreCase) && Password == password;

        public bool Register(string name, string email, string password)
        {
            Name = name; Email = email; Password = password;
            CreatedAt = DateTime.Now; UpdatedAt = DateTime.Now;
            return true;
        }

        public bool Logout(string token) => true;
        public bool VerifyEmail(string token) => true;
        public bool DeleteAccount(int userId) => true;

        public bool PromoteToAdmin(int userId)
        {
            if (Id == userId) { IsAdmin = true; UpdatedAt = DateTime.Now; return true; }
            return false;
        }

        // Stub: integrasikan dengan storage kamu
        public List<Booking> GetUserBookings(int userId, int page, int size) => new();
    }
}
