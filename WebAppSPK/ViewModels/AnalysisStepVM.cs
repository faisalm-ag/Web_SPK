using System.Collections.Generic;
using System.Linq;

namespace WebAppSPK.ViewModels
{
    /// <summary>
    /// ViewModel yang diperkuat untuk mendukung narasi mendalam yang sinkron antar tahap.
    /// Memungkinkan halaman saat ini mengetahui hasil analisis dari tahap sebelumnya.
    /// </summary>
    public class AnalysisStepVM
    {
        // --- Header & Identitas ---
        public string Icon { get; set; } = "bi-bar-chart";
        public string Title { get; set; } = "Analysis Step";
        public string Description { get; set; } = "Deep Analytical Engine";
        public int StepNumber { get; set; } = 3;
        public string JobCode { get; set; } = string.Empty;

        // --- Navigasi Sub-Step (C1 - C4) ---
        public int CurrentSubStep { get; set; } // 1: Gaji, 2: CPI, 3: Peluang, 4: Lingkungan
        public string CriteriaCode { get; set; } = "C1";

        // --- Sinkronisasi Lokasi ---
        public string UserLocationChoice { get; set; } = string.Empty;

        // --- Data Visualisasi Utama (Bar/Radar Chart) ---
        public List<string> ChartLabels { get; set; } = new();
        public List<string> TranslatedLabels { get; set; } = new();
        public List<double> ChartValues { get; set; } = new();

        // --- Data Visualisasi Historis (Line Chart) ---
        public List<string> HistoryLabels { get; set; } = new();
        public List<double> HistoryValues { get; set; } = new();

        // --- SINKRONISASI NARASI (Contextual Storytelling) ---
        public string Narration { get; set; } = string.Empty; // Deskripsi dasar data saat ini
        public string DeepInsight { get; set; } = string.Empty; // Analisis mendalam
        public string WhyItMatters { get; set; } = string.Empty; // Relevansi bagi mahasiswa
        
        // PROPERTI BARU: Menampung pesan keterhubungan antar tahap
        // Contoh: "Gaji IT yang stabil (C1) akan kami bandingkan dengan biaya hidup (C2) berikut ini"
        public string PreviousStepSummary { get; set; } = string.Empty; 
        public string ComparisonAnalysis { get; set; } = string.Empty;

        // --- Metadata SAW ---
        public string CriteriaTypeInfo { get; set; } = "Benefit"; 

        // --- Statistik Big Data ---
        public int TotalDataProcessed { get; set; }
        public string DataProcessingTime { get; set; } = "0.0000s";
        public string NextStepAction { get; set; } = "Next";

        // --- Properti Reaktif untuk UI ---
        public bool IsCostCriteria => CriteriaTypeInfo.ToLower() == "cost";
        
        // Warna dinamis berdasarkan kriteria (Biru untuk Benefit, Kuning/Orange untuk Cost)
        public string ThemeColor => IsCostCriteria ? "#f59e0b" : "#22d3ee";
        public string ChartBgColor => IsCostCriteria ? "rgba(245, 158, 11, 0.2)" : "rgba(34, 211, 238, 0.2)";
        public string ChartBorderColor => ThemeColor;

        // Label Dinamis untuk Tooltip Chart
        public string UnitLabel => CriteriaCode switch
        {
            "C1" => "Yen (¥)",
            "C2" => "Cost Index",
            "C3" => "Companies",
            "C4" => "Population (K)",
            _ => "Value"
        };

        public bool HasHistoryData => HistoryValues != null && HistoryValues.Any();

        // --- HELPER UNTUK SINKRONISASI ---
        // Digunakan oleh View untuk menampilkan pesan transisi yang halus
        public bool IsStepComparisonAvailable => !string.IsNullOrEmpty(PreviousStepSummary);
    }
}