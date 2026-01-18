using System;
using System.Collections.Generic;
using System.Linq;
using SPKDomain.Models;

namespace WebAppSPK.ViewModels
{
    /// <summary>
    /// ViewModel Utama untuk Laporan Akhir (Consultancy Mode).
    /// </summary>
    public class FinalResultVM
    {
        // --- 1. Identitas & Tracking ---
        public string ReportId { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        public string Nama { get; set; } = string.Empty;
        public string BidangPekerjaan { get; set; } = string.Empty;
        public string LokasiPilihan { get; set; } = string.Empty;

        // --- 2. Pilar Kesiapan Mental (Radar Chart) ---
        public double ScoreDisiplinEtika { get; set; }  
        public double ScoreKetahananDiri { get; set; }  
        public double ScoreAdaptasiSosial { get; set; } 
        public double MentalReadinessScore => (Prediction != null) ? Math.Round(Prediction.Probability * 100, 1) : 50.0;
        public PredictionResult Prediction { get; set; } = new();

        // --- 3. Hasil Ekonomi (SAW - Dataset e-Stat) ---
        public List<SAWResult> Rankings { get; set; } = new();
        public SAWResult? TopRecommendation => Rankings.OrderByDescending(r => r.TotalScore).FirstOrDefault();
        
        // --- PERBAIKAN: Mengambil data langsung dari hasil Choice Service agar tidak null meski di luar Top 5 ---
        public SAWResult? UserChosenLocationResult { get; set; }

        // --- 4. Skor Akhir & Keputusan ---
        public double EligibilityScore { get; set; } 
        public string RecommendationCategory { get; set; } = "Dipertimbangkan"; 
        
        public string SummaryMessage { get; set; } = string.Empty; 
        public string ScoreExplanation { get; set; } = string.Empty; 

        // --- 5. Nasihat Konsultatif ---
        public string SectionIntroPersonal { get; set; } = string.Empty; 
        public List<string> PersonalStrengths { get; set; } = new(); 
        public List<string> PersonalWeaknesses { get; set; } = new();
        public string FinancialAdvice { get; set; } = string.Empty;     
        public string FamilyPermissionAdvice { get; set; } = string.Empty; 
        public string CareerInsight { get; set; } = string.Empty;      

        // --- 6. Pesan Kebijaksanaan ---
        public string DecisionDisclaimer { get; set; } = string.Empty;

        // --- 7. Transparansi Kriteria ---
        public double WeightSalary { get; set; }
        public double WeightCPI { get; set; }
        public double WeightCompany { get; set; }
        public double WeightPopulation { get; set; }

        // --- 8. Helper Skor Normalisasi (Perbaikan: Menggunakan pengali 100 agar tampil sebagai persentase) ---
        public double ScoreC1 => Math.Round((UserChosenLocationResult?.NormalizedSalary ?? 0) * 100, 1);
        public double ScoreC2 => Math.Round((UserChosenLocationResult?.NormalizedCPI ?? 0) * 100, 1);
        public double ScoreC3 => Math.Round((UserChosenLocationResult?.NormalizedCompany ?? 0) * 100, 1);
        public double ScoreC4 => Math.Round((UserChosenLocationResult?.NormalizedPopulation ?? 0) * 100, 1);

        // --- UI Logic Helpers ---
        public string StatusColor => EligibilityScore switch
        {
            >= 80 => "success",
            >= 65 => "warning",
            _ => "danger"
        };

        public bool IsUserChoiceTheBest => TopRecommendation != null && 
                                          UserChosenLocationResult != null &&
                                          TopRecommendation.AreaName.Equals(UserChosenLocationResult.AreaName, StringComparison.OrdinalIgnoreCase);
    }
}