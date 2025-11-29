using CobaHW7.Class;
using Supabase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Supabase.Storage;

namespace CobaHW7.Services
{
    public class SupabaseService
    {
        private static readonly string supabaseUrl = "https://xgldgydfazfjrujegwju.supabase.co";
        private static readonly string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InhnbGRneWRmYXpmanJ1amVnd2p1Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjI4NDE4ODgsImV4cCI6MjA3ODQxNzg4OH0.JV8X8VjRaiL-P011C0Tvq255ayWul_03l_TjSUEZoOE";

        public static global::Supabase.Client Client { get; }

        static SupabaseService()
        {
            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true
            };

            try
            {
                Client = new global::Supabase.Client(supabaseUrl, supabaseKey, options);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Kesalahan inisialisasi Supabase: {ex.Message}");
                throw;
            }
        }
        public static async Task<List<Boat>> GetBoatsAsync()
        {
            try
            {
                var response = await Client.From<Boat>().Get();
                return response.Models ?? new List<Boat>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching boats: {ex.Message}");
                return new List<Boat>();
            }
        }

        public static async Task<List<Booking>> GetBookingsAsync()
        {
            try
            {
                var response = await Client.From<Booking>().Get();
                return response.Models ?? new List<Booking>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching bookings: {ex.Message}");
                return new List<Booking>();
            }
        }

        public static async Task<(bool available, string message)> CheckBoatAvailabilityAsync(long boatId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var allBookings = await GetBookingsAsync();
                var boatBookings = allBookings.Where(b => b.BoatId == boatId && b.Status != "Cancelled").ToList();

                Debug.WriteLine($"[CheckAvailability] Boat {boatId}: Found {boatBookings.Count} bookings");

                int overlappingCount = 0;
                DateTime? firstOverlapStart = null;
                DateTime? firstOverlapEnd = null;

                foreach (var booking in boatBookings)
                {
                    if (startDate < booking.EndDate && endDate > booking.StartDate)
                    {
                        overlappingCount++;

                        if (overlappingCount == 1)
                        {
                            firstOverlapStart = booking.StartDate;
                            firstOverlapEnd = booking.EndDate;
                        }

                        Debug.WriteLine($"[CheckAvailability] Overlap found: {booking.StartDate} - {booking.EndDate}");
                    }
                }

                Debug.WriteLine($"[CheckAvailability] Overlapping bookings: {overlappingCount}");

                if (overlappingCount >= 2)
                {
                    string dateRange = $"{firstOverlapStart?.ToString("dd/MM/yyyy")} hingga {firstOverlapEnd?.ToString("dd/MM/yyyy")}";
                    return (false, $"Kapal tidak tersedia pada tanggal {dateRange}. Sudah ada 2 pesanan lain pada periode ini.");
                }

                return (true, "Kapal tersedia");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CheckAvailability] Error: {ex.Message}");
                throw;
            }
        }

        public static async Task<Boat> AddBoatAsync(Boat newBoat)
        {
            try
            {
                Debug.WriteLine($"[AddBoatAsync] Starting - Name: {newBoat.Name}, Price: {newBoat.PricePerDay}");

                var response = await Client.Rpc("create_boat", new Dictionary<string, object>
                {
                    { "p_name", newBoat.Name },
                    { "p_model", newBoat.Model },
                    { "p_location", newBoat.Location },
                    { "p_capacity", newBoat.Capacity },
                    { "p_year", newBoat.Year },
                    { "p_rating", newBoat.Rating },
                    { "p_price_per_day", newBoat.PricePerDay },
                    { "p_available", newBoat.Available ?? false },
                    { "p_thumbnail_path", newBoat.ThumbnailPath }
                });

                if (long.TryParse(response.Content, out long boatId))
                {
                    newBoat.ID = boatId;
                    Debug.WriteLine($"[AddBoatAsync] Success - Boat ID: {boatId}");
                    return newBoat;
                }
                else
                {
                    throw new Exception($"Failed to parse boat ID: {response.Content}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AddBoatAsync] Error: {ex.Message}");
                Debug.WriteLine($"[AddBoatAsync] StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public static async Task<Boat> UpdateBoatAsync(Boat updatedBoat)
        {
            try
            {
                Debug.WriteLine($"[UpdateBoatAsync] Starting - ID: {updatedBoat.ID}, Name: {updatedBoat.Name}");
                var response = await Client.From<Boat>().Update(updatedBoat);

                Debug.WriteLine($"[UpdateBoatAsync] Success - Boat ID: {updatedBoat.ID}");
                return updatedBoat;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdateBoatAsync] Error: {ex.Message}");
                Debug.WriteLine($"[UpdateBoatAsync] StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public static async Task<bool> DeleteBoatAsync(long? boatId)
        {
            try
            {
                if (boatId == null)
                    throw new ArgumentNullException(nameof(boatId));

                await Client.From<Boat>()
                            .Where(b => b.ID == boatId)
                            .Delete();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting boat: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Meng-upload file gambar ke Supabase Storage
        /// </summary>
        /// <param name="localFilePath">Path file di komputer user (C:\...)</param>
        /// <param name="fileNameInStorage">Nama file yang diinginkan di Supabase</param>
        /// <returns>URL publik dari gambar yang diupload</returns>
        public static async Task<string> UploadBoatImageAsync(string localFilePath, string fileNameInStorage)
        {
            try
            {
                var imageBytes = File.ReadAllBytes(localFilePath);
                var storage = Client.Storage;
                var bucket = storage.From("boat-images");

                await bucket.Upload(imageBytes, fileNameInStorage, new global::Supabase.Storage.FileOptions
                {
                    CacheControl = "3600",
                    Upsert = true
                });

                var publicUrl = bucket.GetPublicUrl(fileNameInStorage);
                return publicUrl;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error uploading image: {ex.Message}");
                throw;
            }
        }
    }
}
