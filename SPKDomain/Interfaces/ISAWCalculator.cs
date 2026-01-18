using System.Collections.Generic;
using SPKDomain.Models;
using SPKDomain.Entities;
using SPKDomain.ValueObjects;

namespace SPKDomain.Interfaces
{
    /// <summary>
    /// Kontrak untuk mesin perhitungan Simple Additive Weighting (SAW).
    /// Mengelola proses normalisasi dan perankingan lokasi magang.
    /// </summary>
    public interface ISAWCalculator
    {
        /// <summary>
        /// Mengubah data mentah (Dataset 1-4) menjadi objek SAWInput yang berisi nilai ternormalisasi.
        /// Sekaligus menyuntikkan bobot (W) dari kuesioner ke dalam tiap baris data.
        /// </summary>
        /// <param name="rawData">Daftar data gabungan dari repository.</param>
        /// <param name="weights">Bobot kepentingan hasil kuesioner mahasiswa.</param>
        /// <returns>Daftar data yang sudah siap dihitung (skala 0-1) dan membawa informasi bobot.</returns>
        List<SAWInput> PrepareAndNormalize(List<RawDataRow> rawData, List<Weight> weights);

        /// <summary>
        /// Menghitung skor akhir (V) dengan mengalikan nilai ternormalisasi (R) dengan bobot (W).
        /// Rumus: Vi = Σ (Wj * Rij)
        /// </summary>
        /// <param name="normalizedInputs">Data hasil normalisasi yang sudah memiliki bobot.</param>
        /// <returns>Hasil ranking lokasi lengkap dengan skor total dan detail kontribusi kriteria.</returns>
        List<SAWResult> CalculateRanking(List<SAWInput> normalizedInputs);
    }
}