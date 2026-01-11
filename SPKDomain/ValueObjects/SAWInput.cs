namespace SPKDomain.ValueObjects
{
    /// <summary>
    /// ValueObject yang menampung data input untuk kalkulator SAW.
    /// Objek ini membawa nilai yang sudah siap dihitung (normalisasi) 
    /// sekaligus data asli (raw) untuk keperluan transparansi laporan.
    /// </summary>
    public class SAWInput
    {
        public string AreaCode { get; set; } = string.Empty;
        public string AreaName { get; set; } = string.Empty;
        public string JobCode { get; set; } = string.Empty; // Penting untuk konteks analisis gaji

        // --- Kriteria SAW (Ternormalisasi 0.0 - 1.0) ---
        // Digunakan langsung dalam perkalian dengan bobot (W)
        public double NormSalary { get; set; }
        public double NormCPI { get; set; }
        public double NormCompanyCount { get; set; }
        public double NormPopulation { get; set; }

        // --- Data Asli (Raw Values) ---
        // Digunakan untuk menampilkan data sebenarnya di halaman "Hasil Analisis"
        public double RawSalary { get; set; }
        public double RawCPI { get; set; }
        public double RawCompanyCount { get; set; }
        public double RawPopulation { get; set; }

        public SAWInput() { }

        public SAWInput(string areaCode, string areaName, string jobCode)
        {
            AreaCode = areaCode;
            AreaName = areaName;
            JobCode = jobCode;
        }
    }
}