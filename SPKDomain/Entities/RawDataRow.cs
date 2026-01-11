namespace SPKDomain.Entities
{
    /// <summary>
    /// Representasi data mentah gabungan untuk keperluan perhitungan skor per wilayah/kriteria.
    /// Objek ini menampung hasil pemetaan dari 4 dataset (Gaji, CPI, Perusahaan, Populasi).
    /// </summary>
    public class RawDataRow
    {
        // Identitas Wilayah
        public string AreaCode { get; set; } = string.Empty;
        public string AreaName { get; set; } = string.Empty;

        // Data Ekonomi (Dataset Gaji & CPI)
        public string JobCode { get; set; } = string.Empty; // Ditambahkan untuk mapping spesifik ke dataset gaji
        public string BidangPekerjaan { get; set; } = string.Empty;
        public double AverageSalary { get; set; }
        public double ConsumerPriceIndex { get; set; }

        // Data Peluang & Lingkungan (Dataset Perusahaan & Populasi)
        public double CompanyCount { get; set; }
        public double Population { get; set; }

        /// <summary>
        /// Menghitung Rasio Pendapatan Riil (Gaji / Biaya Hidup).
        /// Digunakan sebagai indikator kepuasan finansial mahasiswa.
        /// </summary>
        public double RealIncomeRatio => ConsumerPriceIndex > 0 ? AverageSalary / ConsumerPriceIndex : 0;
    }
}