using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Class
{
    public class Boat
    {
        // Properties
        public int ID { get; set; }
        public string Model { get; set; }
        public decimal Price { get; set; }
        public int Year { get; set; }
        public string Image { get; set; }
        public bool Available { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Constructor
        public Boat(int id, string model, decimal price, int year, string image, bool available)
        {
            ID = id;
            Model = model;
            Price = price;
            Year = year;
            Image = image;
            Available = available;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        // Create boat from dictionary (simulate Map)
        public static Boat Create(Dictionary<string, object> data)
        {
            return new Boat(
                (int)data["ID"],
                data["Model"].ToString() ?? string.Empty,
                Convert.ToDecimal(data["Price"]),
                (int)data["Year"],
                data["Image"].ToString() ?? string.Empty,
                Convert.ToBoolean(data["Available"])
            );
        }

        // Get by ID
        public static Boat? GetById(int id, List<Boat> boats)
        {
            return boats.Find(b => b.ID == id);
        }

        // List boats
        public static List<Boat> List(List<Boat> boats)
        {
            return boats;
        }

        // Update
        public void Update(Dictionary<string, object> updates)
        {
            if (updates.ContainsKey("Model")) Model = updates["Model"]?.ToString() ?? string.Empty;
            if (updates.ContainsKey("Price")) Price = Convert.ToDecimal(updates["Price"]);
            if (updates.ContainsKey("Year")) Year = (int)updates["Year"];
            if (updates.ContainsKey("Image")) Image = updates["Image"]?.ToString() ?? string.Empty;
            if (updates.ContainsKey("Available")) Available = Convert.ToBoolean(updates["Available"]);
            UpdatedAt = DateTime.Now;
        }

        // Delete
        public bool Delete()
        {
            Available = false;
            UpdatedAt = DateTime.Now;
            return true;
        }

        // Set availability
        public void SetAvailability(bool available)
        {
            Available = available;
            UpdatedAt = DateTime.Now;
        }

        // Check availability (Test dulu)
        public bool IsAvailable(DateTime startDate, DateTime endDate)
        {
            return Available; // nanti bisa ditambah logika
        }

        // Calculate price (Test dulu)
        public decimal CalculatePrice(DateTime startDate, DateTime endDate, decimal? discount = null)
        {
            int days = (endDate - startDate).Days;
            if (days <= 0) days = 1;
            decimal total = Price * days;

            if (discount != null)
                total -= (decimal)discount;

            return total;
        }

        public override string ToString()
        {
            return $"ID: {ID}, Model: {Model}, Price: {Price}, Year: {Year}, Available: {Available}";
        }
    }
}
