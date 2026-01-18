using System;
using System.Collections.Generic;
using System.Linq;
using SPKDomain.Models;

namespace SPKCore.Services
{
    /// <summary>
    /// Service untuk menangani logika Machine Learning (Klasifikasi Kesiapan).
    /// Memproses input kuesioner mahasiswa untuk memprediksi potensi kepuasan di Jepang.
    /// </summary>
    public class PredictionService
    {
        /// <summary>
        /// Melakukan prediksi tingkat kesiapan/kepuasan berdasarkan skor kuesioner.
        /// Menggunakan logika Heuristic Classification yang dapat ditingkatkan ke ML.NET.
        /// </summary>
        /// <param name="answers">Kumpulan nilai jawaban kuesioner (1-5).</param>
        /// <returns>Objek PredictionResult untuk laporan akhir.</returns>
        public PredictionResult PredictSatisfaction(List<int> answers)
        {
            if (answers == null || !answers.Any())
            {
                return new PredictionResult 
                { 
                    SatisfactionLevel = "Unknown", 
                    Probability = 0.5,
                    AnalysisSummary = "Data kuesioner tidak lengkap."
                };
            }

            // 1. Feature Engineering: Menghitung mean (fitur utama kesiapan)
            double averageScore = answers.Average();
            
            // 2. Klasifikasi & Probability Mapping
            // Logika ini mensimulasikan output dari algoritma Multi-class Classification
            string level;
            string summary;
            double probability = averageScore / 5.0; // Normalisasi ke 0.0 - 1.0

            if (averageScore >= 4.2)
            {
                level = "High";
                summary = "Profil menunjukkan kemandirian dan kesiapan mental yang luar biasa. Anda diprediksi akan sangat puas dengan budaya kerja di lokasi pilihan.";
            }
            else if (averageScore >= 3.0)
            {
                level = "Medium";
                summary = "Anda memiliki potensi adaptasi yang baik. Kepuasan Anda akan bergantung pada dukungan komunitas di wilayah tujuan.";
            }
            else
            {
                level = "Low";
                summary = "Disarankan untuk memperkuat persiapan bahasa dan mental sebelum berangkat untuk menghindari 'culture shock' yang berat.";
            }

            return new PredictionResult
            {
                SatisfactionLevel = level,
                Probability = Math.Round(probability, 2),
                AnalysisSummary = summary
            };
        }

        /// <summary>
        /// Memberikan nilai bobot profil mahasiswa (0-1).
        /// Digunakan sebagai pengali dalam perhitungan Final Eligibility Score.
        /// </summary>
        public double GetProfileReliability(List<int> answers)
        {
            if (answers == null || !answers.Any()) return 0.5;
            
            // Memberikan penalti jika jawaban terlalu ekstrem (inkonsisten)
            double stdDev = CalculateStandardDeviation(answers);
            double baseScore = answers.Average() / 5.0;

            return stdDev > 1.5 ? baseScore * 0.8 : baseScore;
        }

        private double CalculateStandardDeviation(List<int> values)
        {
            double avg = values.Average();
            double sum = values.Sum(v => Math.Pow(v - avg, 2));
            return Math.Sqrt(sum / values.Count);
        }
    }
}