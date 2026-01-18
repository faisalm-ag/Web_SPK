namespace SPKDomain.ValueObjects
{
    /// <summary>
    /// ValueObject yang menampung data input untuk kalkulator SAW.
    /// Membawa Nilai Asli (Raw), Nilai Normalisasi (Norm), dan Bobot (Weight).
    /// </summary>
    public class SAWInput
    {
        // Identitas Wilayah & Pekerjaan
        public string StandardizedAreaEn { get; set; } = string.Empty;
        public string AreaNameRaw { get; set; } = string.Empty;
        public string JobCategoryEn { get; set; } = string.Empty;

        // --- 1. Nilai Asli (Raw Values) ---
        // Digunakan untuk Dashboard Narasi (Contoh: "Gaji di sini ¥250.000")
        public double RawSalary { get; set; }
        public double RawCPI { get; set; }
        public double RawCompanyCount { get; set; }
        public double RawPopulation { get; set; }

        // --- 2. Nilai Normalisasi (R matrix: 0.0 - 1.0) ---
        // Hasil dari pembagian dengan nilai Max atau Min
        public double NormSalary { get; set; }
        public double NormCPI { get; set; }
        public double NormCompanyCount { get; set; }
        public double NormPopulation { get; set; }

        // --- 3. Bobot dari Kuesioner (W vector) ---
        // Disimpan di sini agar AnalysisService bisa menghitung V = W * R
        public double WeightSalary { get; set; }
        public double WeightCPI { get; set; }
        public double WeightCompany { get; set; }
        public double WeightPopulation { get; set; }

        public SAWInput() { }

        public SAWInput(string standardizedAreaEn, string areaNameRaw, string jobCategoryEn)
        {
            StandardizedAreaEn = standardizedAreaEn;
            AreaNameRaw = areaNameRaw;
            JobCategoryEn = jobCategoryEn;
        }
    }
}