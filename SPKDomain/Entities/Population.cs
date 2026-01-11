using System.Diagnostics.CodeAnalysis;

namespace SPKDomain.Entities
{
    /// <summary>
    /// Entitas yang merepresentasikan data dari FINAL_Populasi.csv.
    /// Digunakan untuk analisis lingkungan dan gaya hidup (Dataset 4).
    /// Kriteria ini membantu memprediksi kepuasan berdasarkan preferensi keramaian wilayah.
    /// </summary>
    public class Population
    {
        // Kode Wilayah (Contoh: 1000 untuk Hokkaido, 2000 untuk Aomori)
        public required string AreaCode { get; set; }

        // Nama Wilayah Kanji/Asli (Contoh: 北海道)
        public required string AreaName { get; set; }

        // Nama Wilayah Romaji (Contoh: Hokkaido) - Untuk tampilan UI
        public string? AreaNameDisplay { get; set; }

        // Tahun pendataan (Contoh: 1301, 1701 - format spesifik e-Stat)
        public int Year { get; set; }

        // Jumlah Penduduk dalam ribuan (C4: Population in K)
        public double PopulationK { get; set; }

        /// <summary>
        /// Mengklasifikasikan tipe wilayah berdasarkan jumlah penduduk.
        /// Membantu memberikan saran pada Laporan Akhir.
        /// </summary>
        public string AreaType => PopulationK > 5000 ? "Metropolitan/Ramai" : 
                                  PopulationK > 1000 ? "Urban/Sedang" : "Rural/Tenang";

        public Population() { }

        [SetsRequiredMembers]
        public Population(string areaCode, string areaName, int year, double populationK)
        {
            AreaCode = areaCode;
            AreaName = areaName;
            Year = year;
            PopulationK = populationK;
        }
    }
}