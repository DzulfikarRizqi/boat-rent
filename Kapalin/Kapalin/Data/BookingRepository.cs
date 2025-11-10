using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using CobaHW7.Class; 

namespace CobaHW7.Data
{
    public class BookingRepository
    {
        public async Task<int> CreateAsync(Booking b)
        {
            using var conn = Db.GetOpenConnection();
            using var cmd = new NpgsqlCommand(
                @"INSERT INTO bookings(user_id, boat_id, start_date, end_date, total_amount, status, paid_with)
                  VALUES(@uid,@bid,@sd,@ed,@tot,@st,@pw) RETURNING id", conn);
            cmd.Parameters.AddWithValue("uid", b.UserId);
            cmd.Parameters.AddWithValue("bid", b.BoatId);
            cmd.Parameters.AddWithValue("sd", b.StartDate);
            cmd.Parameters.AddWithValue("ed", b.EndDate);
            cmd.Parameters.AddWithValue("tot", b.TotalAmount);
            cmd.Parameters.AddWithValue("st", b.Status.ToString());
            cmd.Parameters.AddWithValue("pw", (object?)b.PaidWith ?? DBNull.Value);

            var obj = await cmd.ExecuteScalarAsync();
            if (obj is null || obj is DBNull)
                throw new InvalidOperationException("Insert booking gagal: tidak ada id yang dikembalikan.");

            int id = obj is int i ? i
                    : obj is long l ? checked((int)l)   // jaga2 kalau driver balikin bigint
                    : Convert.ToInt32(obj);

            return id;
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            var list = new List<Booking>();
            using var conn = Db.GetOpenConnection();
            using var cmd = new NpgsqlCommand(
                "SELECT id, user_id, boat_id, start_date, end_date, total_amount, status, paid_with FROM bookings ORDER BY id DESC", conn);
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new Booking
                {
                    BookingId = rd.GetInt32(0),
                    UserId = rd.GetInt32(1),
                    BoatId = rd.GetInt32(2),
                    StartDate = rd.GetDateTime(3),
                    EndDate = rd.GetDateTime(4),
                    TotalAmount = rd.GetDecimal(5),
                    Status = Enum.Parse<BookingStatus>(rd.GetString(6)),
                    //PaidWith = rd.IsDBNull(7) ? null : rd.GetString(7)
                });
            }
            return list;
        }
    }
}
