using System.Collections.Generic;
using System.Linq;

namespace WebAppSPK.ViewModels
{
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

        // --- Sinkronisasi Lokasi & Pilihan ---
        public string UserLocationChoice { get; set; } = string.Empty;
        
        // Nilai spesifik untuk prefektur yang dipilih user
        public double UserChoiceValue { get; set; }
        
        // Mapping dari AnalysisStepData.CurrentValue
        public double CurrentValue { get; set; }

        // --- Data Visualisasi Utama (Bar/Radar Chart) ---
        public List<string> ChartLabels { get; set; } = new();
        public List<double> ChartValues { get; set; } = new();

        // --- Data Visualisasi Historis (Line Chart) ---
        public List<string> HistoryLabels { get; set; } = new();
        public List<double> HistoryValues { get; set; } = new();

        // --- SINKRONISASI NARASI ---
        public string Narration { get; set; } = string.Empty; 
        public string DeepInsight { get; set; } = string.Empty; 
        public string WhyItMatters { get; set; } = string.Empty; 
        
        // Properti ini tetap dipertahankan agar Controller tidak error saat mapping
        public string PreviousStepSummary { get; set; } = string.Empty; 
        public string ComparisonAnalysis { get; set; } = string.Empty;

        // --- Metadata SAW ---
        public string CriteriaTypeInfo { get; set; } = "Benefit"; 

        // --- Statistik ---
        public int TotalDataProcessed { get; set; }
        public string DataProcessingTime { get; set; } = "0.0012s";
        public string NextStepAction { get; set; } = "Next";

        // --- LOGIKA REAKTIF (Warna & Tema) ---
        // Sesuai permintaan: Menghilangkan Oranye, beralih ke Cyan/Pink
        public bool IsCostCriteria => CriteriaTypeInfo?.ToLower().Contains("cost") ?? false;
        
        // Oranye (#f59e0b) diganti ke Cyan (#22d3ee) atau tetap jika ingin pembeda, 
        // tapi di View kita akan paksa menggunakan Cyan/Pink.
        public string ThemeColor => "#22d3ee"; 
        public string AccentPink => "#f472b6";

        public string UnitLabel => CriteriaCode switch
        {
            "C1" => "Yen (¥)",
            "C2" => "Cost Index (%)",
            "C3" => "Companies Count",
            "C4" => "People (x1000)",
            _ => "Value"
        };

        public bool HasHistoryData => HistoryValues != null && HistoryValues.Any();
        public bool IsStepComparisonAvailable => !string.IsNullOrEmpty(PreviousStepSummary);

        public string GetBarColor(string label)
        {
            return label.Equals(UserLocationChoice, System.StringComparison.OrdinalIgnoreCase) 
                ? AccentPink : ThemeColor;
        }
    }
}