using SPKDomain.Interfaces;
using SPKDomain.Models;
using SPKCore.Services;
using SPKInfrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Framework Services ---
builder.Services.AddControllersWithViews();

// Tambahkan Memory Cache agar Session bisa berjalan (Wajib untuk Biodata & Kuesioner)
builder.Services.AddDistributedMemoryCache(); 

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".CareerSPK.Session";
});

// --- 2. Application Layers (Dependency Injection) ---
// Mendaftarkan Repository untuk akses data CSV
builder.Services.AddScoped<IDataRepository, CsvDataRepository>();

// Mendaftarkan Kalkulator SAW (Logika Normalisasi & Perankingan)
builder.Services.AddScoped<ISAWCalculator, SAWCalculator>();

// Mendaftarkan Business Services untuk ML dan Analisis Tahapan
builder.Services.AddScoped<AnalysisService>();
builder.Services.AddScoped<PredictionService>();

// --- 3. Bind Configuration ---
// SAWSettings DIHAPUS sesuai Roadmap Poin 13 untuk menyederhanakan kode.
// Jika Anda membutuhkan nilai dari appsettings.json, Service bisa menyuntikkan IConfiguration secara langsung.

var app = builder.Build();

// --- 4. Middleware Pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Penting: UseSession harus diletakkan setelah UseRouting dan sebelum UseAuthorization 
// agar data kuesioner tidak hilang saat pindah halaman.
app.UseSession(); 

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();