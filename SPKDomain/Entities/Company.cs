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
        // cat01_code (Contoh: AR, 06, D)
        public required string IndustryCode { get; set; }

        // industry_name_en (Contoh: All Industries, Construction, dsb)
        public required string IndustryNameEn { get; set; }

        // area_code (Contoh: 1000, 1100)
        public required string AreaCode { get; set; }

        // Area (Nama asli dari dataset: Hokkaido, Sapporo-shi)
        public required string AreaNameRaw { get; set; }

        // standardized_area_en (Contoh: Hokkaido) - Kunci Join
        public required string StandardizedAreaEn { get; set; }

        // area_level (Prefecture / City / National)
        // Memastikan kita tidak menghitung ganda antara kota dan prefektur
        public required string AreaLevel { get; set; }

        // is_total_industry (1 jika 'All Industries', 0 jika spesifik)
        public int IsTotalIndustry { get; set; }

        // clean_value (Jumlah unit usaha/perusahaan)
        public double EstimatedCount { get; set; }

        // Helper property untuk mempermudah filter di Repository/Service
        public bool IsPrefectureLevel => AreaLevel == "Prefecture";

        public Company() { }

        [SetsRequiredMembers]
        public Company(string industryCode, string industryNameEn, string areaCode, string areaNameRaw, 
                       string standardizedAreaEn, string areaLevel, int isTotalIndustry, double estimatedCount)
        {
            IndustryCode = industryCode;
            IndustryNameEn = industryNameEn;
            AreaCode = areaCode;
            AreaNameRaw = areaNameRaw;
            StandardizedAreaEn = standardizedAreaEn;
            AreaLevel = areaLevel;
            IsTotalIndustry = isTotalIndustry;
            EstimatedCount = estimatedCount;
        }
    }
}