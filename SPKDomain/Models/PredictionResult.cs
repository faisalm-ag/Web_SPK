using System;

namespace SPKDomain.Models
{
    public class PredictionResult
    {
        public string PredictedLabel { get; set; } = string.Empty;
        public float ConfidenceScore { get; set; }

        // --- ALIAS UNTUK VIEWMODEL & SINKRONISASI ---
        public double Probability => (double)ConfidenceScore;
        
        // Menambah alias agar sinkron dengan FinalReport.cshtml
        public double SatisfactionPercentage => ReadinessPercentage;
        public string PredictionMessage => AnalysisSummary;

        public double ReadinessPercentage => Math.Round(ConfidenceScore * 100, 2);
        public string AnalysisSummary { get; set; } = string.Empty;

        public bool IsQualified => ConfidenceScore >= 0.6f;
        public string StatusColor => IsQualified ? "success" : (ConfidenceScore >= 0.4f ? "warning" : "danger");
    }
}