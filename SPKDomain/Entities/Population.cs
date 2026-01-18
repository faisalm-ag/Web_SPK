using System.Diagnostics.CodeAnalysis;

namespace SPKDomain.Entities
{
    /// <summary>
    /// Entitas yang merepresentasikan data dari FINAL_Populasi.csv.
    /// Digunakan untuk analisis lingkungan dan sosial (Dataset 4).
    /// Dalam SPK, kriteria ini biasanya bersifat "Benefit" karena wilayah ramai 
    /// cenderung memiliki fasilitas umum dan komunitas yang lebih lengkap.
    /// </summary>
    public class Population
    {
        // area_code (Contoh: 1000, 2000)
        public required string AreaCode { get; set; }

        // 全国・都道府県 (Nama asli Jepang: 北海道, 青森県)
        public required string AreaNameRaw { get; set; }

        // standardized_area_en (Contoh: Hokkaido, Aomori)
        // Kunci utama untuk sinkronisasi dengan dataset lainnya
        public required string StandardizedAreaEn { get; set; }

        // year (Angka tahun murni hasil ekstraksi: 2021, 2022, 2023, 2024)
        public int Year { get; set; }

        // is_total_pop (1 jika Total Populasi 男女計 & 総人口, 0 jika detail gender)
        public int IsTotalPop { get; set; }

        // clean_value (Jumlah Penduduk dalam satuan ribuan / Thousand Persons)
        public double PopulationK { get; set; }

        /// <summary>
        /// Mengklasifikasikan tipe wilayah berdasarkan jumlah penduduk.
        /// Digunakan untuk narasi otomatis pada Final Report.
        /// </summary>
        public string AreaType => PopulationK > 5000 ? "Metropolitan/Ramai" : 
                                  PopulationK > 1500 ? "Urban/Sedang" : "Rural/Tenang";

        public Population() { }

        [SetsRequiredMembers]
        public Population(string areaCode, string areaNameRaw, string standardizedAreaEn, int year, int isTotalPop, double populationK)
        {
            AreaCode = areaCode;
            AreaNameRaw = areaNameRaw;
            StandardizedAreaEn = standardizedAreaEn;
            Year = year;
            IsTotalPop = isTotalPop;
            PopulationK = populationK;
        }
    }
}