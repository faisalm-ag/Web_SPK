using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace WebAppSPK.ViewModels
{
    public class BiodataVM
    {
        [Required(ErrorMessage = "Nama lengkap wajib diisi.")]
        [Display(Name = "Nama Lengkap")]
        public string Nama { get; set; } = string.Empty;

        [Required(ErrorMessage = "Silakan pilih prefektur minat Anda.")]
        [Display(Name = "Prefektur Minat")]
        public string Lokasi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Silakan pilih sektor pekerjaan.")]
        [Display(Name = "Sektor Pekerjaan")]
        public string Bidang { get; set; } = string.Empty;

        // Penampung data dinamis untuk Dropdown
        public List<SelectListItem> PrefekturOptions { get; set; } = new();
        public List<SelectListItem> BidangOptions { get; set; } = new();

        public int CurrentStep => 1;
        public int TotalSteps => 4; 
    }
}