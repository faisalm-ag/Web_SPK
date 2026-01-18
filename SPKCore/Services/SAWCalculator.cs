using System;
using System.Collections.Generic;
using System.Linq;
using SPKDomain.Interfaces;
using SPKDomain.Models;
using SPKDomain.Entities;
using SPKDomain.ValueObjects;

namespace SPKCore.Services
{
    public class SAWCalculator : ISAWCalculator
    {
        public List<SAWInput> PrepareAndNormalize(List<RawDataRow> rawData, List<Weight> weights)
        {
            if (rawData == null || !rawData.Any()) 
                return new List<SAWInput>();

            // 1. Mencari Nilai Ekstrem (Max/Min) untuk proses Normalisasi
            // Gaji, Perusahaan, Populasi = Benefit (Maksimal)
            // CPI = Cost (Minimal)
            
            // Catatan: Karena Gaji sekarang Nasional, maxSalary kemungkinan besar akan sama untuk semua baris.
            double maxSalary = Math.Max(rawData.Max(x => x.AverageSalary), 0.0001);
            double minCPI = Math.Max(rawData.Min(x => x.ConsumerPriceIndex), 0.0001);
            double maxCompanies = Math.Max(rawData.Max(x => x.CompanyCount), 0.0001);
            double maxPopulation = Math.Max(rawData.Max(x => x.Population), 0.0001);

            // Ambil nilai bobot dari parameter
            double wSalary = weights.FirstOrDefault(w => w.Criteria == "Salary")?.Value ?? 0.25;
            double wCPI = weights.FirstOrDefault(w => w.Criteria == "CPI")?.Value ?? 0.25;
            double wCompany = weights.FirstOrDefault(w => w.Criteria == "Company")?.Value ?? 0.25;
            double wPop = weights.FirstOrDefault(w => w.Criteria == "Population")?.Value ?? 0.25;

            // 2. Transformasi ke SAWInput (Normalisasi Matrix R)
            return rawData.Select(item => new SAWInput
            {
                StandardizedAreaEn = item.AreaCode, 
                AreaNameRaw = item.AreaName,
                JobCategoryEn = item.JobCode, 
                
                // Normalisasi C1: Gaji (Benefit) -> x / max
                NormSalary = item.AverageSalary / maxSalary,
                
                // Normalisasi C2: CPI (Cost) -> min / x
                // Wilayah dengan CPI rendah akan mendapat nilai normalisasi mendekati 1 (Bagus)
                NormCPI = item.ConsumerPriceIndex > 0 ? minCPI / item.ConsumerPriceIndex : 0,
                
                // Normalisasi C3: Perusahaan (Benefit) -> x / max
                NormCompanyCount = item.CompanyCount / maxCompanies,
                
                // Normalisasi C4: Populasi (Benefit) -> x / max
                NormPopulation = item.Population / maxPopulation,

                RawSalary = item.AverageSalary,
                RawCPI = item.ConsumerPriceIndex,
                RawCompanyCount = item.CompanyCount,
                RawPopulation = item.Population,

                WeightSalary = wSalary,
                WeightCPI = wCPI,
                WeightCompany = wCompany,
                WeightPopulation = wPop
            }).ToList();
        }

        public List<SAWResult> CalculateRanking(List<SAWInput> normalizedInputs)
        {
            if (normalizedInputs == null || !normalizedInputs.Any())
                return new List<SAWResult>();

            var results = normalizedInputs.Select(input =>
            {
                // Rumus SAW: V_i = Σ (w_j * r_ij)
                double sContrib = input.NormSalary * input.WeightSalary;
                double cContrib = input.NormCPI * input.WeightCPI;
                double oContrib = input.NormCompanyCount * input.WeightCompany;
                double pContrib = input.NormPopulation * input.WeightPopulation;

                double totalScore = sContrib + cContrib + oContrib + pContrib;

                // Hitung Purchasing Power Index (Analisis tambahan)
                // Karena Gaji Nasional, PPI akan murni dipengaruhi oleh perbedaan CPI lokal.
                double ppi = input.RawCPI > 0 ? (input.RawSalary / input.RawCPI) * 100 : 0;

                return new SAWResult
                {
                    AreaCode = input.StandardizedAreaEn,
                    AreaName = input.AreaNameRaw,
                    TotalScore = Math.Round(totalScore, 4),
                    
                    SalaryScoreContribution = sContrib,
                    CPIScoreContribution = cContrib,
                    OpportunityScoreContribution = oContrib,
                    PopulationScoreContribution = pContrib,

                    RawSalary = input.RawSalary,
                    RawCPI = input.RawCPI,
                    RawCompany = input.RawCompanyCount,
                    RawPopulation = input.RawPopulation,

                    NormalizedSalary = input.NormSalary,
                    NormalizedCPI = input.NormCPI,
                    NormalizedCompany = input.NormCompanyCount,
                    NormalizedPopulation = input.NormPopulation,

                    PurchasingPowerIndex = Math.Round(ppi, 2),
                    RecommendationNote = GenerateNote(totalScore)
                };
            })
            .OrderByDescending(r => r.TotalScore)
            .ToList();

            for (int i = 0; i < results.Count; i++)
            {
                results[i].Rank = i + 1;
                results[i].UrgencyStatus = results[i].Rank switch {
                    1 => "Highest Recommendation",
                    <= 3 => "Top Alternative",
                    _ => "General Option"
                };
            }

            return results;
        }

        private string GenerateNote(double score)
        {
            if (score > 0.8) return "Sangat Direkomendasikan: Wilayah ini menawarkan efisiensi biaya hidup terbaik untuk gaji profesi Anda.";
            if (score > 0.6) return "Direkomendasikan: Wilayah ini memiliki keseimbangan yang baik antara peluang kerja dan beban hidup.";
            if (score > 0.4) return "Cukup: Wilayah ini layak, namun perhatikan tingginya pengeluaran bulanan dibandingkan wilayah lain.";
            return "Pertimbangkan Kembali: Lokasi ini mungkin memiliki biaya hidup yang terlalu tinggi atau peluang industri yang terbatas.";
        }
    }
}