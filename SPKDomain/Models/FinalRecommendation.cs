using System;
using System.Collections.Generic;
using System.Linq;

namespace SPKDomain.Models
{
    public class FinalRecommendation
    {
        // --- 1. Identitas ---
        public string StudentName { get; set; } = string.Empty;
        public string TargetJob { get; set; } = string.Empty;
        public string ChosenLocation { get; set; } = string.Empty;

        // --- 2. Analisis Kesiapan Internal ---
        public double ScoreDisiplinEtika { get; set; }  
        public double ScoreKetahananDiri { get; set; }  
        public double ScoreAdaptasiSosial { get; set; } 
        public PredictionResult Prediction { get; set; } = new();

        // --- 3. Analisis Eksternal (Dataset) ---
        public List<SAWResult> LocationRankings { get; set; } = new();
        
        // --- PERBAIKAN: Menampung detail data lokasi yang dipilih user ---
        public SAWResult? ChosenLocationData { get; set; }

        // --- 4. Kesimpulan Strategis & Narasi Utama ---
        public double EligibilityScore { get; set; } 
        public string RecommendationCategory { get; set; } = string.Empty;
        public string ScoreExplanation { get; set; } = string.Empty;
        public string Conclusion { get; set; } = string.Empty;

        // --- 5. Narasi Konsultatif ---
        public string SectionIntroPersonal { get; set; } = string.Empty;
        public List<string> PersonalStrengths { get; set; } = new(); 
        public List<string> PersonalWeaknesses { get; set; } = new();
        public string FinancialAdvice { get; set; } = string.Empty;
        public string FamilyAdvice { get; set; } = string.Empty;
        public string CareerInsight { get; set; } = string.Empty;

        // --- 6. Pesan Kebijaksanaan ---
        public string DecisionDisclaimer { get; set; } = string.Empty;

        // --- 7. Helpers ---
        public SAWResult? TopRecommendation => LocationRankings.OrderByDescending(x => x.TotalScore).FirstOrDefault();

        public string SeverityColor => EligibilityScore switch
        {
            >= 80 => "success",
            >= 65 => "warning",
            _ => "danger"
        };
    }
}