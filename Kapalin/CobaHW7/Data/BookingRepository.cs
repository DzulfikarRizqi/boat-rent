using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using CobaHW7.Class;   // model Booking { UserId, BoatId, StartDate, EndDate, TotalAmount, Status, ... }

namespace CobaHW7.Data
{
    // Row view untuk tampilan kartu booking
    public class BookingRow
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public string? UserName { get; set; }

        public int BoatId { get; set; }
        public string BoatName { get; set; } = "";
        public string? ImagePath { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Status { get; set; } = "";
        public decimal TotalAmount { get; set; }
    }

    public class BookingRepository
    {
        // INSERT booking dan kembalikan id
        public async Task<int> CreateAsync(Booking b)
        {
            using var con = Db.GetOpenConnection();
            var sql = @"
        INSERT INTO bookings(user_id, boat_id, start_date, end_date, total_amount, status)
        VALUES(@uid, @bid, @sd, @ed, @amt, COALESCE(@st,'Pending'))
        RETURNING id;";

            using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@uid", b.UserId);
            cmd.Parameters.AddWithValue("@bid", b.BoatId);
            cmd.Parameters.AddWithValue("@sd", b.StartDate);
            cmd.Parameters.AddWithValue("@ed", b.EndDate);
            cmd.Parameters.AddWithValue("@amt", b.TotalAmount);

            // FIX: enum -> string (tanpa operator ?. )
            var statusStr = Enum.GetName(typeof(BookingStatus), b.Status) ?? "Pending";
            cmd.Parameters.AddWithValue("@st", statusStr);

            var idObj = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(idObj);
        }


        // LIST booking milik user tertentu (untuk Dashboard user)
        public async Task<List<BookingRow>> GetByUserAsync(int userId)
        {
            var list = new List<BookingRow>();
            using var con = Db.GetOpenConnection();
            var sql = @"
                SELECT  b.id,
                        u.id           AS user_id,
                        u.username     AS user_name,
                        bo.id          AS boat_id,
                        bo.name        AS boat_name,
                        bo.image_path,
                        b.start_date,
                        b.end_date,
                        b.status,
                        COALESCE(b.total_amount, 0) AS total_amount
                FROM bookings b
                JOIN users   u  ON u.id  = b.user_id
                JOIN boats   bo ON bo.id = b.boat_id
                WHERE b.user_id = @uid
                ORDER BY b.start_date DESC;";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@uid", userId);

            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new BookingRow
                {
                    Id = rd.GetInt32(0),
                    UserId = rd.GetInt32(1),
                    UserName = rd.IsDBNull(2) ? null : rd.GetString(2),
                    BoatId = rd.GetInt32(3),
                    BoatName = rd.GetString(4),
                    ImagePath = rd.IsDBNull(5) ? null : rd.GetString(5),
                    StartDate = rd.GetDateTime(6),
                    EndDate = rd.GetDateTime(7),
                    Status = rd.GetString(8),
                    TotalAmount = rd.GetDecimal(9)
                });
            }
            return list;
        }

        // LIST semua booking (berguna untuk Dashboard Admin)
        public async Task<List<BookingRow>> GetAllAsync()
        {
            var list = new List<BookingRow>();
            using var con = Db.GetOpenConnection();
            var sql = @"
                SELECT  b.id,
                        u.id           AS user_id,
                        u.username     AS user_name,
                        bo.id          AS boat_id,
                        bo.name        AS boat_name,
                        bo.image_path,
                        b.start_date,
                        b.end_date,
                        b.status,
                        COALESCE(b.total_amount, 0) AS total_amount
                FROM bookings b
                JOIN users   u  ON u.id  = b.user_id
                JOIN boats   bo ON bo.id = b.boat_id
                ORDER BY b.start_date DESC;";
            using var cmd = new NpgsqlCommand(sql, con);

            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new BookingRow
                {
                    Id = rd.GetInt32(0),
                    UserId = rd.GetInt32(1),
                    UserName = rd.IsDBNull(2) ? null : rd.GetString(2),
                    BoatId = rd.GetInt32(3),
                    BoatName = rd.GetString(4),
                    ImagePath = rd.IsDBNull(5) ? null : rd.GetString(5),
                    StartDate = rd.GetDateTime(6),
                    EndDate = rd.GetDateTime(7),
                    Status = rd.GetString(8),
                    TotalAmount = rd.GetDecimal(9)
                });
            }
            return list;
        }

        // (Opsional) update status booking
        public async Task<int> UpdateStatusAsync(int bookingId, string newStatus)
        {
            using var con = Db.GetOpenConnection();
            var sql = @"UPDATE bookings SET status=@st WHERE id=@id;";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@st", newStatus ?? "Pending");
            cmd.Parameters.AddWithValue("@id", bookingId);
            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
