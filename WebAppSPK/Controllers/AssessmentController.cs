using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SPKDomain.Interfaces;
using SPKCore.Services;
using WebAppSPK.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppSPK.Controllers
{
    public class AssessmentController : Controller
    {
        private readonly IDataRepository _repository;
        private readonly AnalysisService _analysisService; // Dependency Kamus Pusat

        public AssessmentController(IDataRepository repository, AnalysisService analysisService)
        {
            _repository = repository;
            _analysisService = analysisService;
        }

        [HttpGet]
        public async Task<IActionResult> Biodata()
        {
            var salaries = await _repository.GetSalariesAsync();
            var populations = await _repository.GetPopulationsAsync();

            var viewModel = new BiodataVM
            {
                // Menggunakan _analysisService.Translate untuk mapping Bidang Pekerjaan
                BidangOptions = salaries.Select(s => s.JobName).Distinct()
                    .Select(name => new SelectListItem 
                    { 
                        Value = name, 
                        Text = _analysisService.Translate(name) 
                    })
                    .OrderBy(x => x.Text)
                    .ToList(),
                
                // Menggunakan _analysisService.Translate untuk mapping Prefektur
                PrefekturOptions = populations.Select(p => p.AreaName).Distinct()
                    .Select(name => new SelectListItem 
                    { 
                        Value = name, 
                        Text = _analysisService.Translate(name)
                    })
                    .OrderBy(x => x.Text)
                    .ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Biodata(BiodataVM model)
        {
            if (!ModelState.IsValid) return View(model);

            TempData["UserNama"] = model.Nama;
            TempData["UserLokasi"] = model.Lokasi;
            TempData["UserBidang"] = model.Bidang;

            return RedirectToAction("Kuesioner");
        }

        [HttpGet]
        public IActionResult Kuesioner()
        {
            if (TempData["UserNama"] == null) return RedirectToAction("Biodata");
            TempData.Keep(); 

            var questions = new List<QuestionVM>
            {
                new() { Index = 0, Category = "Salary", Text = "Seberapa penting faktor GAJI TINGGI bagi Anda?" },
                new() { Index = 1, Category = "CPI", Text = "Seberapa besar kekhawatiran Anda terhadap BIAYA HIDUP?" },
                new() { Index = 2, Category = "Company", Text = "Seberapa penting kemudahan menemukan PELUANG KERJA?" },
                new() { Index = 3, Category = "Population", Text = "Apakah Anda menyukai LINGKUNGAN RAMAI (Kota Besar)?" },
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
            if (answers == null || !answers.Any()) 
            {
                TempData.Keep();
                return RedirectToAction("Kuesioner");
            }
            var scores = answers.OrderBy(a => a.Index).Select(a => a.SelectedScore).ToArray();
            TempData["Answers"] = scores;
            TempData.Keep();
            return RedirectToAction("Gaji", "Analysis");
        }
    }
}