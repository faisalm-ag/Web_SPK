using System.ComponentModel.DataAnnotations;

namespace WebAppSPK.ViewModels
{
    /// <summary>
    /// ViewModel untuk satu butir pertanyaan kuesioner.
    /// Input dari sini digunakan sebagai dasar penentuan Bobot SAW (C1-C4)
    /// dan fitur (feature) prediksi kesiapan mental oleh Machine Learning.
    /// </summary>
    public class QuestionVM
    {
        // Kategori kriteria (Salary, CPI, Company, Population, Komitmen, dsb.)
        public string Category { get; set; } = string.Empty;
        
        // Teks pertanyaan deskriptif
        public string Text { get; set; } = string.Empty;
        
        // Urutan pertanyaan untuk sinkronisasi array di controller
        public int Index { get; set; }
        
        // Nilai input pengguna (Skala Likert: 1-5)
        [Required]
        [Range(1, 5, ErrorMessage = "Skala penilaian harus antara 1 sampai 5.")]
        public int SelectedScore { get; set; } = 3; // Default: Tengah (Cukup)

        /// <summary>
        /// Mengambil ikon Bootstrap untuk mempercantik tampilan kuesioner di UI.
        /// </summary>
        public string GetCategoryIcon() => Category switch
        {
            "Salary" => "bi-cash-coin",
            "CPI" => "bi-cart-check",
            "Company" => "bi-building",
            "Population" => "bi-people",
            "Komitmen" => "bi-pencil-square",
            "Mental" => "bi-heart-pulse",
            "Etika" => "bi-shield-check",
            _ => "bi-question-circle"
        };

        /// <summary>
        /// Memberikan label deskriptif untuk ditampilkan di UI (ToolTip atau Label).
        /// </summary>
        public string GetScoreLabel() => SelectedScore switch
        {
            1 => "Sangat Tidak Penting / Tidak Siap",
            2 => "Kurang Penting / Kurang Siap",
            3 => "Cukup / Netral",
            4 => "Penting / Siap",
            5 => "Sangat Penting / Sangat Siap",
            _ => "Belum Memilih"
        };
    }
}