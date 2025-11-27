using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace CobaHW7.Class
{
    [Table("Boats")]
    public class Boat : BaseModel
    {
        [PrimaryKey("id", false)]
        [Column("id")]
        public long ID { get; set; }

        [Column("Name")]
        public string Name { get; set; } = "";

        [Column("Model")]
        public string Model { get; set; } = "";

        [Column("Location")]
        public string Location { get; set; } = "";

        [Column("Capacity")]
        public int Capacity { get; set; }

        [Column("Year")]
        public int Year { get; set; }

        [Column("Rating")]
        public double Rating { get; set; }

        [Column("PricePerDay")]
        public decimal PricePerDay { get; set; }

        [Column("Available")]
        public bool? Available { get; set; }

        [Column("ThumbnailPath")]
        public string ThumbnailPath { get; set; } = "";
    }
}
