using CobaHW7.Class;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Collections.Generic;
//using classDiagram1;

namespace CobaHW7.ViewModels
{
    public class DashboardUserViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Boat> Boats { get; } = new();
        public ICollectionView BoatsView { get; }

        // Filters
        private string _searchText;
        public string SearchText { get => _searchText; set { _searchText = value; OnChanged(nameof(SearchText)); BoatsView.Refresh(); } }

        private string _priceMinText, _priceMaxText, _capacityMinText;
        public string PriceMinText { get => _priceMinText; set { _priceMinText = value; OnChanged(nameof(PriceMinText)); BoatsView.Refresh(); } }
        public string PriceMaxText { get => _priceMaxText; set { _priceMaxText = value; OnChanged(nameof(PriceMaxText)); BoatsView.Refresh(); } }
        public string CapacityMinText { get => _capacityMinText; set { _capacityMinText = value; OnChanged(nameof(CapacityMinText)); BoatsView.Refresh(); } }

        private bool _availableOnly;
        public bool AvailableOnly { get => _availableOnly; set { _availableOnly = value; OnChanged(nameof(AvailableOnly)); BoatsView.Refresh(); } }

        private DateTime? _startDate, _endDate;
        public DateTime? StartDate { get => _startDate; set { _startDate = value; OnChanged(nameof(StartDate)); BoatsView.Refresh(); } }
        public DateTime? EndDate { get => _endDate; set { _endDate = value; OnChanged(nameof(EndDate)); BoatsView.Refresh(); } }

        private string _sortBy = "Harga (Naik)";
        public string SortBy { get => _sortBy; set { _sortBy = value; OnChanged(nameof(SortBy)); ApplySort(); } }
        public string[] SortOptions => new[] { "Harga (Naik)", "Harga (Turun)", "Rating", "Kapasitas", "Tahun" };

        public DashboardUserViewModel()
        {
            Seed(); // sementara hardcoded
            BoatsView = CollectionViewSource.GetDefaultView(Boats);
            BoatsView.Filter = FilterBoat;
            ApplySort();
        }

        private bool FilterBoat(object obj)
        {
            if (obj is not Boat b) return false;

            // Search
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var t = SearchText.Trim().ToLowerInvariant();
                bool hit = (b.Name ?? "").ToLowerInvariant().Contains(t)
                        || (b.Model ?? "").ToLowerInvariant().Contains(t)
                        || (b.Location ?? "").ToLowerInvariant().Contains(t);
                if (!hit) return false;
            }

            // Harga
            if (decimal.TryParse(PriceMinText, out var pmin) && b.Price < pmin) return false;
            if (decimal.TryParse(PriceMaxText, out var pmax) && b.Price > pmax) return false;

            // Kapasitas
            if (int.TryParse(CapacityMinText, out var cmin) && b.Capacity < cmin) return false;

            // Ketersediaan
            if (AvailableOnly)
            {
                if (StartDate == null || EndDate == null || StartDate >= EndDate) return false;
                if (!IsAvailable(b, StartDate.Value, EndDate.Value)) return false;
            }

            return true;
        }

        private static bool IsAvailable(Boat b, DateTime start, DateTime end)
        {
            if (b.Booked == null) return true;
            return !b.Booked.Any(x => x.Overlaps(start, end));
        }

        public void ClearFilters()
        {
            SearchText = "";
            PriceMinText = PriceMaxText = CapacityMinText = "";
            AvailableOnly = false;
            StartDate = EndDate = null;
            SortBy = "Harga (Naik)";
            ApplySort();
            BoatsView.Refresh();
        }

        private void ApplySort()
        {
            BoatsView.SortDescriptions.Clear();
            switch (SortBy)
            {
                case "Harga (Naik)":
                    BoatsView.SortDescriptions.Add(new SortDescription(nameof(Boat.Price), ListSortDirection.Ascending)); break;
                case "Harga (Turun)":
                    BoatsView.SortDescriptions.Add(new SortDescription(nameof(Boat.Price), ListSortDirection.Descending)); break;
                case "Rating":
                    BoatsView.SortDescriptions.Add(new SortDescription(nameof(Boat.Rating), ListSortDirection.Descending)); break;
                case "Kapasitas":
                    BoatsView.SortDescriptions.Add(new SortDescription(nameof(Boat.Capacity), ListSortDirection.Descending)); break;
                case "Tahun":
                    BoatsView.SortDescriptions.Add(new SortDescription(nameof(Boat.Year), ListSortDirection.Descending)); break;
            }
        }

        private void Seed()
        {
            Boats.Clear();
            Boats.Add(new Boat
            {
                ID = 001,
                Name = "Samudra 1",
                Model = "Speedboat 22",
                Location = "Jakarta",
                Capacity = 8,
                Year = 2021,
                Rating = 4.7,
                Price = 3500000,
                ThumbnailPath = null,
                Facilities = new List<string> { "Kapten", "Pelampung", "Toilet" },
                Booked = new List<DateRange> { new DateRange(DateTime.Today.AddDays(2), DateTime.Today.AddDays(4)) }
            });
            Boats.Add(new Boat
            {
                ID = 002, 
                Name = "Pelangi Laut",
                Model = "Yacht 35",
                Location = "Bali",
                Capacity = 12,
                Year = 2019,
                Rating = 4.9,
                Price = 9500000,
                ThumbnailPath = null,
                Facilities = new List<string> { "Kapten", "BBQ", "Snorkeling" }
            });
            Boats.Add(new Boat
            {
                ID = 003,
                Name = "Nusantara",
                Model = "Fishing 28",
                Location = "Makassar",
                Capacity = 6,
                Year = 2018,
                Rating = 4.3,
                Price = 2500000,
                ThumbnailPath = null
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
