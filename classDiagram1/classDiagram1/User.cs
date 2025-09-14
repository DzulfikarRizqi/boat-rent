using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace classDiagram1
{
    internal class User
    {
        // instance Variable
        private int _ID;
        private string _name;
        private string _email;
        private string _password;
        private bool _is_admin;
        private DateTime _created_at;
        private DateTime _updated_at;

        // Define property
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string email
        {
            get { return _email; }
            set { _email = value; }
        }
        public string password
        {
            get { return _password; }
            set { _password = value; }
        }
        public bool is_admin { 
            get { return _is_admin; }
            set { _is_admin = value; }
        }
        public DateTime created_at
        {
            get { return _created_at; }
            set { _created_at = value; }
        }
        public DateTime updated_at
        {
            get { return _updated_at; }
            set { _updated_at = value; }
        }


        // Define methods
        public bool login (string emailParam, string passwordParam)
        {
            // TODO: implementasi login
            return email == emailParam && password == passwordParam;
        }
        public bool register(string nameParam, string emailParam, string passwordParam)
        {
            // TODO: simpan user baru
            this.name = nameParam;
            this.email = emailParam;
            this.password = passwordParam;
            this.created_at = DateTime.Now;
            this.updated_at = DateTime.Now;
            return true;
        }
        public bool logout(string token)
        {
            // TODO: implementasi logout (misalnya hapus token session)
            return true;
        }

        public bool verifyEmail(string token)
        {
            // TODO: verifikasi email pakai token
            return true;
        }

        public bool deleteAccount(int userId)
        {
            // TODO: hapus akun dari database
            return true;
        }
        public bool PromoteToAdmin(int userId)
        {
            // TODO: jadikan user admin
            if (this.ID == userId)
            {
                this.is_admin = true;
                return true;
            }
            return false;
        }
        public List<Booking> GetUserBookings(int userId, int page, int size)
        {
            // TODO: ambil daftar booking dari database
            return new List<Booking>();
        }
    }
    public class Booking
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
