namespace SPKDomain.Enums
{
    /// <summary>
    /// Menentukan tipe kriteria untuk perhitungan normalisasi pada metode Simple Additive Weighting (SAW).
    /// </summary>
    public enum CriteriaType
    {
        /// <summary>
        /// Kriteria yang nilainya semakin besar semakin baik (Contoh: Gaji, Jumlah Perusahaan).
        /// Rumus Normalisasi: R = Nilai / Max(Nilai)
        /// </summary>
        Benefit,

        /// <summary>
        /// Kriteria yang nilainya semakin kecil semakin baik (Contoh: Indeks Biaya Hidup/CPI).
        /// Rumus Normalisasi: R = Min(Nilai) / Nilai
        /// </summary>
        Cost
    }
}