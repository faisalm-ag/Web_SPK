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
            var pathCombine = Path.Combine(rootDir, "wwwroot", "data", "datasets");

            if (!Directory.Exists(pathCombine))
            {
                pathCombine = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "data", "datasets");
            }
            
            _basePath = pathCombine;
        }

        public void ClearCache()
        {
            _salaryCache = null;
            _cpiCache = null;
            _companyCache = null;
            _populationCache = null;
        }

        public async Task<List<RawDataRow>> GetRawDataAsync()
        {
            var salaries = await GetSalariesAsync();
            var defaultJob = salaries.FirstOrDefault()?.JobCode ?? "1072"; // Default ke IT jika kosong
            return await GetRawDataByJobAsync(defaultJob);
        }

        public async Task<List<RawDataRow>> GetRawDataByJobAsync(string jobCode)
        {
            var rawDataList = new List<RawDataRow>();

            var populations = await GetPopulationsAsync();
            var cpis = await GetCPIAsync();
            var salaries = await GetSalariesAsync();
            var companies = await GetCompaniesAsync();

            // Ambil gaji terbaru untuk job yang dipilih
            var targetSalary = salaries
                .Where(s => s.JobCode == jobCode || s.JobName == jobCode)
                .OrderByDescending(s => s.Year)
                .FirstOrDefault();

            var salaryValue = targetSalary?.SalaryValue ?? 0;
            var jobName = targetSalary?.JobName ?? "Selected Industry";

            // Kita gunakan Populasi sebagai base (Master Area)
            // Filter hanya level Prefektur (AreaCode berakhiran 000) agar ranking adil
            var prefecturePopulations = populations.Where(p => p.AreaCode.EndsWith("000") || p.AreaCode.Length <= 2);

            foreach (var pop in prefecturePopulations)
            {
                // Ambil 2 digit awal untuk sinkronisasi dengan CPI (e.g., 13000 -> 13)
                var prefPrefix = pop.AreaCode.Substring(0, 2);

                // Cari CPI yang paling relevan (prefix match)
                var cpiRow = cpis.FirstOrDefault(c => c.AreaCode.StartsWith(prefPrefix));
                
                // Cari data perusahaan di prefektur yang sama
                var companyRow = companies.FirstOrDefault(cp => cp.AreaCode == pop.AreaCode);
                
                rawDataList.Add(new RawDataRow
                {
                    AreaCode = pop.AreaCode,
                    AreaName = pop.AreaName,
                    JobCode = jobCode,
                    BidangPekerjaan = jobName,
                    Population = pop.PopulationK,
                    ConsumerPriceIndex = cpiRow?.CPIIndex ?? 100.0, 
                    AverageSalary = salaryValue, // Gaji spesifik job
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
                AreaCode = cols.Length > 0 ? cols[0] : "", 
                AreaName = cols.Length > 1 ? cols[1] : "", 
                Period = cols.Length > 2 ? cols[2] : "", 
                CPIIndex = cols.Length > 3 ? ParseDouble(cols[3]) : 100.0
            }).ToList();
            return _cpiCache;
        }

        public async Task<List<Salary>> GetSalariesAsync()
        {
            if (_salaryCache != null) return _salaryCache;
            var data = await ReadCsvAsync(Path.Combine(_basePath, "FINAL_Gaji.csv"));
            _salaryCache = data.Select(cols => new Salary 
            { 
                JobCode = cols.Length > 0 ? cols[0] : "", 
                JobName = cols.Length > 1 ? cols[1] : "", 
                Year = cols.Length > 2 && long.TryParse(cols[2], out long y) ? y : 0, 
                SalaryValue = cols.Length > 3 ? ParseDouble(cols[3]) : 0 
            }).ToList();
            return _salaryCache;
        }

        public async Task<List<Company>> GetCompaniesAsync()
        {
            if (_companyCache != null) return _companyCache;
            var data = await ReadCsvAsync(Path.Combine(_basePath, "FINAL_Perusahaan.csv"));
            _companyCache = data.Select(cols => new Company 
            { 
                IndustryCode = cols.Length > 0 ? cols[0] : "", 
                IndustryName = cols.Length > 1 ? cols[1] : "", 
                AreaCode = cols.Length > 2 ? cols[2] : "", 
                AreaName = cols.Length > 3 ? cols[3] : "", 
                EstimatedCount = cols.Length > 4 ? ParseDouble(cols[4]) : 0 
            }).ToList();
            return _companyCache;
        }

        public async Task<List<Population>> GetPopulationsAsync()
        {
            if (_populationCache != null) return _populationCache;
            var data = await ReadCsvAsync(Path.Combine(_basePath, "FINAL_Populasi.csv"));
            _populationCache = data.Select(cols => new Population 
            { 
                AreaCode = cols.Length > 0 ? cols[0] : "", 
                AreaName = cols.Length > 1 ? cols[1] : "", 
                Year = cols.Length > 2 && int.TryParse(cols[2], out int y) ? y : 0, 
                PopulationK = cols.Length > 3 ? ParseDouble(cols[3]) : 0
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

                // Regex sakti untuk menangani koma di dalam tanda kutip (penting untuk nama industri/area)
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
            // Menangani angka dengan format ribuan atau desimal titik
            return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result) ? result : 0;
        }
    }
}