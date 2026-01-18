using System;

namespace SPKDomain.Models
{
    public class PredictionResult
    {
        public string PredictedLabel { get; set; } = string.Empty;
        public float ConfidenceScore { get; set; }

        // PERBAIKAN: Diubah dari '=>' menjadi '{ get; set; }' agar bisa diisi nilainya
        public double Probability { get; set; } 
        
        // PERBAIKAN: Menambahkan SatisfactionLevel yang dicari oleh Service
        public string SatisfactionLevel { get; set; } = string.Empty;

        // Alias untuk sinkronisasi tampilan
        public double SatisfactionPercentage => ReadinessPercentage;
        public string PredictionMessage => AnalysisSummary;

        public double ReadinessPercentage => Math.Round(ConfidenceScore * 100, 2);
        public string AnalysisSummary { get; set; } = string.Empty;

        public bool IsQualified => ConfidenceScore >= 0.6f;
        public string StatusColor => IsQualified ? "success" : (ConfidenceScore >= 0.4f ? "warning" : "danger");
    }
}