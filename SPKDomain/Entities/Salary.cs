using System.Diagnostics.CodeAnalysis;

namespace SPKDomain.Entities
{
    /// <summary>
    /// Entitas yang merepresentasikan data dari FINAL_Gaji.csv.
    /// Digunakan untuk analisis kesejahteraan (Dataset 1).
    /// </summary>
    public class Salary
    {
        // Job Code (Contoh: 1031, 1051) - Digunakan untuk filter berdasarkan input biodata
        public required string JobCode { get; set; }
        
        // Nama Pekerjaan Kanji (Contoh: 研究者) - Sesuai dataset mentah
        public required string JobName { get; set; }

        // Nama Pekerjaan Romaji/Indonesia - Untuk mempermudah mahasiswa saat pembacaan laporan
        public string? JobNameTranslated { get; set; }
        
        // Tahun Data dalam format dataset (Contoh: 2021000000)
        public long Year { get; set; } // Diubah ke long karena nilai di dataset mencapai 10 digit
        
        // Nilai Gaji (Semakin tinggi semakin baik bagi mahasiswa)
        public double SalaryValue { get; set; }

        public Salary() { }

        [SetsRequiredMembers]
        public Salary(string jobCode, string jobName, long year, double salaryValue)
        {
            JobCode = jobCode;
            JobName = jobName;
            Year = year;
            SalaryValue = salaryValue;
        }
    }
}