using System.Diagnostics.CodeAnalysis;

namespace SPKDomain.Entities
{
    /// <summary>
    /// Entitas yang merepresentasikan data dari FINAL_CPI.csv.
    /// Digunakan untuk analisis biaya hidup (Dataset 2).
    /// Dalam SPK, kriteria ini bersifat "Cost" (semakin kecil semakin baik).
    /// </summary>
    public class CPI
    {
        // area_code (Contoh: 13A01)
        public required string AreaCode { get; set; }

        // area_name / 地域（2020年基準） (Contoh: "13100 東京都区部")
        public required string AreaName { get; set; }

        // standardized_area_en (Contoh: Tokyo)
        // Kunci utama untuk join dengan dataset Gaji, Perusahaan, dan Populasi
        public required string StandardizedAreaEn { get; set; }

        // time_code (Contoh: 2025001212)
        public long TimeCode { get; set; }

        // is_general_index (1 jika "0001 総合", 0 jika kategori spesifik)
        // Memastikan kita menghitung biaya hidup total dalam rumus SAW
        public int IsGeneralIndex { get; set; }

        // clean_value / CPI Index (C2: Cost)
        public double CPIIndex { get; set; }

        public CPI() { }

        [SetsRequiredMembers]
        public CPI(string areaCode, string areaName, string standardizedAreaEn, long timeCode, int isGeneralIndex, double cpiIndex)
        {
            AreaCode = areaCode;
            AreaName = areaName;
            StandardizedAreaEn = standardizedAreaEn;
            TimeCode = timeCode;
            IsGeneralIndex = isGeneralIndex;
            CPIIndex = cpiIndex;
        }
    }
}