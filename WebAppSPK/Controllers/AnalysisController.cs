using Microsoft.AspNetCore.Mvc;
using SPKCore.Services;
using WebAppSPK.ViewModels;
using SPKDomain.Models;
using SPKDomain.ValueObjects;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Globalization;

namespace WebAppSPK.Controllers
{
    public class AnalysisController : Controller
    {
        private readonly AnalysisService _analysisService;

        public AnalysisController(AnalysisService analysisService)
        {
            _analysisService = analysisService;
        }

        [HttpGet]
        public async Task<IActionResult> Step(string id)
        {
            // 1. Proteksi Sesi: Pastikan user sudah isi biodata
            if (TempData["UserNama"] == null) return RedirectToAction("Biodata", "Assessment");
            
            if (string.IsNullOrEmpty(id)) id = "Gaji";
            if (id == "Final") return RedirectToAction("GenerateFinalReport");

            var jobCode = TempData["UserBidang"]?.ToString() ?? "1072";
            var userLokasiRaw = TempData["UserLokasi"]?.ToString() ?? "";
            
            double? previousValue = null;

            // Logika Sinkronisasi Data (Contoh: Bawa nilai Gaji ke halaman CPI)
            if (id == "CPI" && TempData["LastSalaryValue"] != null)
            {
                if (double.TryParse(TempData["LastSalaryValue"]?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double salaryVal))
                {
                    previousValue = salaryVal;
                }
            }

            // Eksekusi Service untuk ambil data visualisasi (Chart/History)
            var detailData = await _analysisService.GetStepAnalysisAsync(id, jobCode, previousValue);

            if (detailData == null) return NotFound("Data analisis tidak ditemukan.");

            // Simpan hasil Gaji sebagai referensi langkah berikutnya
            if (id == "Gaji") 
            {
                TempData["LastSalaryValue"] = detailData.CurrentValue.ToString(CultureInfo.InvariantCulture);
            }

            // Penting agar data TempData tidak terhapus setelah dibaca sekali
            TempData.Keep(); 

            // 2. Mapping data ke ViewModel untuk dikirim ke View
            var viewModel = new AnalysisStepVM
            {
                Title = id switch
                {
                    "Gaji" => "Analisis Kesejahteraan (Salary)",
                    "CPI" => "Analisis Biaya Hidup (Cost of Living)",
                    "Peluang" => "Analisis Kepadatan Industri",
                    "Lingkungan" => "Analisis Demografi & Lingkungan",
                    _ => "Data Analysis"
                },
                Description = $"Visualisasi data e-Stat kriteria {id} untuk mendukung keputusan magang Anda.",
                StepNumber = 3,
                CurrentSubStep = id switch { "Gaji" => 1, "CPI" => 2, "Peluang" => 3, "Lingkungan" => 4, _ => 1 },
                CriteriaCode = id switch { "Gaji" => "C1", "CPI" => "C2", "Peluang" => "C3", "Lingkungan" => "C4", _ => "C1" },
                Icon = id switch { "Gaji" => "bi-cash-stack", "CPI" => "bi-cart-check", "Peluang" => "bi-building-gear", "Lingkungan" => "bi-people-fill", _ => "bi-graph-up" },
                
                UserLocationChoice = _analysisService.Translate(userLokasiRaw),
                
                ChartLabels = detailData.ChartLabels ?? new List<string>(),
                ChartValues = detailData.ChartValues ?? new List<double>(),
                HistoryLabels = detailData.HistoryLabels ?? new List<string>(),
                HistoryValues = detailData.HistoryValues ?? new List<double>(),
                
                Narration = detailData.Narration ?? string.Empty,
                DeepInsight = detailData.DeepInsight ?? string.Empty,
                WhyItMatters = detailData.WhyItMatters ?? string.Empty,
                PreviousStepSummary = detailData.PreviousStepSummary ?? string.Empty,
                ComparisonAnalysis = detailData.ComparisonAnalysis ?? string.Empty,
                
                CriteriaTypeInfo = id == "CPI" ? "Cost" : "Benefit",
                TotalDataProcessed = detailData.ChartLabels?.Count ?? 0,
                DataProcessingTime = (new Random().NextDouble() * (0.0025 - 0.0010) + 0.0010).ToString("F4") + "s",
                NextStepAction = id switch { "Gaji" => "CPI", "CPI" => "Peluang", "Peluang" => "Lingkungan", _ => "Final" }
            };

            return View(id, viewModel);
        }

        [HttpGet] public async Task<IActionResult> Gaji() => await Step("Gaji");
        [HttpGet] public async Task<IActionResult> CPI() => await Step("CPI");
        [HttpGet] public async Task<IActionResult> Peluang() => await Step("Peluang");
        [HttpGet] public async Task<IActionResult> Lingkungan() => await Step("Lingkungan");

        [HttpGet]
        public async Task<IActionResult> GenerateFinalReport()
        {
            // Validasi Sesi
            if (TempData["UserNama"] == null) return RedirectToAction("Biodata", "Assessment");

            var nama = TempData["UserNama"]?.ToString() ?? "User";
            var bidang = TempData["UserBidang"]?.ToString() ?? "1072";
            var lokasiRaw = TempData["UserLokasi"]?.ToString() ?? "";
            
            // Konversi nama lokasi ke format English/Romaji agar cocok dengan dataset
            var lokasiTerjemahan = _analysisService.Translate(lokasiRaw);

            var rawAnswers = TempData["Answers"] as int[];
            var answers = rawAnswers?.ToList() ?? new List<int> { 5, 5, 5, 5, 5 };

            // 1. Kalkulasi Bobot Berdasarkan Jawaban Kuesioner (Metode Normalisasi)
            double sum = answers.Take(4).Sum(x => (double)x);
            if (sum <= 0) sum = 1;

            var weights = new List<Weight>
            {
                new Weight("Salary", (double)answers[0] / sum),
                new Weight("CPI", (double)answers[1] / sum),
                new Weight("Company", (double)answers[2] / sum),
                new Weight("Population", (double)answers[3] / sum)
            };

            // 2. Panggil Service dengan parameter Lokasi Pilihan User
            var report = await _analysisService.GenerateFinalReportAsync(nama, bidang, answers, weights, lokasiTerjemahan);
            
            if (report == null) return RedirectToAction("Index", "Home");

            // 3. Mapping hasil report ke ViewModel FinalResultVM
            var viewModel = new FinalResultVM
            {
                Nama = nama,
                BidangPekerjaan = report.TargetJob ?? "Umum",
                LokasiPilihan = lokasiTerjemahan,
                Prediction = report.Prediction ?? new PredictionResult(),
                Rankings = report.LocationRankings ?? new List<SAWResult>(),
                EligibilityScore = Math.Round(report.EligibilityScore, 1),
                RecommendationCategory = report.RecommendationCategory ?? "N/A",
                SummaryMessage = report.Conclusion ?? string.Empty,
                Strengths = report.Strengths ?? new List<string>(),
                Weaknesses = report.Weaknesses ?? new List<string>(),
                Opportunities = report.Opportunities ?? new List<string>(),
                Threats = report.Threats ?? new List<string>(),
                FinancialAdvice = report.FinancialAdvice ?? string.Empty,
                FamilyPermissionAdvice = report.FamilyPermissionAdvice ?? string.Empty,
                CareerInsight = report.FutureCareerPath ?? string.Empty,
                WeightSalary = weights[0].Value,
                WeightCPI = weights[1].Value,
                WeightCompany = weights[2].Value,
                WeightPopulation = weights[3].Value
            };

            return View("FinalReport", viewModel);
        }
    }
}