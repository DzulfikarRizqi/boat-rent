using System;
using System.Data;                 // DuplicateNameException
using System.Threading.Tasks;
using Npgsql;
using CobaHW7.Class;               // model User { Id, Name, Email, Password, IsAdmin }

namespace CobaHW7.Data
{
    /// <summary>
    /// Repository untuk tabel public.users
    /// Kolom: id, username(varchar), email(citext), password(varchar), is_admin(bool)
    /// </summary>
    public class UserRepository
    {
        // Nama kolom sesuai skema DB kamu:
        private const string NameColumn = "username";
        private const string PassColumn = "password";

        // =========================
        // Helpers
        // =========================
        private static User MapUser(NpgsqlDataReader rd) => new User
        {
            Id = rd.GetInt32(0),
            Name = rd.IsDBNull(1) ? "" : rd.GetString(1),
            Email = rd.IsDBNull(2) ? "" : rd.GetString(2),
            // TRIM untuk jaga-jaga jika ada spasi tersisa dari tipe CHAR(n)/input lama
            Password = rd.IsDBNull(3) ? "" : rd.GetString(3).Trim(),
            IsAdmin = rd.GetBoolean(4)
        };

        // =========================
        // READ (Login by username/email)
        // =========================
        /// <summary>
        /// Ambil user berdasarkan identifier (boleh username ATAU email).
        /// Email case-insensitive (citext). Query cek keduanya sekaligus (OR).
        /// </summary>
        public async Task<User?> GetByLoginAsync(string identifier)
        {
            using var con = Db.GetOpenConnection();

            string sql = $@"
                SELECT id,
                       {NameColumn} AS uname,
                       email,
                       TRIM(BOTH FROM {PassColumn}) AS pwd,
                       COALESCE(is_admin,false)
                FROM users
                WHERE email = @v OR {NameColumn} = btrim(@v)
                LIMIT 1;";

            using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@v", (identifier ?? "").Trim());

            using var rd = await cmd.ExecuteReaderAsync();
            if (!await rd.ReadAsync()) return null;

            return MapUser(rd);
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            using var con = Db.GetOpenConnection();
            string sql = $@"
                SELECT id, {NameColumn}, email, TRIM(BOTH FROM {PassColumn}), COALESCE(is_admin,false)
                FROM users
                WHERE email = @e
                LIMIT 1;";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@e", (email ?? "").Trim());
            using var rd = await cmd.ExecuteReaderAsync();
            if (!await rd.ReadAsync()) return null;
            return MapUser(rd);
        }

        public async Task<User?> FindByUsernameAsync(string username)
        {
            using var con = Db.GetOpenConnection();
            string sql = $@"
                SELECT id, {NameColumn}, email, TRIM(BOTH FROM {PassColumn}), COALESCE(is_admin,false)
                FROM users
                WHERE {NameColumn} = btrim(@u)
                LIMIT 1;";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@u", (username ?? "").Trim());
            using var rd = await cmd.ExecuteReaderAsync();
            if (!await rd.ReadAsync()) return null;
            return MapUser(rd);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using var con = Db.GetOpenConnection();
            string sql = $@"
                SELECT id, {NameColumn}, email, TRIM(BOTH FROM {PassColumn}), COALESCE(is_admin,false)
                FROM users
                WHERE id = @id
                LIMIT 1;";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", id);
            using var rd = await cmd.ExecuteReaderAsync();
            if (!await rd.ReadAsync()) return null;
            return MapUser(rd);
        }

        // =========================
        // VERIFY
        // =========================
        /// <summary>
        /// Verifikasi password polos (tanpa hash). Nanti kalau pakai hash, ganti implementasi ini.
        /// </summary>
        public bool VerifyPassword(User user, string plain)
        {
            var db = (user?.Password ?? "").Trim();
            var input = (plain ?? "").Trim();
            return string.Equals(db, input, StringComparison.Ordinal);
        }

        // =========================
        // EXISTS
        // =========================
        public async Task<bool> UsernameExistsAsync(string username)
        {
            using var con = Db.GetOpenConnection();
            using var cmd = new NpgsqlCommand(
                $"SELECT 1 FROM users WHERE {NameColumn}=btrim(@u) LIMIT 1;", con);
            cmd.Parameters.AddWithValue("@u", (username ?? "").Trim());
            using var rd = await cmd.ExecuteReaderAsync();
            return await rd.ReadAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            using var con = Db.GetOpenConnection();
            using var cmd = new NpgsqlCommand(
                "SELECT 1 FROM users WHERE email=@e LIMIT 1;", con); // citext: case-insensitive
            cmd.Parameters.AddWithValue("@e", (email ?? "").Trim());
            using var rd = await cmd.ExecuteReaderAsync();
            return await rd.ReadAsync();
        }

        // =========================
        // CREATE / UPDATE
        // =========================
        public async Task<int> CreateAsync(User u)
        {
            using var con = Db.GetOpenConnection();
            string sql = $@"
                INSERT INTO users({NameColumn}, email, {PassColumn}, is_admin)
                VALUES(@n, @e, @p, @a)
                RETURNING id;";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@n", (u.Name ?? "").Trim());
            cmd.Parameters.AddWithValue("@e", (u.Email ?? "").Trim());
            cmd.Parameters.AddWithValue("@p", (u.Password ?? "").Trim());   // TODO: ganti ke hash
            cmd.Parameters.AddWithValue("@a", u.IsAdmin);

            try
            {
                var idObj = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(idObj);
            }
            catch (PostgresException ex) when (ex.SqlState == "23505") // unique_violation
            {
                // Deteksi constraint mana yang kena (email / username)
                if (string.Equals(ex.ConstraintName, "users_email_key", StringComparison.Ordinal))
                    throw new DuplicateNameException("EMAIL_TAKEN");
                if (string.Equals(ex.ConstraintName, "users_username_key", StringComparison.Ordinal))
                    throw new DuplicateNameException("USERNAME_TAKEN");
                throw;
            }
        }

        public async Task<int> UpdatePasswordAsync(int userId, string newPlainPassword)
        {
            using var con = Db.GetOpenConnection();
            string sql = $@"UPDATE users SET {PassColumn}=@p WHERE id=@id;";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@p", (newPlainPassword ?? "").Trim());
            cmd.Parameters.AddWithValue("@id", userId);
            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
