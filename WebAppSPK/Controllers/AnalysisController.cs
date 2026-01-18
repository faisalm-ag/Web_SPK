using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SPKCore.Services;
using WebAppSPK.ViewModels;
using SPKDomain.Models;
using SPKDomain.ValueObjects;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.Json;
using System.Diagnostics;

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
            var timer = Stopwatch.StartNew();

            var userNama = HttpContext.Session.GetString("UserNama");
            var jobCategory = HttpContext.Session.GetString("SelectedJob");
            var userLokasiRaw = HttpContext.Session.GetString("SelectedArea");

            if (string.IsNullOrEmpty(userNama)) 
                return RedirectToAction("Biodata", "Assessment");
            
            if (string.IsNullOrEmpty(id)) id = "Gaji";
            if (id == "Final") return RedirectToAction("GenerateFinalReport");

            var detailData = await _analysisService.GetStepAnalysisAsync(id, jobCategory ?? "", userLokasiRaw ?? "");

            if (detailData == null) 
                return NotFound("Data analisis tidak ditemukan.");

            timer.Stop();
            string actualLatency = $"{timer.Elapsed.TotalMilliseconds:F4}ms";

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
                Description = $"Visualisasi kriteria {id} berdasarkan data rill untuk membantu keputusan Anda.",
                StepNumber = 3,
                CurrentSubStep = id switch { "Gaji" => 1, "CPI" => 2, "Peluang" => 3, "Lingkungan" => 4, _ => 1 },
                CriteriaCode = id switch { "Gaji" => "C1", "CPI" => "C2", "Peluang" => "C3", "Lingkungan" => "C4", _ => "C1" },
                Icon = id switch { "Gaji" => "bi-cash-stack", "CPI" => "bi-cart-check", "Peluang" => "bi-building-gear", "Lingkungan" => "bi-people-fill", _ => "bi-graph-up" },
                
                UserLocationChoice = userLokasiRaw ?? "Not Selected", 
                UserChoiceValue = detailData.CurrentValue,
                CurrentValue = detailData.CurrentValue,
                
                ChartLabels = detailData.ChartLabels ?? new List<string>(),
                ChartValues = detailData.ChartValues ?? new List<double>(),
                HistoryLabels = detailData.HistoryLabels ?? new List<string>(),
                HistoryValues = detailData.HistoryValues ?? new List<double>(),
                
                Narration = detailData.Narration,
                DeepInsight = detailData.DeepInsight,
                WhyItMatters = detailData.WhyItMatters,
                
                CriteriaTypeInfo = id == "CPI" ? "Cost" : "Benefit",
                TotalDataProcessed = detailData.ChartLabels?.Count ?? 0,
                DataProcessingTime = actualLatency, 
                
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
            var nama = HttpContext.Session.GetString("UserNama");
            var bidang = HttpContext.Session.GetString("SelectedJob");
            var lokasiRaw = HttpContext.Session.GetString("SelectedArea");
            var answersJson = HttpContext.Session.GetString("UserAnswers");

            if (string.IsNullOrEmpty(nama)) 
                return RedirectToAction("Biodata", "Assessment");

            List<int> answers = !string.IsNullOrEmpty(answersJson) 
                ? JsonSerializer.Deserialize<List<int>>(answersJson) ?? new List<int>()
                : Enumerable.Repeat(3, 12).ToList();

            var weightSource = answers.Take(4).Select(x => (double)x).ToList();
            double totalWeightPoints = weightSource.Sum();
            if (totalWeightPoints <= 0) totalWeightPoints = 4;

            var weights = new List<Weight>
            {
                new Weight("Salary", weightSource[0] / totalWeightPoints),
                new Weight("CPI", weightSource[1] / totalWeightPoints),
                new Weight("Company", weightSource[2] / totalWeightPoints),
                new Weight("Population", weightSource[3] / totalWeightPoints)
            };

            var report = await _analysisService.GenerateFinalReportAsync(nama, bidang ?? "", answers, weights, lokasiRaw ?? "");
            
            if (report == null) return RedirectToAction("Index", "Home");

            // mapping ke ViewModel (FinalResultVM)
            var viewModel = new FinalResultVM
            {
                Nama = nama,
                BidangPekerjaan = report.TargetJob,
                LokasiPilihan = lokasiRaw ?? "Unknown",
                
                ScoreDisiplinEtika = report.ScoreDisiplinEtika,
                ScoreKetahananDiri = report.ScoreKetahananDiri,
                ScoreAdaptasiSosial = report.ScoreAdaptasiSosial,
                Prediction = report.Prediction,

                Rankings = report.LocationRankings,
                
                // --- PERBAIKAN: Mapping data lokasi pilihan dari report ke ViewModel ---
                UserChosenLocationResult = report.ChosenLocationData,

                EligibilityScore = report.EligibilityScore,
                RecommendationCategory = report.RecommendationCategory,
                
                SummaryMessage = report.Conclusion,
                ScoreExplanation = report.ScoreExplanation, 
                SectionIntroPersonal = report.SectionIntroPersonal,
                
                PersonalStrengths = report.PersonalStrengths,
                PersonalWeaknesses = report.PersonalWeaknesses,
                FinancialAdvice = report.FinancialAdvice,
                FamilyPermissionAdvice = report.FamilyAdvice,
                CareerInsight = report.CareerInsight,

                DecisionDisclaimer = report.DecisionDisclaimer,
                
                WeightSalary = Math.Round((weightSource[0] / totalWeightPoints) * 100, 1),
                WeightCPI = Math.Round((weightSource[1] / totalWeightPoints) * 100, 1),
                WeightCompany = Math.Round((weightSource[2] / totalWeightPoints) * 100, 1),
                WeightPopulation = Math.Round((weightSource[3] / totalWeightPoints) * 100, 1)
            };

            return View("FinalReport", viewModel);
        }
    }
}