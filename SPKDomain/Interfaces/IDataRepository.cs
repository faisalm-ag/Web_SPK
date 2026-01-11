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
        /// Digunakan sebagai basis perhitungan normalisasi SAW.
        /// </summary>
        Task<List<RawDataRow>> GetRawDataAsync();

        /// <summary>
        /// Mengambil data gabungan yang sudah difilter berdasarkan kode pekerjaan tertentu.
        /// Penting agar analisis gaji relevan dengan minat mahasiswa.
        /// </summary>
        Task<List<RawDataRow>> GetRawDataByJobAsync(string jobCode);

        // --- Method Pengambilan Data Satuan (Dataset 1 - 4) ---
        
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