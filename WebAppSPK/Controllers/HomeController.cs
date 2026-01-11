using Microsoft.AspNetCore.Mvc;
using WebAppSPK.Models;
using System.Diagnostics;

namespace WebAppSPK.Controllers
{
    /// <summary>
    /// Controller utama untuk mengelola halaman landas (Landing Page).
    /// Memberikan informasi awal tentang metodologi sistem kepada mahasiswa.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Menyiapkan informasi transparan untuk Landing Page (Transparansi Sistem)
            ViewData["DataSource"] = "e-Stat Japan (Official Government Statistics)";
            ViewData["AnalysisMethod"] = "Simple Additive Weighting (SAW)";
            ViewData["MLModel"] = "Sentiment & Readiness Classifier";
            ViewData["LastUpdate"] = "2024-2025 Dataset Cycle";
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}