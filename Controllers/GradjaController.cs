using Microsoft.AspNetCore.Mvc;
using BibliotekaPPP.Models.ViewModels;
using BibliotekaPPP.Models.BusinessObjects;
using BibliotekaPPP.Models.EFRepository;
using Microsoft.Identity.Client;

namespace BibliotekaPPP.Controllers
{
    public class GradjaController : Controller
    {
        GradjaRepository gradjaRepository = new GradjaRepository();

        // [1.1.1.1] Pretraga kataloga građe uz filtriranje po dostupnosti za pozajmljivanje
        [HttpGet]
        public IActionResult Pretraga()
        {
            ViewBag.Gradja = null;
            return View("Pretraga", null);
        }

        // [1.1.1.1] Pretraga kataloga građe uz filtriranje po dostupnosti za pozajmljivanje
        [HttpPost]
        public IActionResult Pretraga(GradjaPretragaViewModel gradja)
        {
            List<GradjaBO> pretrazenaGradja = (List<GradjaBO>)gradjaRepository.TraziGradju(
                naslov: gradja.Naslov,
                imePrezimeAutora: gradja.Autor,
                naseljeIzdavanja: gradja.NaseljeIzdavanja,
                nazivIzdavaca: gradja.NazivIzdavaca,
                godinaIzdavanja: gradja.GodinaIzdavanja,
                udk: gradja.Udk,
                ogranak: gradja.Ogranak,
                statusDostupnosti: gradja.StatusDostupnosti
            );

            ViewBag.Gradja = pretrazenaGradja;
            return View("Pretraga", gradja);
        }
    }
}
