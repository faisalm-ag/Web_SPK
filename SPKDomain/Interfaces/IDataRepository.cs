using SPKDomain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPKDomain.Interfaces
{
    /// <summary>
    /// Interface untuk mengakses dataset resmi dari portal e-Stat Japan.
    /// Bertanggung jawab atas abstraksi pengambilan data Gaji, CPI, Perusahaan, dan Populasi.
    /// </summary>
    public interface IDataRepository
    {
        /// <summary>
        /// Mengambil data gabungan (RawDataRow) untuk seluruh wilayah.
        /// </summary>
        Task<List<RawDataRow>> GetRawDataAsync();

        /// <summary>
        /// Mengambil data gabungan yang sudah difilter berdasarkan kode pekerjaan tertentu.
        /// </summary>
        Task<List<RawDataRow>> GetRawDataByJobAsync(string jobCode);

        // --- Method Baru untuk Sinkronisasi Dropdown Biodata ---

        /// <summary>
        /// Mengambil daftar wilayah (Prefektur) unik dari dataset (FINAL_Perusahaan.csv / FINAL_CPI.csv).
        /// </summary>
        Task<List<string>> GetAvailableAreasAsync();

        /// <summary>
        /// Mengambil daftar kategori pekerjaan unik dari dataset (FINAL_Gaji.csv).
        /// </summary>
        Task<List<string>> GetAvailableJobsAsync();

        // --- Method Pengambilan Data Satuan ---
        
        Task<List<Salary>> GetSalariesAsync();
        
        Task<List<CPI>> GetCPIAsync();
        
        Task<List<Company>> GetCompaniesAsync();
        
        Task<List<Population>> GetPopulationsAsync();

        /// <summary>
        /// Membersihkan cache data jika ada pembaruan pada file CSV.
        /// </summary>
        void ClearCache();
    }
}