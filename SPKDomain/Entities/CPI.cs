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
        // Kode Wilayah (Contoh: 13A01)
        public required string AreaCode { get; set; }

        // Nama Wilayah Asli (Contoh: "13100 東京都区部")
        public required string AreaName { get; set; }

        // Nama Wilayah Bersih/Romaji (Contoh: "Tokyo-to")
        // Akan diisi melalui AnalysisService untuk tampilan UI
        public string? AreaNameDisplay { get; set; }

        // Periode data (Contoh: 2025001212)
        public required string Period { get; set; }

        // Nilai Indeks Harga Konsumen (C2: Cost)
        public double CPIIndex { get; set; }

        public CPI() { }

        [SetsRequiredMembers]
        public CPI(string areaCode, string areaName, string period, double cpiIndex)
        {
            AreaCode = areaCode;
            AreaName = areaName;
            Period = period;
            CPIIndex = cpiIndex;
        }
    }
}