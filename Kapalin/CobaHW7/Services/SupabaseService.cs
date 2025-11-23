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
                // Karena ini krusial, mungkin lempar ulang error atau tutup aplikasi
                throw;
            }
        }
        public static async Task<List<Boat>> GetBoatsAsync()
        {
            try
            {
                // Langsung gunakan 'Client' yang static
                var response = await Client.From<Boat>().Get();
                return response.Models ?? new List<Boat>(); // Kembalikan list jika Models null
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching boats: {ex.Message}");
                return new List<Boat>(); // Kembalikan list kosong jika gagal
            }
        }

        public static async Task<List<Booking>> GetBookingsAsync()
        {
            try
            {
                // Langsung gunakan 'Client' yang static
                var response = await Client.From<Booking>().Get();
                return response.Models ?? new List<Booking>(); // Kembalikan list jika Models null
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching bookings: {ex.Message}");
                return new List<Booking>(); // Kembalikan list kosong jika gagal
            }
        }

        public static async Task<Boat> AddBoatAsync(Boat newBoat)
        {
            try
            {
                // 'Insert' akan mengirim data dan mengembalikan data
                // yang baru dibuat (termasuk ID barunya)
                var response = await Client.From<Boat>().Insert(newBoat);

                // Kembalikan objek kapal yang baru, lengkap dengan ID dari database
                return response.Models.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding boat: {ex.Message}");
                return null;
            }
        }

        public static async Task<bool> DeleteBoatAsync(long boatId)
        {
            try
            {
                await Client.From<Boat>()
                            .Where(b => b.ID == boatId) // Asumsi properti ID Anda 'ID'
                            .Delete();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting boat: {ex.Message}");
                // Anda mungkin ingin melempar exception di sini agar ViewModel tahu
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
                // Baca semua data byte dari file
                var imageBytes = File.ReadAllBytes(localFilePath);

                // Dapatkan klien storage
                var storage = Client.Storage;

                // Dapatkan referensi ke bucket 'boat-images'
                var bucket = storage.From("boat-images"); // HARUS SAMA DENGAN NAMA BUCKET

                // Upload file
                await bucket.Upload(imageBytes, fileNameInStorage, new global::Supabase.Storage.FileOptions
                {
                    CacheControl = "3600", // Cache selama 1 jam
                    Upsert = true // Timpa file jika namanya sama
                });

                // Dapatkan URL publik
                var publicUrl = bucket.GetPublicUrl(fileNameInStorage);
                return publicUrl;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error uploading image: {ex.Message}");
                throw; // Lempar ulang error agar UI tahu
            }
        }
    }
}
