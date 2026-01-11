using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace WebAppSPK.ViewModels
{
    /// <summary>
    /// ViewModel untuk menangkap data awal mahasiswa (Step 1).
    /// Berfungsi sebagai filter awal untuk menentukan Standar Gaji (C1) 
    /// dan Area fokus analisis.
    /// </summary>
    public class BiodataVM
    {
        [Required(ErrorMessage = "Nama lengkap wajib diisi untuk kebutuhan sertifikat/laporan.")]
        [Display(Name = "Nama Lengkap")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Nama minimal 3 - 50 karakter.")]
        public string Nama { get; set; } = string.Empty;

        [Required(ErrorMessage = "Silakan pilih prefektur fokus utama Anda.")]
        [Display(Name = "Prefektur Prioritas")]
        public string Lokasi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sektor pekerjaan wajib dipilih untuk kalkulasi skor ekonomi.")]
        [Display(Name = "Sektor Pekerjaan")]
        public string Bidang { get; set; } = string.Empty;

        // Properti helper untuk mengisi dropdown di halaman Razor Pages / MVC
        public List<SelectListItem> PrefekturOptions { get; set; } = new();
        public List<SelectListItem> BidangOptions { get; set; } = new();

        /// <summary>
        /// Digunakan untuk menyimpan state progress user di UI
        /// </summary>
        public int CurrentStep => 1;
    }
}