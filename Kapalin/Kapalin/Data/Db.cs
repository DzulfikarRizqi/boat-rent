using Npgsql;

namespace CobaHW7.Data
{
    public static class Db
    {
        private const string ConnString =
            "Host=localhost;Port=5432;Database=kapalin2;Username=postgres;Password=aspire4730z";

        public static NpgsqlConnection GetOpenConnection()
        {
            var conn = new NpgsqlConnection(ConnString);
            conn.Open();
            return conn;
        }
    }
}
