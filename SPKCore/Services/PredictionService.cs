using System;
using System.Collections.Generic;
using System.Linq;
using SPKDomain.Models;

namespace SPKCore.Services
{
    /// <summary>
    /// Service untuk menangani logika Machine Learning (Klasifikasi).
    /// Memproses input kuesioner mahasiswa untuk memprediksi potensi kepuasan.
    /// </summary>
    public class PredictionService
    {
        /// <summary>
        /// Melakukan prediksi berdasarkan skor kuesioner yang dikirim dari UI.
        /// </summary>
        /// <param name="answers">Kumpulan nilai jawaban (skala 1-5).</param>
        /// <returns>Hasil prediksi berupa label dan tingkat kepercayaan.</returns>
        public PredictionResult PredictSatisfaction(List<int> answers)
        {
            if (answers == null || !answers.Any())
                return new PredictionResult { PredictedLabel = "Tidak Diketahui", ConfidenceScore = 0 };

            // 1. Hitung rata-rata skor kuesioner sebagai fitur utama (Feature Engineering sederhana)
            double averageScore = answers.Average();
            
            // 2. Simulasi Model Klasifikasi (Threshold-based Classification)
            // Dalam ML.NET, bagian ini digantikan oleh: _predictionEngine.Predict(input)
            string label;
            float confidence;
            string recommendation;

            if (averageScore >= 4.0)
            {
                label = "Sangat Siap & Puas";
                confidence = (float)(averageScore / 5.0);
                recommendation = "Profil Anda menunjukkan kemandirian dan kesiapan mental yang sangat tinggi untuk budaya kerja Jepang.";
            }
            else if (averageScore >= 3.0)
            {
                label = "Siap & Puas";
                confidence = (float)(averageScore / 5.0);
                recommendation = "Anda memiliki potensi adaptasi yang baik, namun perlu penguatan pada aspek teknis bahasa.";
            }
            else
            {
                label = "Kurang Siap";
                confidence = (float)(averageScore / 5.0);
                recommendation = "Disarankan untuk mengikuti pelatihan tambahan sebelum keberangkatan agar tingkat kepuasan Anda terjaga.";
            }

            return new PredictionResult
            {
                PredictedLabel = label,
                ConfidenceScore = confidence,
                AnalysisSummary = recommendation
            };
        }

        /// <summary>
        /// Menghitung skor numerik berdasarkan kuesioner untuk dikombinasikan dengan SAW.
        /// </summary>
        public double GetProfileWeight(List<int> answers)
        {
            return answers.Any() ? answers.Average() / 5.0 : 0.5;
        }
    }
}