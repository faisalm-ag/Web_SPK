using SPKDomain.Entities;
using SPKDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SPKInfrastructure.Repositories
{
    public class CsvDataRepository : IDataRepository
    {
        private readonly string _basePath;
        private List<Salary>? _salaryCache;
        private List<CPI>? _cpiCache;
        private List<Company>? _companyCache;
        private List<Population>? _populationCache;

        public CsvDataRepository()
        {
            var rootDir = Directory.GetCurrentDirectory();
            _basePath = Path.Combine(rootDir, "wwwroot", "data", "datasets");
            
            if (!Directory.Exists(_basePath))
            {
                _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "data", "datasets");
            }
        }

        public void ClearCache()
        {
            _salaryCache = null;
            _cpiCache = null;
            _companyCache = null;
            _populationCache = null;
        }

        public async Task<List<string>> GetAvailableAreasAsync()
        {
            var populations = await GetPopulationsAsync();
            return populations
                .Where(p => p.StandardizedAreaEn != "National")
                .Select(p => p.StandardizedAreaEn)
                .Distinct()
                .OrderBy(a => a)
                .ToList();
        }

        public async Task<List<string>> GetAvailableJobsAsync()
        {
            var salaries = await GetSalariesAsync();
            return salaries
                .Where(s => !string.IsNullOrEmpty(s.JobNameEn))
                .Select(s => s.JobNameEn)
                .Distinct()
                .OrderBy(j => j)
                .ToList();
        }

        public async Task<List<RawDataRow>> GetRawDataAsync()
        {
            return await GetRawDataByJobAsync("Technical Specialist");
        }

        public async Task<List<RawDataRow>> GetRawDataByJobAsync(string jobName)
        {
            var rawDataList = new List<RawDataRow>();

            var populations = await GetPopulationsAsync();
            var cpis = await GetCPIAsync();
            var salaries = await GetSalariesAsync();
            var companies = await GetCompaniesAsync();

            // 1. Ambil data gaji nasional untuk job terpilih (Gaji adalah kriteria Benefit)
            var nationalSalary = salaries
                .Where(s => s.JobNameEn == jobName && s.IsSalaryData == 1)
                .OrderByDescending(s => s.TimeCode)
                .FirstOrDefault();

            var filteredCPI = cpis.Where(c => c.IsGeneralIndex == 1).ToList();
            var filteredPop = populations.Where(p => p.IsTotalPop == 1).OrderByDescending(p => p.Year).ToList();
            var filteredComp = companies.Where(c => c.IsTotalIndustry == 1 && c.AreaLevel == "Prefecture").ToList();

            var latestPopByArea = filteredPop
                .GroupBy(p => p.StandardizedAreaEn)
                .Select(g => g.First())
                .Where(p => p.StandardizedAreaEn != "National");

            foreach (var pop in latestPopByArea)
            {
                var cpiRow = filteredCPI.FirstOrDefault(c => c.StandardizedAreaEn == pop.StandardizedAreaEn);
                var companyRow = filteredComp.FirstOrDefault(c => c.StandardizedAreaEn == pop.StandardizedAreaEn);

                rawDataList.Add(new RawDataRow
                {
                    AreaCode = pop.AreaCode,
                    AreaName = pop.StandardizedAreaEn,
                    JobCode = jobName,
                    BidangPekerjaan = jobName,
                    Population = pop.PopulationK,
                    ConsumerPriceIndex = cpiRow?.CPIIndex ?? 100.0,
                    // Di sinilah Logika Nasional bekerja: Gaji yang sama digunakan untuk semua prefektur
                    AverageSalary = nationalSalary?.SalaryValue ?? 0, 
                    CompanyCount = companyRow?.EstimatedCount ?? 0
                });
            }

            return rawDataList;
        }

        public async Task<List<CPI>> GetCPIAsync()
        {
            if (_cpiCache != null) return _cpiCache;
            var data = await ReadCsvAsync(Path.Combine(_basePath, "FINAL_CPI.csv"));
            
            _cpiCache = data.Select(cols => new CPI 
            { 
                AreaCode = cols[4], 
                AreaName = cols[5], 
                StandardizedAreaEn = cols[11],
                TimeCode = long.TryParse(cols[6], out long t) ? t : 0,
                IsGeneralIndex = int.TryParse(cols[12], out int g) ? g : 0,
                CPIIndex = ParseDouble(cols[13])
            }).ToList();
            return _cpiCache;
        }

        public async Task<List<Salary>> GetSalariesAsync()
        {
            if (_salaryCache != null) return _salaryCache;
            var data = await ReadCsvAsync(Path.Combine(_basePath, "FINAL_Gaji.csv"));
            
            _salaryCache = data.Select(cols => new Salary 
            { 
                // Index disesuaikan dengan FINAL_Gaji.csv Anda
                JobCode = cols[6], 
                JobCategoryEn = cols[13],
                JobNameEn = cols[14],
                TimeCode = long.TryParse(cols[8], out long t) ? t : 0,
                IsSalaryData = int.TryParse(cols[15], out int s) ? s : 0,
                SalaryValue = ParseDouble(cols[17])
            }).ToList();
            return _salaryCache;
        }

        public async Task<List<Company>> GetCompaniesAsync()
        {
            if (_companyCache != null) return _companyCache;
            var data = await ReadCsvAsync(Path.Combine(_basePath, "FINAL_Perusahaan.csv"));
            
            _companyCache = data.Select(cols => new Company 
            { 
                IndustryCode = cols[2], 
                IndustryNameEn = cols[15],
                AreaCode = cols[6],
                AreaNameRaw = cols[7],
                StandardizedAreaEn = cols[13],
                AreaLevel = cols[14],
                IsTotalIndustry = int.TryParse(cols[16], out int i) ? i : 0,
                EstimatedCount = ParseDouble(cols[18])
            }).ToList();
            return _companyCache;
        }

        public async Task<List<Population>> GetPopulationsAsync()
        {
            if (_populationCache != null) return _populationCache;
            var data = await ReadCsvAsync(Path.Combine(_basePath, "FINAL_Populasi.csv"));
            
            _populationCache = data.Select(cols => new Population 
            { 
                AreaCode = cols[4], 
                AreaNameRaw = cols[5],
                StandardizedAreaEn = cols[11],
                Year = (int)ParseDouble(cols[12]),
                IsTotalPop = int.TryParse(cols[13], out int tp) ? tp : 0,
                PopulationK = ParseDouble(cols[14])
            }).ToList();
            return _populationCache;
        }

        private async Task<List<string[]>> ReadCsvAsync(string filePath)
        {
            var results = new List<string[]>();
            if (!File.Exists(filePath)) return results;

            using var reader = new StreamReader(filePath);
            await reader.ReadLineAsync(); // Skip header

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var columns = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)")
                                    .Select(s => s.Trim().Trim('"'))
                                    .ToArray();
                
                results.Add(columns);
            }
            return results;
        }

        private double ParseDouble(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result) ? result : 0;
        }
    }
}