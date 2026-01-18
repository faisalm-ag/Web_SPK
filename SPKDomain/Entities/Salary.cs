using System.Diagnostics.CodeAnalysis;

namespace SPKDomain.Entities
{
    /// <summary>
    /// Entitas yang merepresentasikan data dari FINAL_Gaji.csv.
    /// Digunakan untuk analisis kriteria C1 (Gaji Baseline Nasional).
    /// </summary>
    public class Salary
    {
        // Properti Area dihapus karena FINAL_Gaji.csv datanya bersifat Nasional

        // cat03_code (Contoh: 1031, 1051)
        public required string JobCode { get; set; }
        
        // job_category_en (Contoh: Engineering & Research, IT, dsb) 
        public required string JobCategoryEn { get; set; }

        // job_name_en (Contoh: Technical Specialist)
        public required string JobNameEn { get; set; }

        // time_code (Contoh: 2021000000)
        public long TimeCode { get; set; } 
        
        // is_salary_data (1 untuk Gaji Bulanan, 0 untuk lainnya)
        public int IsSalaryData { get; set; }

        // clean_value (Nilai numerik gaji dari hasil pembersihan)
        public double SalaryValue { get; set; }

        public Salary() { }

        [SetsRequiredMembers]
        public Salary(string jobCode, string jobCategoryEn, string jobNameEn, long timeCode, int isSalaryData, double salaryValue)
        {
            JobCode = jobCode;
            JobCategoryEn = jobCategoryEn;
            JobNameEn = jobNameEn;
            TimeCode = timeCode;
            IsSalaryData = isSalaryData;
            SalaryValue = salaryValue;
        }
    }
}