using System.Diagnostics.CodeAnalysis;

namespace SPKDomain.Entities
{
    /// <summary>
    /// Entitas yang merepresentasikan data dari FINAL_Perusahaan.csv.
    /// Digunakan untuk analisis peluang ketersediaan tempat magang (Dataset 3).
    /// Dalam SPK, kriteria ini bersifat "Benefit" (semakin besar semakin baik).
    /// </summary>
    public class Company
    {
        // Sesuaikan dengan ind_code di CSV
        public required string IndustryCode { get; set; }

        public required string IndustryName { get; set; }

        public required string AreaCode { get; set; }

        public required string AreaName { get; set; }

        // Gunakan nama ini agar konsisten
        public double EstimatedCount { get; set; }

        // Logika untuk membedakan Prefektur (Hokkaido, Aomori) dan Kota (Sapporo, Sendai)
        // Berdasarkan dataset Anda, Prefektur memiliki AreaCode seperti 1000, 2000, 3000 (akhiran 000)
        public bool IsPrefecture => AreaCode.EndsWith("000");

        public Company() { }

        [SetsRequiredMembers]
        public Company(string industryCode, string industryName, string areaCode, string areaName, double estCount)
        {
            IndustryCode = industryCode;
            IndustryName = industryName;
            AreaCode = areaCode;
            AreaName = areaName;
            EstimatedCount = estCount;
        }
    }
}