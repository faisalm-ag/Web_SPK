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
        public List<SAWInput> PrepareAndNormalize(List<RawDataRow> rawData)
        {
            if (rawData == null || !rawData.Any()) 
                return new List<SAWInput>();

            // 1. Mencari Nilai Ekstrem (Max/Min) untuk proses Normalisasi
            double maxSalary = Math.Max(rawData.Max(x => x.AverageSalary), 0.0001);
            double minCPI = Math.Max(rawData.Min(x => x.ConsumerPriceIndex), 0.0001);
            double maxCompanies = Math.Max(rawData.Max(x => x.CompanyCount), 0.0001);
            double maxPopulation = Math.Max(rawData.Max(x => x.Population), 0.0001);

            // 2. Transformasi ke SAWInput (Skala 0-1)
            return rawData.Select(item => new SAWInput
            {
                AreaCode = item.AreaCode,
                AreaName = item.AreaName,
                JobCode = item.JobCode,
                
                // Normalisasi C1: Gaji (Benefit) -> x / max
                NormSalary = item.AverageSalary / maxSalary,
                
                // Normalisasi C2: CPI (Cost) -> min / x
                NormCPI = item.ConsumerPriceIndex > 0 ? minCPI / item.ConsumerPriceIndex : 0,
                
                // Normalisasi C3: Perusahaan (Benefit) -> x / max
                NormCompanyCount = item.CompanyCount / maxCompanies,
                
                // Normalisasi C4: Populasi (Benefit) -> x / max
                NormPopulation = item.Population / maxPopulation,

                // Menyimpan data asli (Raw)
                RawSalary = item.AverageSalary,
                RawCPI = item.ConsumerPriceIndex,
                RawCompanyCount = item.CompanyCount,
                RawPopulation = item.Population
            }).ToList();
        }

        public List<SAWResult> CalculateRanking(List<SAWInput> normalizedInputs, List<Weight> weights)
        {
            if (normalizedInputs == null || !normalizedInputs.Any())
                return new List<SAWResult>();

            // Ambil nilai bobot
            double wSalary = weights.FirstOrDefault(w => w.Criteria == "Salary")?.Value ?? 0.25;
            double wCPI = weights.FirstOrDefault(w => w.Criteria == "CPI")?.Value ?? 0.25;
            double wCompany = weights.FirstOrDefault(w => w.Criteria == "Company")?.Value ?? 0.25;
            double wPop = weights.FirstOrDefault(w => w.Criteria == "Population")?.Value ?? 0.25;

            var results = normalizedInputs.Select(input =>
            {
                // Rumus SAW: V_i = Σ (w_j * r_ij)
                double sContrib = input.NormSalary * wSalary;
                double cContrib = input.NormCPI * wCPI;
                double oContrib = input.NormCompanyCount * wCompany;
                double pContrib = input.NormPopulation * wPop;

                double totalScore = sContrib + cContrib + oContrib + pContrib;

                // Hitung Purchasing Power Index (Gaji / CPI) sebagai nilai tambah analisis
                double ppi = input.RawCPI > 0 ? input.RawSalary / input.RawCPI : 0;

                return new SAWResult
                {
                    AreaCode = input.AreaCode,
                    AreaName = input.AreaName,
                    TotalScore = Math.Round(totalScore, 4),
                    
                    // Kontribusi skor
                    SalaryScoreContribution = sContrib,
                    CPIScoreContribution = cContrib,
                    OpportunityScoreContribution = oContrib,
                    PopulationScoreContribution = pContrib,

                    // Data Raw
                    RawSalary = input.RawSalary,
                    RawCPI = input.RawCPI,
                    RawCompany = input.RawCompanyCount,
                    RawPopulation = input.RawPopulation,

                    // Data Ter-normalisasi
                    NormalizedSalary = input.NormSalary,
                    NormalizedCPI = input.NormCPI,
                    NormalizedCompany = input.NormCompanyCount,
                    NormalizedPopulation = input.NormPopulation,

                    // Analisis Ekonomi tambahan
                    PurchasingPowerIndex = Math.Round(ppi, 2),
                    RecommendationNote = GenerateNote(totalScore)
                };
            })
            .OrderByDescending(r => r.TotalScore)
            .ToList();

            // Penentuan Rank
            // CATATAN: SuccessRate tidak perlu diisi manual karena sudah dihitung otomatis 
            // di properti SAWResult.SuccessRate (get-only)
            for (int i = 0; i < results.Count; i++)
            {
                results[i].Rank = i + 1;
                
                // Menentukan status urgensi berdasarkan rank
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
            if (score > 0.8) return "Sangat Direkomendasikan: Keseimbangan ekonomi dan profil wilayah sangat ideal untuk profil Anda.";
            if (score > 0.6) return "Direkomendasikan: Kondisi wilayah cukup kompetitif dan layak dipertimbangkan.";
            if (score > 0.4) return "Cukup: Wilayah ini memenuhi standar minimum namun memiliki beberapa kekurangan di faktor utama.";
            return "Pertimbangkan Kembali: Faktor biaya hidup atau ketersediaan industri mungkin tidak sesuai dengan prioritas Anda.";
        }
    }
}