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
        /// </summary>
        /// <param name="rawData">Daftar data gabungan dari repository.</param>
        /// <returns>Daftar data yang sudah siap dihitung (skala 0-1).</returns>
        List<SAWInput> PrepareAndNormalize(List<RawDataRow> rawData);

        /// <summary>
        /// Menghitung skor akhir (V) dengan mengalikan nilai ternormalisasi (R) dengan bobot (W).
        /// </summary>
        /// <param name="normalizedInputs">Data hasil normalisasi.</param>
        /// <param name="weights">Daftar bobot kepentingan untuk tiap kriteria.</param>
        /// <returns>Hasil ranking lokasi lengkap dengan kontribusi skor per kriteria.</returns>
        List<SAWResult> CalculateRanking(List<SAWInput> normalizedInputs, List<Weight> weights);
    }
}