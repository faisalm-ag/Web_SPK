using System;

namespace SPKDomain.Models
{
    /// <summary>
    /// Model untuk menampung hasil akhir perhitungan metode SAW.
    /// Diperkaya untuk mendukung sinkronisasi narasi antar kriteria (Gaji vs CPI).
    /// </summary>
    public class SAWResult
    {
        public int Rank { get; set; }
        public string AreaCode { get; set; } = string.Empty;
        
        // Penamaan wilayah
        public string AreaName { get; set; } = string.Empty;
        public string Prefektur => AreaName; 
        
        // Skor Akhir
        public double TotalScore { get; set; }
        public double FinalPercentage => Math.Round(TotalScore * 100, 2);
        public string SuccessRate => $"{FinalPercentage}%";
        
        // Status & Rekomendasi
        public string RecommendationNote { get; set; } = string.Empty;
        public string UrgencyStatus { get; set; } = string.Empty; // Green, Yellow, Red

        // --- Perhitungan Sinkronisasi (Kekuatan Ekonomi) ---
        // Rasio daya beli: (Gaji / Biaya Hidup). Semakin tinggi semakin baik.
        public double PurchasingPowerIndex { get; set; } 
        public string EconomicInsight { get; set; } = string.Empty; // Narasi perbandingan Gaji & CPI

        // --- Data Kontribusi Skor (Mapping untuk View) ---
        public double SalaryScoreContribution { get; set; }
        public double CPIScoreContribution { get; set; }
        public double OpportunityScoreContribution { get; set; }
        public double PopulationScoreContribution { get; set; }

        // Alias agar View tetap kompatibel
        public double ScoreSalary => SalaryScoreContribution;
        public double ScoreCPI => CPIScoreContribution;
        public double ScoreCompany => OpportunityScoreContribution;
        public double ScorePopulation => PopulationScoreContribution;

        // --- Data Asli / Raw (e-Stat) ---
        public double RawSalary { get; set; }
        public double RawCPI { get; set; }
        public double RawCompany { get; set; }
        public double RawPopulation { get; set; }

        // --- Data Ternormalisasi (0 - 1) ---
        public double NormalizedSalary { get; set; }
        public double NormalizedCPI { get; set; }
        public double NormalizedCompany { get; set; }
        public double NormalizedPopulation { get; set; }

        // Helper untuk menentukan warna status di Dashboard
        public string StatusColor => FinalPercentage switch
        {
            >= 80 => "success", // Hijau (Sangat Layak)
            >= 60 => "warning", // Kuning (Dipertimbangkan)
            _ => "danger"       // Merah (Pikirkan Kembali)
        };
    }
}