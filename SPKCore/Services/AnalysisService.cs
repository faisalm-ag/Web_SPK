using SPKDomain.Interfaces;
using SPKDomain.Entities;
using SPKDomain.Models;
using SPKDomain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

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
        public double CurrentValue { get; set; }
    }

    public class AnalysisService
    {
        private readonly IDataRepository _repository;
        private readonly ISAWCalculator _calculator;
        private readonly PredictionService _predictionService;

        public AnalysisService(IDataRepository repository, ISAWCalculator calculator, PredictionService predictionService)
        {
            _repository = repository;
            _calculator = calculator;
            _predictionService = predictionService;
        }

        public async Task<AnalysisStepData> GetStepAnalysisAsync(string stepName, string jobCategory, string selectedArea = "")
        {
            var stepData = new AnalysisStepData { StepName = stepName };

            switch (stepName)
            {
                case "Gaji":
                    var salaries = await _repository.GetSalariesAsync();
                    var jobSalaries = salaries
                        .Where(s => s.JobNameEn == jobCategory && s.IsSalaryData == 1)
                        .OrderBy(s => s.TimeCode).ToList();

                    if (jobSalaries.Any())
                    {
                        stepData.HistoryLabels = jobSalaries.Select(h => h.TimeCode.ToString().Substring(0, 4)).Distinct().ToList();
                        stepData.HistoryValues = jobSalaries.Select(h => h.SalaryValue).ToList();
                        stepData.CurrentValue = jobSalaries.Last().SalaryValue;
                    }
                    
                    stepData.Narration = $"Analisis Tolok Ukur Pendapatan Nasional";
                    stepData.DeepInsight = $"Gaji baseline nasional untuk posisi ini adalah ¥{stepData.CurrentValue:N0}.";
                    stepData.WhyItMatters = "Gaji ini menjadi standar pendapatan (Benefit) yang nantinya akan dibandingkan dengan biaya hidup di setiap prefektur.";
                    break;

                case "CPI":
                    var cpis = await _repository.GetCPIAsync();
                    var allCpi = cpis.Where(c => c.IsGeneralIndex == 1)
                                     .GroupBy(c => c.StandardizedAreaEn)
                                     .Select(g => g.First())
                                     .OrderByDescending(c => c.CPIIndex).ToList();

                    var userCpi = allCpi.FirstOrDefault(c => c.StandardizedAreaEn == selectedArea);
                    var top9Cpi = GetTop9PlusUser(allCpi, selectedArea, c => c.StandardizedAreaEn);

                    stepData.ChartLabels = top9Cpi.Select(c => c.StandardizedAreaEn).ToList();
                    stepData.ChartValues = top9Cpi.Select(c => c.CPIIndex).ToList();
                    stepData.CurrentValue = userCpi?.CPIIndex ?? 100.0;
                    
                    stepData.Narration = $"Analisis Indeks Biaya Hidup: {selectedArea}";
                    stepData.DeepInsight = $"Indeks biaya hidup di {selectedArea} berada pada angka {stepData.CurrentValue}.";
                    stepData.WhyItMatters = "CPI adalah kriteria 'Cost'. Semakin tinggi angkanya, semakin besar pengeluaran untuk kebutuhan pokok.";
                    break;

                case "Peluang":
                    var comps = await _repository.GetCompaniesAsync();
                    var allComps = comps.Where(c => c.IsTotalIndustry == 1 && c.AreaLevel == "Prefecture")
                                         .OrderByDescending(c => c.EstimatedCount).ToList();
                    
                    var userComp = allComps.FirstOrDefault(c => c.StandardizedAreaEn == selectedArea);
                    var top9Comps = GetTop9PlusUser(allComps, selectedArea, c => c.StandardizedAreaEn);

                    stepData.ChartLabels = top9Comps.Select(c => c.StandardizedAreaEn).ToList();
                    stepData.ChartValues = top9Comps.Select(c => c.EstimatedCount).ToList();
                    stepData.CurrentValue = userComp?.EstimatedCount ?? 0;

                    stepData.Narration = $"Analisis Kepadatan Industri: {selectedArea}";
                    stepData.DeepInsight = $"Terdapat sekitar {stepData.CurrentValue:N0} unit usaha di wilayah {selectedArea}.";
                    stepData.WhyItMatters = "Banyaknya jumlah perusahaan meningkatkan peluang penempatan dan stabilitas karir magang Anda.";
                    break;

                case "Lingkungan":
                    var pops = await _repository.GetPopulationsAsync();
                    var allPops = pops.Where(p => p.IsTotalPop == 1 && p.StandardizedAreaEn != "National")
                                      .OrderByDescending(p => p.PopulationK).ToList();
                    
                    var userPop = allPops.FirstOrDefault(p => p.StandardizedAreaEn == selectedArea);
                    var top9Pops = GetTop9PlusUser(allPops, selectedArea, p => p.StandardizedAreaEn);

                    stepData.ChartLabels = top9Pops.Select(p => p.StandardizedAreaEn).ToList();
                    stepData.ChartValues = top9Pops.Select(p => p.PopulationK).ToList();
                    stepData.CurrentValue = userPop?.PopulationK ?? 0;

                    stepData.Narration = $"Analisis Lingkungan & Populasi: {selectedArea}";
                    stepData.DeepInsight = $"Populasi di {selectedArea} tercatat {stepData.CurrentValue:N0} ribu jiwa.";
                    stepData.WhyItMatters = "Skala populasi berbanding lurus dengan kelengkapan fasilitas publik dan kemudahan transportasi.";
                    break;
            }
            return stepData;
        }

        private List<T> GetTop9PlusUser<T>(List<T> source, string selectedArea, Func<T, string> areaSelector)
        {
            var top10 = source.Take(10).ToList();
            var isUserInTop10 = top10.Any(x => areaSelector(x) == selectedArea);

            if (isUserInTop10 || string.IsNullOrEmpty(selectedArea)) return top10;

            var result = source.Take(9).ToList();
            var userItem = source.FirstOrDefault(x => areaSelector(x) == selectedArea);
            if (userItem != null) result.Add(userItem);
            
            return result;
        }

        public async Task<FinalRecommendation> GenerateFinalReportAsync(string studentName, string jobCategory, List<int> answers, List<Weight> weights, string selectedArea = "")
        {
            var prediction = _predictionService.PredictSatisfaction(answers);
            var rawData = await _repository.GetRawDataByJobAsync(jobCategory);
            var normalized = _calculator.PrepareAndNormalize(rawData, weights);
            var ranked = _calculator.CalculateRanking(normalized);

            // Pencarian data spesifik wilayah pilihan
            var userChoice = ranked.FirstOrDefault(r => r.AreaName.Equals(selectedArea, StringComparison.OrdinalIgnoreCase)) ?? ranked.First();

            // Pilar Mental
            double pilar1 = (answers[4] + answers[8] + answers[10]) / 3.0; // Disiplin
            double pilar2 = (answers[6] + answers[7] + answers[9]) / 3.0;  // Ketahanan
            double pilar3 = (answers[5] + answers[11]) / 2.0;             // Adaptasi

            var report = new FinalRecommendation
            {
                StudentName = studentName,
                TargetJob = jobCategory,
                ChosenLocation = selectedArea,
                Prediction = prediction,
                LocationRankings = ranked.Take(5).ToList(),
                
                // --- PERBAIKAN: Memasukkan data SAW lengkap ke dalam report ---
                ChosenLocationData = userChoice, 
                
                EligibilityScore = Math.Round((prediction.Probability * 100 * 0.4) + (userChoice.TotalScore * 100 * 0.6), 1),
                ScoreDisiplinEtika = Math.Round(pilar1, 1),
                ScoreKetahananDiri = Math.Round(pilar2, 1),
                ScoreAdaptasiSosial = Math.Round(pilar3, 1)
            };

            // --- NARASI STRATEGIS ---
            report.ScoreExplanation = $"Skor {report.EligibilityScore}% mencerminkan perpaduan antara profil psikologis Anda (40%) dan data riil ekonomi wilayah {selectedArea} (60%). Angka ini merupakan indikator awal keselarasan antara harapan pribadi dengan realitas lapangan.";
            
            report.RecommendationCategory = report.EligibilityScore >= 80 ? "Sangat Direkomendasikan" : 
                                           report.EligibilityScore >= 65 ? "Dipertimbangkan" : "Perlu Persiapan Matang";

            string statusEkonomi = userChoice.TotalScore > 0.7 ? "sangat menguntungkan" : "cukup menantang";
            report.Conclusion = $"Wilayah {selectedArea} menawarkan indeks kecocokan {userChoice.TotalScore:F2} untuk posisi {jobCategory}. Tantangan utama di sini adalah menjaga keseimbangan antara {statusEkonomi}nya biaya hidup lokal dengan proyeksi pendapatan bulanan Anda.";

            report.SectionIntroPersonal = "Evaluasi berikut mencakup kekuatan yang dapat Anda manfaatkan dan potensi hambatan yang perlu Anda antisipasi di Jepang.";

            // --- LOGIKA KELEBIHAN (STRENGTHS) ---
            if (pilar1 >= 3.5) report.PersonalStrengths.Add("Integritas Kerja: Anda memiliki fondasi disiplin yang kuat untuk standar kerja Jepang.");
            if (pilar2 >= 3.5) report.PersonalStrengths.Add("Resiliensi: Kemampuan Anda menghadapi tekanan akan menjadi aset besar saat fase adaptasi.");
            if (pilar3 >= 3.5) report.PersonalStrengths.Add("Komunikasi Sosial: Kemudahan Anda beradaptasi membantu mempercepat proses asimilasi budaya.");
            
            if (!report.PersonalStrengths.Any())
            {
                double maxPilar = Math.Max(pilar1, Math.Max(pilar2, pilar3));
                if (maxPilar == pilar1) report.PersonalStrengths.Add("Potensi Kedisiplinan: Anda menunjukkan niat baik untuk mengikuti aturan, modal awal yang penting.");
                else if (maxPilar == pilar2) report.PersonalStrengths.Add("Potensi Daya Tahan: Ada kemauan dalam diri Anda untuk bertahan meski dalam kondisi sulit.");
                else report.PersonalStrengths.Add("Potensi Interaksi: Sisi ramah Anda dapat membantu mencairkan suasana di lingkungan baru.");
            }

            // --- LOGIKA KEKURANGAN (WEAKNESSES) ---
            if (pilar1 < 3.5) report.PersonalWeaknesses.Add("Fleksibilitas Aturan: Anda perlu membiasakan diri dengan ketegasan SOP di Jepang yang minim toleransi.");
            if (pilar2 < 3.5) report.PersonalWeaknesses.Add("Manajemen Stres: Waspadai potensi kejenuhan; siapkan hobi atau kegiatan positif di waktu luang.");
            if (pilar3 < 3.5) report.PersonalWeaknesses.Add("Keterbukaan Diri: Berusahalah untuk lebih proaktif dalam berkomunikasi guna menghindari rasa terisolasi.");

            if (report.PersonalWeaknesses.Count <= 1)
            {
                report.PersonalWeaknesses.Add("Cultural Fatigue: Meskipun mental kuat, tetap waspadai kelelahan budaya yang biasanya muncul setelah 6 bulan.");
                report.PersonalWeaknesses.Add("Homesickness: Kerinduan pada keluarga adalah hal manusiawi yang tetap harus Anda kelola dengan baik.");
            }

            // --- NASIHAT & DISCLAIMER ---
            report.FinancialAdvice = userChoice.RawSalary > 210000 
                ? $"Gaji di {selectedArea} cukup kompetitif. Fokuslah pada investasi diri."
                : $"Gaji standar di {selectedArea} mewajibkan Anda untuk sangat teliti dalam mengatur pengeluaran harian.";

            report.FamilyAdvice = "Bicarakan rencana ini dengan keluarga, terutama mengenai target jangka panjang Anda setelah selesai magang.";

            report.DecisionDisclaimer = "Hasil ini adalah simulasi sistem berdasarkan data saat ini. Keputusan akhir tetap sepenuhnya berada di tangan Anda. Gunakan laporan ini sebagai salah satu sudut pandang pertimbangan, namun tetaplah mendengarkan intuisi dan hasil diskusi bersama keluarga atau mentor Anda.";

            return report;
        }
    }
}