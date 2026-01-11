using System.ComponentModel.DataAnnotations;

namespace WebAppSPK.ViewModels
{
    /// <summary>
    /// ViewModel untuk satu butir pertanyaan kuesioner.
    /// Input dari sini digunakan sebagai fitur (feature) bagi Machine Learning.
    /// </summary>
    public class QuestionVM
    {
        // Kategori kriteria (Salary, CPI, Company, Population)
        public string Category { get; set; } = string.Empty;
        
        // Teks pertanyaan (Contoh: "Seberapa penting faktor gaji tinggi bagi Anda?")
        public string Text { get; set; } = string.Empty;
        
        public int Index { get; set; }
        
        // Nilai input pengguna (Skala Likert: 1-5)
        [Range(1, 5, ErrorMessage = "Skala penilaian harus antara 1 sampai 5.")]
        public int SelectedScore { get; set; } = 3; // Default: Cukup Penting

        /// <summary>
        /// Memberikan label deskriptif untuk ditampilkan di UI.
        /// </summary>
        public string GetScoreLabel() => SelectedScore switch
        {
            1 => "Sangat Tidak Penting",
            2 => "Kurang Penting",
            3 => "Cukup Penting",
            4 => "Penting",
            5 => "Sangat Penting",
            _ => "Belum Memilih"
        };
    }
}