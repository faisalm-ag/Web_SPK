using SPKDomain.Interfaces;
using SPKDomain.Entities;
using SPKDomain.Models;
using SPKDomain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;

namespace SPKCore.Services
{
    public class AnalysisStepData
    {
        public string StepName { get; set; } = "";
        public List<string> ChartLabels { get; set; } = new();
        public List<double> ChartValues { get; set; } = new();
        public List<string> HistoryLabels { get; set; } = new();
        public List<double> HistoryValues { get; set; } = new();
        public string Narration { get; set; } = "";
        public string DeepInsight { get; set; } = "";
        public string WhyItMatters { get; set; } = "";
        public string PreviousStepSummary { get; set; } = "";
        public string ComparisonAnalysis { get; set; } = "";
        public double CurrentValue { get; set; }
    }

    public class AnalysisService
    {
        private readonly IDataRepository _repository;
        private readonly ISAWCalculator _calculator;
        private readonly PredictionService _predictionService;

        private readonly Dictionary<string, string> _romajiMap = new()
        {
            // --- Prefektur ---
            {"北海道", "Hokkaido"}, {"青森", "Aomori"}, {"岩手", "Iwate"}, {"宮城", "Miyagi"},
            {"秋田", "Akita"}, {"山形", "Yamagata"}, {"福島", "Fukushima"}, {"茨城", "Ibaraki"},
            {"栃木", "Tochigi"}, {"群馬", "Gunma"}, {"埼玉", "Saitama"}, {"千葉", "Chiba"},
            {"東京", "Tokyo"}, {"神奈川", "Kanagawa"}, {"新潟", "Niigata"}, {"富山", "Toyama"},
            {"石川", "Ishikawa"}, {"福井", "Fukui"}, {"山梨", "Yamanashi"}, {"長野", "Nagano"},
            {"岐阜", "Gifu"}, {"静岡", "Shizuoka"}, {"愛知", "Aichi"}, {"三重", "Mie"},
            {"滋賀", "Shiga"}, {"京都", "Kyoto"}, {"大阪", "Osaka"}, {"兵庫", "Hyogo"},
            {"奈良", "Nara"}, {"和歌山", "Wakayama"}, {"鳥取", "Tottori"}, {"島根", "Shimane"},
            {"岡山", "Okayama"}, {"広島", "Hiroshima"}, {"山口", "Yamaguchi"}, {"徳島", "Tokushima"},
            {"香川", "Kagawa"}, {"愛媛", "Ehime"}, {"高知", "Kochi"}, {"福岡", "Fukuoka"},
            {"佐賀", "Saga"}, {"長崎", "Nagasaki"}, {"熊本", "Kumamoto"}, {"大分", "Oita"},
            {"宮崎", "Miyazaki"}, {"鹿児島", "Kagoshima"}, {"沖縄", "Okinawa"},

            // --- IT & Engineering ---
            {"システムコンサルタント・設計者", "IT Consultant/Architect"},
            {"ソフトウェア作成者", "Software Developer"},
            {"情報処理・通信技術者", "IT & Telecom Engineer"},
            {"電気・電子・電気通信技術者", "Electronics Engineer"},
            {"機械技術者", "Mechanical Engineer"},
            {"化学技術者", "Chemical Engineer"},
            {"土木技術者", "Civil Engineer"},
            {"輸送用機器技術者", "Automotive Engineer"},
            {"金属技術者", "Metallurgical Engineer"},
            {"測量技術者", "Surveying Engineer"},
            {"デザイナー", "Designer"},

            // --- Health & Care ---
            {"介護職員", "Caregiver (Kaigo)"},
            {"介護支援専門員", "Care Manager"},
            {"訪問介護従事者", "Home Care Worker"},
            {"医師", "Medical Doctor"},
            {"歯科医師", "Dentist"},
            {"薬剤師", "Pharmacist"},
            {"看護師", "Registered Nurse"},
            {"准看護師", "Associate Nurse"},
            {"保育士", "Nursery Teacher"},
            {"栄養士", "Nutritionist"},

            // --- Construction & Industry ---
            {"建築", "Construction"},
            {"土木従事者", "Civil Engineering"},
            {"大工", "Carpenter"},
            {"電気工事従事者", "Electrician"},
            {"配管従事者", "Plumber"},
            {"鉄工", "Ironworker"},
            {"金属プレス", "Metal Press"},
            {"溶接", "Welder"},

            // --- Service & Logistics ---
            {"飲食物調理", "Chef/Cook"},
            {"飲食物給仕", "Waiter/Waitress"},
            {"理容・美容師", "Barber/Beautician"},
            {"ビル・建物清掃", "Building Cleaner"},
            {"警備員", "Security Guard"},
            {"自動車運転", "Driver"},
            {"大型貨物", "Heavy Truck Driver"},
            {"教員", "Teacher/Educator"},
            {"不詳", "Other/Undisclosed"}
        };

        public AnalysisService(IDataRepository repository, ISAWCalculator calculator, PredictionService predictionService)
        {
            _repository = repository;
            _calculator = calculator;
            _predictionService = predictionService;
        }

        public string Translate(string key)
        {
            if (string.IsNullOrEmpty(key)) return "Other";
            var cleaned = Regex.Replace(key, @"[\d-]", "").Trim();
            foreach (var item in _romajiMap)
            {
                if (cleaned.Contains(item.Key)) return item.Value;
            }
            return cleaned.Replace("県", "").Replace("府", "").Replace("都", "").Replace("道", "")
                          .Replace("従事者", "").Replace("職業", "").Replace("職", "").Trim();
        }

        public async Task<AnalysisStepData> GetStepAnalysisAsync(string stepName, string jobCode, double? previousValue = null)
        {
            var stepData = new AnalysisStepData { StepName = stepName };
            string transJob = Translate(jobCode);

            switch (stepName)
            {
                case "Gaji":
                    var salaries = await _repository.GetSalariesAsync();
                    var jobSalaries = salaries.Where(s => s.JobName == jobCode || s.JobCode == jobCode)
                                             .OrderBy(s => s.Year).ToList();

                    if (jobSalaries.Any())
                    {
                        stepData.HistoryLabels = jobSalaries.Select(h => h.Year.ToString()).ToList();
                        stepData.HistoryValues = jobSalaries.Select(h => h.SalaryValue * 10000).ToList();
                        stepData.CurrentValue = jobSalaries.Last().SalaryValue * 10000;
                    }
                    stepData.Narration = $"Analisis Modal Finansial Sektor {transJob}.";
                    stepData.DeepInsight = $"Gaji rata-rata: ¥{stepData.CurrentValue:N0}.";
                    stepData.WhyItMatters = "Menghitung potensi daya beli dan remitansi ke Indonesia.";
                    break;

                case "CPI":
                    var cpis = await _repository.GetCPIAsync();
                    if (cpis.Any())
                    {
                        var areaGroups = cpis.GroupBy(c => Translate(c.AreaName))
                                            .Select(g => new { Area = g.Key, AvgIndex = g.Average(x => x.CPIIndex) })
                                            .OrderByDescending(x => x.AvgIndex).Take(12).ToList();

                        stepData.ChartLabels = areaGroups.Select(a => a.Area).ToList();
                        stepData.ChartValues = areaGroups.Select(a => a.AvgIndex).ToList();
                        stepData.CurrentValue = cpis.Average(c => c.CPIIndex);
                    }
                    break;

                case "Peluang":
                    var comps = await _repository.GetCompaniesAsync();
                    var prefComps = comps.Where(c => c.AreaCode.EndsWith("000")) 
                                         .GroupBy(c => Translate(c.AreaName))
                                         .Select(g => new { Area = g.Key, Total = (double)g.Max(x => x.EstimatedCount) })
                                         .OrderByDescending(c => c.Total).Take(12).ToList();

                    stepData.ChartLabels = prefComps.Select(c => c.Area).ToList();
                    stepData.ChartValues = prefComps.Select(c => c.Total).ToList();
                    break;

                case "Lingkungan":
                    var pops = await _repository.GetPopulationsAsync();
                    var topPops = pops.GroupBy(p => Translate(p.AreaName))
                                      .Select(g => new { Area = g.Key, Value = g.Max(p => p.PopulationK) })
                                      .OrderByDescending(p => p.Value).Take(12).ToList();

                    stepData.ChartLabels = topPops.Select(p => p.Area).ToList();
                    stepData.ChartValues = topPops.Select(p => p.Value).ToList();
                    break;
            }
            return stepData;
        }

        public async Task<FinalRecommendation> GenerateFinalReportAsync(string studentName, string jobCode, List<int> answers, List<Weight> weights, string selectedArea = "")
        {
            var prediction = _predictionService.PredictSatisfaction(answers);
            var rawData = await _repository.GetRawDataByJobAsync(jobCode);

            if (rawData == null || !rawData.Any()) rawData = await _repository.GetRawDataByJobAsync("1072");

            var normalized = _calculator.PrepareAndNormalize(rawData);
            var rankedRaw = _calculator.CalculateRanking(normalized, weights);

            var ranked = rankedRaw
                .GroupBy(r => Translate(r.AreaName))
                .Select(g => new SAWResult 
                { 
                    AreaName = g.Key, 
                    TotalScore = g.Max(x => x.TotalScore),
                    RawSalary = g.First().RawSalary,
                    RawCPI = g.First().RawCPI
                })
                .OrderByDescending(r => r.TotalScore)
                .ToList();

            // LOGIC: Ambil data wilayah yang dipilih user di biodata
            var userChoice = ranked.FirstOrDefault(r => r.AreaName.Equals(selectedArea, StringComparison.OrdinalIgnoreCase)) 
                             ?? ranked.First();

            var systemTop = ranked.First();
            
            var report = new FinalRecommendation
            {
                StudentName = studentName,
                TargetJob = Translate(jobCode),
                Prediction = prediction,
                LocationRankings = ranked.Take(5).ToList(),
                // Eligibility Score dihitung berdasarkan wilayah PILIHAN USER
                EligibilityScore = Math.Round((prediction.Probability * 100 * 0.4) + (userChoice.TotalScore * 100 * 0.6), 1)
            };

            report.RecommendationCategory = report.EligibilityScore >= 80 ? "Sangat Direkomendasikan" :
                                            report.EligibilityScore >= 65 ? "Direkomendasikan dengan Catatan" : "Pertimbangkan Kembali";

            // Conclusion yang lebih personal
            report.Conclusion = $"Berdasarkan analisis untuk wilayah {userChoice.AreaName}, ";
            if (userChoice.AreaName != systemTop.AreaName)
            {
                report.Conclusion += $"sistem menemukan bahwa {systemTop.AreaName} secara statistik lebih unggul untuk profesi {report.TargetJob}, namun {userChoice.AreaName} tetap dievaluasi sesuai pilihan Anda.";
            }
            else
            {
                report.Conclusion += $"{userChoice.AreaName} merupakan pilihan terbaik secara statistik untuk profesi {report.TargetJob}.";
            }
            
            // SWOT disesuaikan dengan userChoice
            report.Strengths = new List<string> { 
                $"Skor kesesuaian wilayah {userChoice.AreaName}: {userChoice.TotalScore:F4}",
                "Keseimbangan parameter ekonomi lokal terpantau stabil",
                "Potensi dukungan sosial masyarakat di lokasi pilihan"
            };

            report.Weaknesses = new List<string> {
                userChoice.RawCPI > 102 ? $"Biaya hidup di {userChoice.AreaName} di atas rata-rata" : "Persaingan di industri padat cukup ketat",
                "Perlu adaptasi bahasa Jepang teknis yang intensif"
            };

            report.Opportunities = new List<string> {
                "Peluang karir jangka panjang di perusahaan mitra lokal",
                $"Jaringan komunitas pekerja asing di wilayah {userChoice.AreaName}"
            };

            report.Threats = new List<string> {
                "Inflasi lokal yang mempengaruhi daya simpan",
                "Gegar budaya pada 3 bulan pertama"
            };

            // Financial Advice berdasarkan wilayah pilihan
            report.FinancialAdvice = (userChoice.RawSalary * 10000 > 300000) 
                ? $"Gaji di {userChoice.AreaName} tergolong tinggi. Anda berpotensi menabung lebih banyak."
                : $"Gaji di {userChoice.AreaName} standar. Disarankan melakukan penghematan biaya hidup.";

            report.FamilyPermissionAdvice = "Wilayah ini memiliki indeks keamanan dan kenyamanan yang baik untuk ditinggali.";
            report.FutureCareerPath = $"Sertifikasi JLPT akan sangat meningkatkan daya tawar Anda di {userChoice.AreaName}.";

            return report;
        }
    }
}