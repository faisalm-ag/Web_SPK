namespace SPKDomain.ValueObjects
{
    /// <summary>
    /// Menyimpan nilai bobot kepentingan untuk kriteria tertentu dalam metode SAW.
    /// Memastikan bahwa total bobot nantinya dapat dikalikan dengan nilai ternormalisasi.
    /// </summary>
    public class Weight
    {
        // Nama Kriteria (Contoh: "Salary", "CPI", "Company", "Population")
        // Mengubah 'init' menjadi 'set' agar lebih fleksibel bagi Controller
        public string Criteria { get; set; } = string.Empty;

        // Nilai Bobot (0.0 - 1.0)
        public double Value { get; set; }

        // Constructor tanpa parameter (Dibutuhkan untuk beberapa skenario serialisasi)
        public Weight() { }

        // Constructor utama yang dipanggil oleh AnalysisController
        public Weight(string criteria, double value)
        {
            Criteria = criteria;
            // Validasi sederhana agar bobot tidak negatif atau melebihi 1
            Value = value < 0 ? 0 : (value > 1 ? 1 : value);
        }

        /// <summary>
        /// Menampilkan nilai bobot dalam bentuk persentase (Contoh: 0.3 -> 30%).
        /// </summary>
        public string AsPercentageText => $"{(Value * 100):0}%";
    }
}