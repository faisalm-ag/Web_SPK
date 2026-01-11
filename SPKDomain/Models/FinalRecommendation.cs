using System.Collections.Generic;
using System.Linq;

namespace SPKDomain.Models
{
    /// <summary>
    /// Model Laporan Akhir yang menggabungkan hasil Machine Learning dan Kalkulasi SAW.
    /// Diperkaya dengan narasi strategis untuk konsultasi mahasiswa STT Cipasung.
    /// </summary>
    public class FinalRecommendation
    {
        public string StudentName { get; set; } = string.Empty;
        public string TargetJob { get; set; } = string.Empty;

        // Hasil Prediksi Kesiapan Mental (Input Kuesioner via ML)
        public PredictionResult Prediction { get; set; } = new();

        // Hasil Perankingan Lokasi (Input 4 Dataset via SAW)
        public List<SAWResult> LocationRankings { get; set; } = new();

        // --- Analisis Kelayakan Strategis ---
        
        // Skor gabungan akhir (Mental + Ekonomi Dataset) dalam skala 0-100
        public double EligibilityScore { get; set; }
        
        // Kategori: "Sangat Direkomendasikan", "Dipertimbangkan Kembali", dll.
        public string RecommendationCategory { get; set; } = string.Empty;
        
        // Kesimpulan narasi utama (Vonis Akhir)
        public string Conclusion { get; set; } = string.Empty;

        // --- Analisis SWOT & Konsultasi ---
        public List<string> Strengths { get; set; } = new();    // Alasan kuat berangkat
        public List<string> Weaknesses { get; set; } = new();   // Tantangan internal (mental/skill)
        public List<string> Opportunities { get; set; } = new(); // Keuntungan karir di prefektur tsb
        public List<string> Threats { get; set; } = new();      // Risiko eksternal (biaya/sosial)

        // --- Poin Pertimbangan Humanis ---
        public string FinancialAdvice { get; set; } = string.Empty;     // Saran biaya & tabungan
        public string FamilyPermissionAdvice { get; set; } = string.Empty; // Narasi untuk diskusi orang tua
        public string FutureCareerPath { get; set; } = string.Empty;    // Proyeksi setelah magang

        /// <summary>
        /// Mendapatkan lokasi rekomendasi peringkat pertama.
        /// </summary>
        public SAWResult? TopRecommendation => LocationRankings.OrderByDescending(x => x.TotalScore).FirstOrDefault();

        /// <summary>
        /// Mendapatkan status warna untuk indikator visual di UI (gauge/badge).
        /// </summary>
        public string SeverityColor => EligibilityScore switch
        {
            >= 80 => "success", // Green
            >= 65 => "warning", // Yellow
            _ => "danger"       // Red
        };
    }
}