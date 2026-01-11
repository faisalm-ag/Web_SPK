namespace SPKDomain.ValueObjects
{
    /// <summary>
    /// Representasi nilai skor tunggal untuk wilayah tertentu.
    /// Digunakan dalam proses pemetaan hasil normalisasi per kriteria 
    /// sebelum dikalikan dengan bobot.
    /// </summary>
    public class Score
    {
        // Kode wilayah sebagai identitas unik
        public string AreaCode { get; init; } = string.Empty;
        
        // Nilai skor (bisa nilai mentah atau nilai normalisasi 0.0 - 1.0)
        public double Value { get; init; }

        public Score(string areaCode, double value)
        {
            AreaCode = areaCode;
            Value = value;
        }

        /// <summary>
        /// Mendapatkan representasi persentase dari skor (jika nilai adalah normalisasi).
        /// </summary>
        public double AsPercentage() => System.Math.Round(Value * 100, 2);
    }
}