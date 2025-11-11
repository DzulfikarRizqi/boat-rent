using System.Collections.Generic;
using System.Threading.Tasks;
using CobaHW7.Class;
using Npgsql;

namespace CobaHW7.Data
{
    public class BoatRepository
    {
        public async Task<List<Boat>> GetAllAsync()
        {
            var list = new List<Boat>();
            using var conn = Db.GetOpenConnection();
            using var cmd = new NpgsqlCommand(
                "SELECT id, name, price_per_day, year_built, image_path, available FROM boats ORDER BY id", conn);
            using var rd = await cmd.ExecuteReaderAsync();

            while (await rd.ReadAsync())
            {
                int id = rd.GetInt32(0);
                string model = rd.IsDBNull(1) ? "" : rd.GetString(1);
                decimal price = rd.IsDBNull(2) ? 0m : rd.GetDecimal(2);
                int year = rd.IsDBNull(3) ? 0 : rd.GetInt32(3);
                string image = rd.IsDBNull(4) ? "" : rd.GetString(4);
                bool available = rd.GetBoolean(5);
                list.Add(new Boat(id, model, price, year, image, available));
            }

            return list;
        }
    }
}
