using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using SPKDomain.Interfaces;
using SPKCore.Services;
using WebAppSPK.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace WebAppSPK.Controllers
{
    public class AssessmentController : Controller
    {
        private readonly IDataRepository _repository;

        public AssessmentController(IDataRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Biodata()
        {
            // Mengambil data unik langsung dari metode repository yang baru
            var areas = await _repository.GetAvailableAreasAsync();
            var jobs = await _repository.GetAvailableJobsAsync();

            var viewModel = new BiodataVM
            {
                // Mapping otomatis dari List<string> ke List<SelectListItem>
                PrefekturOptions = areas.Select(a => new SelectListItem { Value = a, Text = a }).ToList(),
                BidangOptions = jobs.Select(j => new SelectListItem { Value = j, Text = j }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Biodata(BiodataVM model)
        {
            if (!ModelState.IsValid) return View(model);

            // Simpan ke Session agar tahan lama sampai laporan akhir
            HttpContext.Session.SetString("UserNama", model.Nama);
            HttpContext.Session.SetString("SelectedArea", model.Lokasi);
            HttpContext.Session.SetString("SelectedJob", model.Bidang);

            return RedirectToAction("Kuesioner");
        }

        [HttpGet]
        public IActionResult Kuesioner()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserNama"))) 
                return RedirectToAction("Biodata");

            var questions = new List<QuestionVM>
            {
                // SAW Criteria (Weights)
                new() { Index = 0, Category = "Salary", Text = "Seberapa penting faktor GAJI TINGGI bagi Anda?" },
                new() { Index = 1, Category = "CPI", Text = "Seberapa besar kekhawatiran Anda terhadap BIAYA HIDUP?" },
                new() { Index = 2, Category = "Company", Text = "Seberapa penting kemudahan menemukan PELUANG KERJA?" },
                new() { Index = 3, Category = "Population", Text = "Apakah Anda menyukai LINGKUNGAN RAMAI (Kota Besar)?" },
                
                // ML Criteria (Preparedness)
                new() { Index = 4, Category = "Komitmen", Text = "Tetap belajar Kanji 2 jam tanpa distraksi meski lelah?" },
                new() { Index = 5, Category = "Adaptasi", Text = "Sanggup tidak memegang smartphone selama 8 jam kerja?" },
                new() { Index = 6, Category = "Mental", Text = "Mampu bertahan saat homesick berat tanpa berniat pulang?" },
                new() { Index = 7, Category = "Fisik", Text = "Yakin mampu bekerja berdiri/membungkuk di suhu ekstrem?" },
                new() { Index = 8, Category = "Hukum", Text = "Janji tidak akan kabur menjadi pekerja ilegal?" },
                new() { Index = 9, Category = "Resiliensi", Text = "Cepat beradaptasi dengan makanan Jepang?" },
                new() { Index = 10, Category = "Etika", Text = "Disiplin memisahkan sampah sesuai aturan ketat?" },
                new() { Index = 11, Category = "Komunikasi", Text = "Siap mempraktikkan konsep Hou-Ren-Sou secara rutin?" }
            };
            return View(questions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Kuesioner(List<QuestionVM> answers)
        {
            if (answers == null || !answers.Any()) return RedirectToAction("Kuesioner");

            var scores = answers.OrderBy(a => a.Index).Select(a => a.SelectedScore).ToList();
            
            // Simpan jawaban ke Session dalam bentuk JSON
            HttpContext.Session.SetString("UserAnswers", JsonSerializer.Serialize(scores));
            
            return RedirectToAction("Gaji", "Analysis");
        }
    }
}