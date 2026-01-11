using System;
using System.Collections.Generic;
using System.Linq;
using SPKDomain.Models;

namespace WebAppSPK.ViewModels
{
    /// <summary>
    /// ViewModel Utama untuk Laporan Akhir (Final Report).
    /// Berfungsi sebagai Dashboard Konsultasi yang menggabungkan ML, SAW, dan Narasi Strategis.
    /// </summary>
    public class FinalResultVM
    {
        // --- 1. Identitas & Biodata ---
        public string Nama { get; set; } = string.Empty;
        public string BidangPekerjaan { get; set; } = string.Empty;
        public string LokasiPilihan { get; set; } = string.Empty;

        // --- 2. Hasil Machine Learning (Prediksi Kesiapan Mental) ---
        public PredictionResult Prediction { get; set; } = new PredictionResult();
        public double MentalReadinessScore => (Prediction != null) ? Math.Round(Prediction.Probability * 100, 1) : 0;

        // --- 3. Hasil Perhitungan SAW (Ranking Lokasi dari Dataset e-Stat) ---
        public List<SAWResult> Rankings { get; set; } = new List<SAWResult>();
        public SAWResult? TopRecommendation => Rankings.OrderByDescending(r => r.TotalScore).FirstOrDefault();

        // --- 4. Skor Kelayakan & Vonis Akhir (The Decision) ---
        // Gabungan antara Skor Mental dan Skor Data Ekonomi
        public double EligibilityScore { get; set; } 
        public string RecommendationCategory { get; set; } = string.Empty; // Sangat Layak, Dipertimbangkan, dll.
        public string SummaryMessage { get; set; } = string.Empty;

        // --- 5. Analisis SWOT (Strategi Konsultasi) ---
        public List<string> Strengths { get; set; } = new List<string>();
        public List<string> Weaknesses { get; set; } = new List<string>();
        public List<string> Opportunities { get; set; } = new List<string>();
        public List<string> Threats { get; set; } = new List<string>();

        // --- 6. Pesan Humanis (Khusus STT Cipasung) ---
        public string FinancialAdvice { get; set; } = string.Empty;     // Contoh: Simulasi tabungan
        public string FamilyPermissionAdvice { get; set; } = string.Empty; // Bahan bicara ke orang tua
        public string CareerInsight { get; set; } = string.Empty;      // Peluang masa depan

        // --- 7. Data Transparansi Bobot ---
        public double WeightSalary { get; set; }
        public double WeightCPI { get; set; }
        public double WeightCompany { get; set; }
        public double WeightPopulation { get; set; }

        // --- HELPER UNTUK VISUALISASI ---
        
        // Warna Status untuk Gauge Chart dan UI Alerts
        public string StatusColor => EligibilityScore switch
        {
            >= 80 => "success", // Hijau
            >= 65 => "warning", // Kuning
            _ => "danger"       // Merah
        };

        // Data untuk Radar Chart (Kekuatan Lokasi No. 1)
        public List<double> TopLocationScores => TopRecommendation != null 
            ? new List<double> { 
                TopRecommendation.NormalizedSalary, 
                TopRecommendation.NormalizedCPI, 
                TopRecommendation.NormalizedCompany, 
                TopRecommendation.NormalizedPopulation 
              } 
            : new List<double>();
            
        // Menghitung potensi sisa uang (Gaji - CPI) dalam Yen
        public double EstimatedMonthlySaving => TopRecommendation != null 
            ? (TopRecommendation.RawSalary - (TopRecommendation.RawCPI * 1000)) // Asumsi skala CPI
            : 0;
    }
}